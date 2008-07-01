using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Expressions;
using Remotion.Data.Linq.Parsing.FieldResolving;
using Remotion.Data.Linq.Visitor;
using Remotion.Utilities;

namespace Remotion.Data.Linq
{
  public class QueryModel : IQueryElement
  {
    private readonly List<IBodyClause> _bodyClauses = new List<IBodyClause> ();

    private readonly Dictionary<string, IResolveableClause> _clausesByIdentifier = new Dictionary<string, IResolveableClause> ();

    private Expression _expressionTree;

    public QueryModel (Type resultType, MainFromClause mainFromClause, ISelectGroupClause selectOrGroupClause, Expression expressionTree)
    {
      ArgumentUtility.CheckNotNull ("resultType", resultType);
      ArgumentUtility.CheckNotNull ("mainFromClause", mainFromClause);
      ArgumentUtility.CheckNotNull ("SelectOrGroupClause", selectOrGroupClause);

      ResultType = resultType;
      MainFromClause = mainFromClause;
      SelectOrGroupClause = selectOrGroupClause;
      _expressionTree = expressionTree;
    }

    public QueryModel (Type resultType, MainFromClause fromClause, ISelectGroupClause selectOrGroupClause)
        : this (resultType, fromClause, selectOrGroupClause, null)
    {
    }

    public Type ResultType { get; private set; }
    public QueryModel ParentQuery { get; private set; }

    public void SetParentQuery(QueryModel parentQuery)
    {
      ArgumentUtility.CheckNotNull ("parentQueryExpression", parentQuery);
      if (ParentQuery != null)
        throw new InvalidOperationException ("The query already has a parent query.");

      ParentQuery = parentQuery;
    }

    public MainFromClause MainFromClause { get; private set; }
    public ISelectGroupClause SelectOrGroupClause { get; private set; }

    public ReadOnlyCollection<IBodyClause> BodyClauses
    {
      get { return _bodyClauses.AsReadOnly (); }
    }

    public void AddBodyClause (IBodyClause clause)
    {
      ArgumentUtility.CheckNotNull ("clause", clause);

      var clauseAsFromClause = clause as FromClauseBase;
      if (clauseAsFromClause != null)
        RegisterClause(clauseAsFromClause.Identifier, clauseAsFromClause);

      var clauseAsLetClause = clause as LetClause;
      if (clauseAsLetClause != null)
        RegisterClause (clauseAsLetClause.Identifier, clauseAsLetClause);

      clause.SetQueryModel (this);
      _bodyClauses.Add (clause);
    }

    private void RegisterClause (ParameterExpression identifier, IResolveableClause clauseToBeRegistered)
    {
      if (MainFromClause.Identifier.Name == identifier.Name || _clausesByIdentifier.ContainsKey (identifier.Name))
      {
        string message = string.Format ("Multiple clauses with the same identifier name ('{0}') are not supported.",
            identifier.Name);
        throw new InvalidOperationException (message);
      }
      _clausesByIdentifier.Add (identifier.Name, clauseToBeRegistered);
    }

    public IResolveableClause GetResolveableClause (string identifierName, Type identifierType)
    {
      ArgumentUtility.CheckNotNull ("identifierName", identifierName);
      ArgumentUtility.CheckNotNull ("identifierType", identifierType);

      if (identifierName == MainFromClause.Identifier.Name)
      {
        CheckResolvedIdentifierType (MainFromClause.Identifier, identifierType);
        return MainFromClause;
      }
      else
        return GetBodyClauseByIdentifier (identifierName, identifierType);
    }

    private IResolveableClause GetBodyClauseByIdentifier (string identifierName, Type identifierType)
    {
      ArgumentUtility.CheckNotNull ("identifierName", identifierName);
      ArgumentUtility.CheckNotNull ("identifierType", identifierType);

      IResolveableClause clause;
      if (_clausesByIdentifier.TryGetValue (identifierName, out clause))
      {
        CheckResolvedIdentifierType (clause.Identifier,identifierType);
        return clause;
      }
      return null;
    }


    public void Accept (IQueryVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitQueryModel (this);
    }

    public override string ToString ()
    {
      var sv = new StringVisitor();
      sv.VisitQueryModel (this);
      return sv.ToString();
    }

    // Once we have a working ExpressionTreeBuildingVisitor, we could use it to build trees for constructed models. For now, we just create
    // a special ConstructedExpression node.
    public Expression GetExpressionTree ()
    {
      if (_expressionTree == null)
        return new ConstructedQueryExpression (this);
      else
        return _expressionTree;
    }

    public FieldDescriptor ResolveField (ClauseFieldResolver resolver, Expression fieldAccessExpression, JoinedTableContext joinedTableContext)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("fieldAccessExpression", fieldAccessExpression);
      ArgumentUtility.CheckNotNull ("joinedTableContext", joinedTableContext);
      
      return new QueryModelFieldResolver (this).ResolveField (resolver, fieldAccessExpression, joinedTableContext);
    }

    private void CheckResolvedIdentifierType (ParameterExpression identifier,Type expectedType)
    {
      if (identifier.Type != expectedType)
      {
        string message = string.Format ("The from clause with identifier '{0}' has type '{1}', but '{2}' was requested.", identifier.Name,
            identifier.Type, expectedType);
        throw new ClauseLookupException (message);
      }
    }
  }
}