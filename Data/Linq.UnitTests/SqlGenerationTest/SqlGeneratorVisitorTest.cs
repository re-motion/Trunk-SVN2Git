/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.Parsing.FieldResolving;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Data.Linq.UnitTests.SqlGenerationTest
{
  [TestFixture]
  public class SqlGeneratorVisitorTest
  {
    private JoinedTableContext _context;
    private ParseMode _parseMode;

    [SetUp]
    public void SetUp()
    {
      _context = new JoinedTableContext();
      _parseMode = new ParseMode();
    }

    [Test]
    public void VisitSelectClause ()
    {
      IQueryable<Tuple<string, string>> query = SelectTestQueryGenerator.CreateSimpleQueryWithFieldProjection (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      SelectClause selectClause = (SelectClause) parsedQuery.SelectOrGroupClause;

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitSelectClause (selectClause);

      NewObject expectedNewObject = new NewObject (typeof (Tuple<string, string>).GetConstructors()[0], new IEvaluation[] {
          new Column (new Table ("studentTable", "s"), "FirstColumn"),
          new Column (new Table ("studentTable", "s"), "LastColumn")});
      Assert.That (sqlGeneratorVisitor.SqlGenerationData.SelectEvaluations, Is.EqualTo (new object[] { expectedNewObject}));
    }


    [Test]
    public void VisitSelectClause_WithNullProjection ()
    {
      IQueryable<Student> query = WhereTestQueryGenerator.CreateSimpleWhereQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      SelectClause selectClause = (SelectClause) parsedQuery.SelectOrGroupClause;

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitSelectClause (selectClause);    
      Assert.That (sqlGeneratorVisitor.SqlGenerationData.SelectEvaluations, Is.EqualTo (new object[] { new Column (new Table ("studentTable", "s"), "*") }));
    }
    

    [Test]
    public void VisitLetClause ()
    {
      IQueryable<string> query = LetTestQueryGenerator.CreateSimpleLetClause (ExpressionHelper.CreateQuerySource ());

      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      LetClause letClause = (LetClause) parsedQuery.BodyClauses.First ();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitLetClause (letClause);

      BinaryEvaluation expectedResult = 
        new BinaryEvaluation(new Column (new Table ("studentTable", "s"), "FirstColumn"),new Column (new Table ("studentTable", "s"), "LastColumn"),
          BinaryEvaluation.EvaluationKind.Add);
      
      Assert.That(sqlGeneratorVisitor.SqlGenerationData.LetEvaluations.First().Evaluation, Is.EqualTo (expectedResult));
      Assert.AreEqual (letClause.Identifier.Name, sqlGeneratorVisitor.SqlGenerationData.LetEvaluations.First ().Name);
    }

    [Test]
    public void VisitLetClause_WithJoin ()
    {
      IQueryable<string> query = LetTestQueryGenerator.CreateLet_WithJoin_NoTable (ExpressionHelper.CreateQuerySource_Detail ());

      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      LetClause letClause = (LetClause) parsedQuery.BodyClauses.First ();
      IColumnSource studentDetailTable = parsedQuery.MainFromClause.GetFromSource (StubDatabaseInfo.Instance);

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitLetClause (letClause);
      PropertyInfo relationMember = typeof (Student_Detail).GetProperty ("Student");
      Table studentTable = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, relationMember);


      Column expectedResult = new Column (new Table ("studentTable", "s"), "FirstColumn");

      Column c1 = new Column (studentDetailTable, "Student_Detail_PK");
      Column c2 = new Column (studentTable, "Student_Detail_to_Student_FK");

      SingleJoin expectedJoin = new SingleJoin (c1, c2);
      Assert.AreEqual (1, sqlGeneratorVisitor.SqlGenerationData.Joins.Count);
      
      SingleJoin actualJoin = sqlGeneratorVisitor.SqlGenerationData.Joins[studentDetailTable].First ();
      Assert.AreEqual (expectedJoin, actualJoin);
    }
    
    [Test]
    public void VisitSelectClause_DistinctFalse ()
    {
      IQueryable<Tuple<string, string>> query = SelectTestQueryGenerator.CreateSimpleQueryWithFieldProjection (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      SelectClause selectClause = (SelectClause) parsedQuery.SelectOrGroupClause;

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitSelectClause (selectClause);

      Assert.IsFalse (selectClause.Distinct);
    }
    
    [Test]
    public void VisitSelectClause_DistinctTrue ()
    {
      IQueryable<string> query = DistinctTestQueryGenerator.CreateSimpleDistinctQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      SelectClause selectClause = (SelectClause) parsedQuery.SelectOrGroupClause;

      Assert.IsTrue (selectClause.Distinct);
    }

    [Test]
    public void VisitSelectClause_WithJoins ()
    {
      IQueryable<string> query = JoinTestQueryGenerator.CreateSimpleImplicitSelectJoin (ExpressionHelper.CreateQuerySource_Detail ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      SelectClause selectClause = (SelectClause) parsedQuery.SelectOrGroupClause;
      sqlGeneratorVisitor.VisitSelectClause (selectClause);

      PropertyInfo relationMember = typeof (Student_Detail).GetProperty ("Student");
      IColumnSource studentDetailTable = parsedQuery.MainFromClause.GetFromSource (StubDatabaseInfo.Instance);
      Table studentTable = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, relationMember);
      Tuple<string, string> joinSelectEvaluations = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, relationMember);
      SingleJoin join = new SingleJoin (new Column (studentDetailTable, joinSelectEvaluations.A), new Column (studentTable, joinSelectEvaluations.B));
     
      Assert.AreEqual (1, sqlGeneratorVisitor.SqlGenerationData.Joins.Count);
      
      List<SingleJoin> actualJoins = sqlGeneratorVisitor.SqlGenerationData.Joins[studentDetailTable];
      Assert.That (actualJoins, Is.EqualTo (new object[] { join }));
    }

    [Test]
    public void VisitSelectClause_UsesContext ()
    {
      Assert.AreEqual (0, _context.Count);
      VisitSelectClause_WithJoins();
      Assert.AreEqual (1, _context.Count);
    }

    [Test]
    public void VisitMainFromClause()
    {
      IQueryable<Student> query = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      MainFromClause fromClause = parsedQuery.MainFromClause;

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitMainFromClause (fromClause);

      Assert.That (sqlGeneratorVisitor.SqlGenerationData.FromSources, Is.EqualTo (new object[] { new Table ("studentTable", "s") }));
    }

    [Test]
    public void VisitAdditionalFromClause ()
    {
      IQueryable<Student> query = MixedTestQueryGenerator.CreateMultiFromWhereQuery (ExpressionHelper.CreateQuerySource (), ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      AdditionalFromClause fromClause = (AdditionalFromClause)parsedQuery.BodyClauses.First();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitAdditionalFromClause (fromClause);

      Assert.That (sqlGeneratorVisitor.SqlGenerationData.FromSources, Is.EqualTo (new object[] { new Table ("studentTable", "s2") }));
    }

    [Test]
    public void VisitSubQueryFromClause ()
    {
      IQueryable<Student> query = SubQueryTestQueryGenerator.CreateSimpleSubQueryInAdditionalFromClause (ExpressionHelper.CreateQuerySource());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      SubQueryFromClause subQueryFromClause = (SubQueryFromClause) parsedQuery.BodyClauses.First();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitSubQueryFromClause (subQueryFromClause);

      Assert.That (sqlGeneratorVisitor.SqlGenerationData.FromSources, Is.EqualTo (new object[] { subQueryFromClause.GetFromSource (StubDatabaseInfo.Instance) }));
    }

    [Test]
    public void VisitWhereClause()
    {
      IQueryable<Student> query = WhereTestQueryGenerator.CreateSimpleWhereQuery (ExpressionHelper.CreateQuerySource());
      
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);

      WhereClause whereClause = (WhereClause)parsedQuery.BodyClauses.First();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitWhereClause (whereClause);

      Assert.AreEqual (new BinaryCondition (new Column (new Table ("studentTable", "s"), "LastColumn"),
          new Constant ("Garcia"), BinaryCondition.ConditionKind.Equal),
          sqlGeneratorVisitor.SqlGenerationData.Criterion);
    }

    [Test]
    public void VisitWhereClause_MultipleTimes ()
    {
      IQueryable<Student> query = WhereTestQueryGenerator.CreateMultiWhereQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);

      WhereClause whereClause1 = (WhereClause) parsedQuery.BodyClauses[0];
      WhereClause whereClause2 = (WhereClause) parsedQuery.BodyClauses[1];
      WhereClause whereClause3 = (WhereClause) parsedQuery.BodyClauses[2];

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitWhereClause (whereClause1);
      sqlGeneratorVisitor.VisitWhereClause (whereClause2);

      var condition1 = new BinaryCondition (new Column (new Table ("studentTable", "s"), "LastColumn"), new Constant ("Garcia"), 
          BinaryCondition.ConditionKind.Equal);
      var condition2 = new BinaryCondition (new Column (new Table ("studentTable", "s"), "FirstColumn"), new Constant ("Hugo"),
          BinaryCondition.ConditionKind.Equal);
      var combination12 = new ComplexCriterion (condition1, condition2, ComplexCriterion.JunctionKind.And);
      Assert.AreEqual (combination12, sqlGeneratorVisitor.SqlGenerationData.Criterion);
      
      sqlGeneratorVisitor.VisitWhereClause (whereClause3);

      var condition3 = new BinaryCondition (new Column (new Table ("studentTable", "s"), "IDColumn"), new Constant (100),
          BinaryCondition.ConditionKind.GreaterThan);
      var combination123 = new ComplexCriterion (combination12, condition3, ComplexCriterion.JunctionKind.And);
      Assert.AreEqual (combination123, sqlGeneratorVisitor.SqlGenerationData.Criterion);
    }
    
    [Test]
    public void VisitOrderingClause()
    {
      IQueryable<Student> query = OrderByTestQueryGenerator.CreateSimpleOrderByQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      OrderByClause orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      OrderingClause orderingClause = orderBy.OrderingList.First ();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitOrderingClause (orderingClause);

      FieldDescriptor fieldDescriptor = ExpressionHelper.CreateFieldDescriptor (parsedQuery.MainFromClause, typeof (Student).GetProperty ("First"));
      Assert.That (sqlGeneratorVisitor.SqlGenerationData.OrderingFields,
          Is.EqualTo (new object[] { new OrderingField (fieldDescriptor, OrderDirection.Asc) }));
    }

    [Test]
    public void VisitOrderingClause_WithJoins ()
    {
      IQueryable<Student_Detail> query = JoinTestQueryGenerator.CreateSimpleImplicitOrderByJoin (ExpressionHelper.CreateQuerySource_Detail ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      OrderByClause orderBy = (OrderByClause) parsedQuery.BodyClauses.First ();
      OrderingClause orderingClause = orderBy.OrderingList.First ();

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitOrderingClause (orderingClause);

      PropertyInfo relationMember = typeof (Student_Detail).GetProperty ("Student");
      IColumnSource sourceTable = parsedQuery.MainFromClause.GetFromSource (StubDatabaseInfo.Instance);
      Table relatedTable = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, relationMember); // Student
      Tuple<string, string> columns = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, relationMember);

      SingleJoin join = new SingleJoin (new Column (sourceTable, columns.A), new Column (relatedTable, columns.B));

      Assert.AreEqual (1, sqlGeneratorVisitor.SqlGenerationData.Joins.Count);
      List<SingleJoin> actualJoins = sqlGeneratorVisitor.SqlGenerationData.Joins[sourceTable];
      Assert.That (actualJoins, Is.EqualTo (new object[] { join }));
    }

    [Test]
    public void VisitOrderingClause_UsesContext()
    {
      Assert.AreEqual (0, _context.Count);
      VisitOrderingClause_WithJoins();
      Assert.AreEqual (1, _context.Count);
    }

    [Test]
    public void VisitOrderByClause ()
    {
      IQueryable<Student> query = OrderByTestQueryGenerator.CreateThreeOrderByQuery (ExpressionHelper.CreateQuerySource ());
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      OrderByClause orderBy1 = (OrderByClause) parsedQuery.BodyClauses.First ();

      FieldDescriptor fieldDescriptor1 = ExpressionHelper.CreateFieldDescriptor (parsedQuery.MainFromClause, typeof (Student).GetProperty ("First"));
      FieldDescriptor fieldDescriptor2 = ExpressionHelper.CreateFieldDescriptor (parsedQuery.MainFromClause, typeof (Student).GetProperty ("Last"));

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitOrderByClause (orderBy1);
      Assert.That (sqlGeneratorVisitor.SqlGenerationData.OrderingFields,
          Is.EqualTo (new object[]
              {
                new OrderingField (fieldDescriptor1, OrderDirection.Asc),
                new OrderingField (fieldDescriptor2, OrderDirection.Asc),
              }));
    }
    
    [Test]
    public void VisitWhereClause_WithJoins ()
    {
      IQueryable<Student_Detail> query = JoinTestQueryGenerator.CreateSimpleImplicitWhereJoin (ExpressionHelper.CreateQuerySource_Detail ());
      
      QueryModel parsedQuery = ExpressionHelper.ParseQuery (query);
      WhereClause whereClause = ClauseFinder.FindClause<WhereClause> (parsedQuery.SelectOrGroupClause);

      DetailParser detailParser = new DetailParser (StubDatabaseInfo.Instance, _parseMode);
      SqlGeneratorVisitor sqlGeneratorVisitor = new SqlGeneratorVisitor (StubDatabaseInfo.Instance, ParseMode.TopLevelQuery,detailParser, new ParseContext (parsedQuery, parsedQuery.GetExpressionTree(), new List<FieldDescriptor>(), _context));
      sqlGeneratorVisitor.VisitWhereClause (whereClause);

      PropertyInfo relationMember = typeof (Student_Detail).GetProperty ("Student");
      IColumnSource sourceTable = parsedQuery.MainFromClause.GetFromSource (StubDatabaseInfo.Instance); // Student_Detail
      Table relatedTable = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, relationMember); // Student
      Tuple<string, string> columns = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, relationMember);
      SingleJoin join = new SingleJoin (new Column (sourceTable, columns.A), new Column (relatedTable, columns.B));

      Assert.AreEqual (1, sqlGeneratorVisitor.SqlGenerationData.Joins.Count);

      List<SingleJoin> actualJoins = sqlGeneratorVisitor.SqlGenerationData.Joins[sourceTable];

      Assert.That (actualJoins, Is.EqualTo (new object[] { join }));
    }

    [Test]
    public void VisitWhereClause_UsesContext ()
    {
      Assert.AreEqual (0, _context.Count);
      VisitWhereClause_WithJoins ();
      Assert.AreEqual (1, _context.Count);
    }

  }
}
