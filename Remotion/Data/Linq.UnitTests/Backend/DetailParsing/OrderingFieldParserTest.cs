// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Remotion.Data.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Data.Linq.UnitTests.Backend.DetailParsing
{
  [TestFixture]
  public class OrderingFieldParserTest
  {
    private JoinedTableContext _joinedTableContext;
    private OrderingFieldParser _parser;

    [SetUp]
    public void SetUp ()
    {
      _joinedTableContext = new JoinedTableContext (StubDatabaseInfo.Instance);
      _parser = new OrderingFieldParser (StubDatabaseInfo.Instance);
    }

    [Test]
    public void SimpleOrderingClause ()
    {
      IQueryable<Chef> query = OrderByTestQueryGenerator.CreateSimpleOrderByQuery (ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      var expectedFieldDescriptor = ExpressionHelper.CreateFieldDescriptor (_joinedTableContext, parsedQuery.MainFromClause, typeof (Chef).GetProperty ("FirstName"));
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Asc)));
    }

    [Test]
    public void TwoOrderingClause_FirstClause ()
    {
      IQueryable<Chef> query = OrderByTestQueryGenerator.CreateTwoOrderByQuery (ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      var expectedFieldDescriptor = ExpressionHelper.CreateFieldDescriptor (_joinedTableContext, parsedQuery.MainFromClause, typeof (Chef).GetProperty ("FirstName"));
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Asc)));
    }

    [Test]
    public void TwoOrderingClause_SecondClause ()
    {
      IQueryable<Chef> query = OrderByTestQueryGenerator.CreateTwoOrderByQuery (ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.Last ();
      var ordering = orderBy.Orderings.Last ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      FieldDescriptor expectedFieldDescriptor = ExpressionHelper.CreateFieldDescriptor (_joinedTableContext, parsedQuery.MainFromClause, typeof (Chef).GetProperty ("Name"));
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Desc)));
    }

    [Test]
    public void ComplexOrderingClause_FirstOrdering ()
    {
      IQueryable<Chef> query =
          MixedTestQueryGenerator.CreateMultiFromWhereOrderByQuery (ExpressionHelper.CreateStudentQueryable (), ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.Skip (2).First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      FieldDescriptor expectedFieldDescriptor = ExpressionHelper.CreateFieldDescriptor (_joinedTableContext, parsedQuery.MainFromClause, typeof (Chef).GetProperty ("FirstName"));
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Asc)));
    }

    [Test]
    public void ComplexOrderingClause_SecondOrdering ()
    {
      IQueryable<Chef> query =
          MixedTestQueryGenerator.CreateMultiFromWhereOrderByQuery (ExpressionHelper.CreateStudentQueryable (), ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.Skip (2).First ();
      var ordering = orderBy.Orderings.Last ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      FieldDescriptor expectedFieldDescriptor = ExpressionHelper.CreateFieldDescriptor (_joinedTableContext, (FromClauseBase) parsedQuery.BodyClauses[0], typeof (Chef).GetProperty ("Name"));
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Desc)));
    }

    [Test]
    [ExpectedException (typeof (FieldAccessResolveException), ExpectedMessage = "The member 'Remotion.Data.Linq.UnitTests.TestDomain.Chef.NonDBStringProperty' "
        + "does not identify a queryable column.")]
    public void OrderingClause_WithNonDBField ()
    {
      IQueryable<Chef> query = OrderByTestQueryGenerator.CreateOrderByNonDBPropertyQuery (ExpressionHelper.CreateStudentQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);
    }

    [Test]
    public void JoinOrderingClause ()
    {
      IQueryable<Student_Detail> query = JoinTestQueryGenerator.CreateSimpleImplicitOrderByJoin (ExpressionHelper.CreateStudentDetailQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor>(), _joinedTableContext);
      var result = _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);

      FromClauseBase fromClause = parsedQuery.MainFromClause;
      PropertyInfo relationMember = typeof (Student_Detail).GetProperty ("Chef");
      IColumnSource sourceTable = _joinedTableContext.GetColumnSource (fromClause); // Student_Detail
      Table relatedTable = StubDatabaseInfo.Instance.GetTableForRelation (relationMember, null); // Chef
      var join = StubDatabaseInfo.Instance.GetJoinForMember (relationMember, sourceTable, relatedTable);

      PropertyInfo orderingMember = typeof (Chef).GetProperty ("FirstName");
      var path = new FieldSourcePath (sourceTable, new[] { join });
      var column = StubDatabaseInfo.Instance.GetColumnForMember (relatedTable, orderingMember);
      var expectedFieldDescriptor = new FieldDescriptor (orderingMember, path, column);
      
      Assert.That (result, Is.EqualTo (new OrderingField (expectedFieldDescriptor, OrderingDirection.Asc)));
    }

    [Test]
    public void ParserUsesContext()
    {
      Assert.That (_joinedTableContext.Count, Is.EqualTo (0));
      JoinOrderingClause();
      Assert.That (_joinedTableContext.Count, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Ordering by 'Remotion.Data.Linq.UnitTests.TestDomain.Student_Detail.Chef' "
        + "is not supported because it is a relation member.")]
    public void OrderingOnRelationMemberThrows()
    {
      IQueryable<Student_Detail> query = OrderByTestQueryGenerator.CreateRelationMemberOrderByQuery (ExpressionHelper.CreateStudentDetailQueryable ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      var orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      var ordering = orderBy.Orderings.First ();

      var parseContext = new ParseContext (parsedQuery, new List<FieldDescriptor> (), _joinedTableContext);
      _parser.Parse (ordering.Expression, parseContext, ordering.OrderingDirection);
    }
  }
}
