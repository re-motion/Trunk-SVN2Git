// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Collections;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultModifications;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Utilities;

namespace Remotion.Data.Linq
{
  /// <summary>
  /// Provides an abstraction of an expression tree created for a LINQ query. <see cref="QueryModel"/> instances are passed to LINQ providers based
  /// on re-linq via <see cref="IQueryExecutor"/>, but you can also use <see cref="QueryParser"/> to parse an expression tree by hand or construct
  /// a <see cref="QueryModel"/> manually via its constructor.
  /// </summary>
  /// <remarks>
  /// The different parts of the query are mapped to clauses, see <see cref="MainFromClause"/>, <see cref="BodyClauses"/>, and 
  /// <see cref="SelectOrGroupClause"/>. The simplest way to process all the clauses belonging to a <see cref="QueryModel"/> is by implementing
  /// <see cref="IQueryModelVisitor"/> (or deriving from <see cref="QueryModelVisitorBase"/>) and calling <see cref="Accept"/>.
  /// </remarks>
  public class QueryModel : ICloneable
  {
    private readonly UniqueIdentifierGenerator _uniqueIdentifierGenerator;

    private Type _resultType;
    private MainFromClause _mainFromClause;
    private ISelectGroupClause _selectOrGroupClause;

    /// <summary>
    /// Initializes a new instance of <see cref="QueryModel"/>
    /// </summary>
    /// <param name="resultType">The type of the underlying LINQ query, usually a type implementing <see cref="IQueryable{T}"/>.</param>
    /// <param name="mainFromClause">The <see cref="Clauses.MainFromClause"/> of the query. This is the starting point of the query, generating items 
    /// that are filtered and projected by the query.</param>
    /// <param name="selectOrGroupClause">The <see cref="SelectClause"/> or <see cref="GroupClause"/> of the query. This is the end point of
    /// the query, it defines what is atually returned for each of the items coming from the <see cref="MainFromClause"/> and passing the 
    /// <see cref="BodyClauses"/>.</param>
    public QueryModel (Type resultType, MainFromClause mainFromClause, ISelectGroupClause selectOrGroupClause)
    {
      ArgumentUtility.CheckNotNull ("resultType", resultType);
      ArgumentUtility.CheckNotNull ("mainFromClause", mainFromClause);
      ArgumentUtility.CheckNotNull ("SelectOrGroupClause", selectOrGroupClause);

      _uniqueIdentifierGenerator = new UniqueIdentifierGenerator();
      ResultType = resultType;
      MainFromClause = mainFromClause;
      SelectOrGroupClause = selectOrGroupClause;

      BodyClauses = new ObservableCollection<IBodyClause> ();
      BodyClauses.ItemInserted += BodyClauses_Added;
      BodyClauses.ItemSet += BodyClauses_Added;
    }

    /// <summary>
    /// Gets or sets the result type of the underlying LINQ query. This is usually a type that implements <see cref="IQueryable{T}"/>, unless the
    /// query ends with a <see cref="ResultOperatorBase"/>. For example, if the query ends with a <see cref="CountResultOperator"/>, the
    /// result type will be <see cref="int"/>.
    /// </summary>
    public Type ResultType
    {
      get { return _resultType; }
      set { _resultType = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets or sets the query's <see cref="Clauses.MainFromClause"/>. This is the starting point of the query, generating items that are processed by 
    /// the <see cref="BodyClauses"/> and projected or grouped by the <see cref="SelectOrGroupClause"/>.
    /// </summary>
    public MainFromClause MainFromClause
    {
      get { return _mainFromClause; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _mainFromClause = value;
        _uniqueIdentifierGenerator.AddKnownIdentifier (value.ItemName);
      }
    }

    /// <summary>
    /// Gets or sets the query's select or group clause. This is the end point of the query, it defines what is atually returned for each of the 
    /// items coming from the <see cref="MainFromClause"/> and passing the <see cref="BodyClauses"/>.
    /// </summary>
    public ISelectGroupClause SelectOrGroupClause
    {
      get { return _selectOrGroupClause; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _selectOrGroupClause = value;
      }
    }

    /// <summary>
    /// Gets a collection representing the query's body clauses. Body clauses take the items generated by the <see cref="MainFromClause"/>,
    /// filtering (<see cref="WhereClause"/>), ordering (<see cref="OrderByClause"/>), augmenting (<see cref="AdditionalFromClause"/>), or otherwise
    /// processing them before they are passed to the <see cref="SelectOrGroupClause"/>.
    /// </summary>
    public ObservableCollection<IBodyClause> BodyClauses { get; private set; }

    /// <summary>
    /// Accepts an implementation of <see cref="IQueryModelVisitor"/> or <see cref="QueryModelVisitorBase"/>, as defined by the Visitor pattern.
    /// </summary>
    public void Accept (IQueryModelVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitQueryModel (this);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> representation of this <see cref="QueryModel"/>.
    /// </summary>
    public override string ToString ()
    {
      return MainFromClause + BodyClauses.Aggregate ("", (s, b) => s + " " + b) + " " + SelectOrGroupClause;
    }

    /// <summary>
    /// Clones this <see cref="QueryModel"/>, returning a new <see cref="QueryModel"/> equivalent to this instance, but with its clauses being
    /// clones of this instance's clauses. Any <see cref="QuerySourceReferenceExpression"/> in the cloned clauses that points back to another clause 
    /// in this <see cref="QueryModel"/> (including its subqueries) is adjusted to point to the respective clones in the cloned 
    /// <see cref="QueryModel"/>. Any subquery nested in the <see cref="QueryModel"/> is also cloned.
    /// </summary>
    public QueryModel Clone ()
    {
      return Clone (new ClauseMapping());
    }

    /// <summary>
    /// Clones this <see cref="QueryModel"/>, returning a new <see cref="QueryModel"/> equivalent to this instance, but with its clauses being
    /// clones of this instance's clauses. Any <see cref="QuerySourceReferenceExpression"/> in the cloned clauses that points back to another clause 
    /// in  this <see cref="QueryModel"/> (including its subqueries) is adjusted to point to the respective clones in the cloned 
    /// <see cref="QueryModel"/>. Any subquery nested in the <see cref="QueryModel"/> is also cloned.
    /// </summary>
    /// <param name="clauseMapping">The <see cref="ClauseMapping"/> defining how to adjust instances of 
    /// <see cref="QuerySourceReferenceExpression"/> in the cloned <see cref="QueryModel"/>. If there is a <see cref="QuerySourceReferenceExpression"/>
    /// that points out of the <see cref="QueryModel"/> being cloned, specify its replacement via this parameter. At the end of the cloning process,
    /// this object maps all the clauses in this original <see cref="QueryModel"/> to the clones created in the process.
    /// </param>
    public QueryModel Clone (ClauseMapping clauseMapping)
    {
      ArgumentUtility.CheckNotNull ("clauseMapping", clauseMapping);

      var cloneContext = new CloneContext (clauseMapping);
      var queryModelBuilder = new QueryModelBuilder();

      queryModelBuilder.AddClause (MainFromClause.Clone (cloneContext));
      foreach (var bodyClause in BodyClauses)
        queryModelBuilder.AddClause (bodyClause.Clone (cloneContext));
      queryModelBuilder.AddClause (SelectOrGroupClause.Clone (cloneContext));

      return queryModelBuilder.Build (ResultType);
    }

    object ICloneable.Clone ()
    {
      return Clone();
    }

    /// <summary>
    /// Transforms all the expressions in this <see cref="QueryModel"/>'s clauses via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this 
    /// <see cref="QueryModel"/>, and those expressions will be replaced with what the delegate returns.</param>
    public void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);

      MainFromClause.TransformExpressions (transformation);
      
      foreach (var bodyClause in BodyClauses)
        bodyClause.TransformExpressions (transformation);

      SelectOrGroupClause.TransformExpressions (transformation);
    }

    /// <summary>
    /// Returns a new name with the given prefix. The name is different from that of any <see cref="FromClauseBase"/> added
    /// in the <see cref="QueryModel"/>. Note that clause names that are changed after the clause is added as well as names of other clauses
    /// than from clauses are not considered when determining "unique" names. Use names only for readability and debugging, not
    /// for uniquely identifying clauses.
    /// </summary>
    public string GetNewName (string prefix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("prefix", prefix);
      return _uniqueIdentifierGenerator.GetUniqueIdentifier (prefix);
    }

    private void BodyClauses_Added (object sender, ObservableCollectionChangedEventArgs<IBodyClause> e)
    {
      ArgumentUtility.CheckNotNull ("e.Item", e.Item);

      var clauseAsFromClause = e.Item as FromClauseBase;
      if (clauseAsFromClause != null)
        _uniqueIdentifierGenerator.AddKnownIdentifier (clauseAsFromClause.ItemName);
    }
  }
}