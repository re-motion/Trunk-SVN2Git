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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq
{
  [TestFixture]
  public class QueryProviderBaseTest
  {
    private MockRepository _mockRepository;
    private QueryProviderBase _queryProvider;
    private IQueryExecutor _executorMock;
    private TestQueryable<Student> _queryableWithExecutorMock;
    private MethodCallExpressionNodeTypeRegistry _nodeTypeRegistry;
    private TestQueryProvider _queryProviderWithSpecificRegistry;

    private PropertyInfo _scoresMember;
    private PropertyInfo _friendsMember;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _executorMock = _mockRepository.StrictMock<IQueryExecutor>();
      _queryProvider = new TestQueryProvider (_executorMock);
      _nodeTypeRegistry = new MethodCallExpressionNodeTypeRegistry ();
      _queryProviderWithSpecificRegistry = new TestQueryProvider (_executorMock, _nodeTypeRegistry);
      _queryableWithExecutorMock = new TestQueryable<Student> (_executorMock);

      _scoresMember = typeof (Student).GetProperty ("Scores");
      _friendsMember = typeof (Student).GetProperty ("Friends");
    }

    [Test]
    public void Initialization()
    {
      Assert.IsNotNull (_queryProvider);
    }

    [Test]
    public void CreateQueryReturnsIQueryable()
    {
      Expression expression = ExpressionHelper.CreateExpression();
      IQueryable queryable = _queryProvider.CreateQuery (expression);

      Assert.IsNotNull (queryable);
      Assert.IsInstanceOfType (typeof (IQueryable<int>), queryable);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CreateQueryUnwrapsException ()
    {
      Expression expression = ExpressionHelper.CreateLambdaExpression ();
      _queryProvider.CreateQuery (expression);
    }

    [Test]
    public void GenericCreateQueryReturnsIQueryable ()
    {
      Expression expression = ExpressionHelper.CreateNewIntArrayExpression ();
      IQueryable<int> queryable = _queryProvider.CreateQuery<int> (expression);

      Assert.IsNotNull (queryable);
    }

    [Test]
    public void GenerateQueryModel ()
    {
      Expression expression = SelectTestQueryGenerator.CreateSimpleQuery_SelectExpression (ExpressionHelper.CreateStudentQueryable ());
      var queryModel = _queryProvider.GenerateQueryModel (expression);

      Assert.That (((QuerySourceReferenceExpression) queryModel.SelectClause.Selector).ReferencedQuerySource,
          Is.SameAs (queryModel.MainFromClause));
    }

    [Test]
    public void Execute_Generic ()
    {
      var expectedResult = new Student[0];
      Expression expression = (from s in _queryableWithExecutorMock select s).Expression;
      _executorMock.Expect (mock => mock.ExecuteCollection<Student> (
          Arg<QueryModel>.Is.Anything,
          Arg<FetchRequestBase[]>.List.Equal (new FetchManyRequest[0]))).Return (expectedResult);

      _executorMock.Replay ();
      var result = _queryProvider.Execute<IEnumerable<Student>> (expression);
      _executorMock.VerifyAllExpectations ();

      Assert.That (result.ToArray(), Is.EqualTo (expectedResult));
    }

    [Test]
    public void Execute_NonGeneric ()
    {
      var expectedResult = new Student[0];
      Expression expression = (from s in _queryableWithExecutorMock select s).Expression;
      _executorMock.Expect (mock => mock.ExecuteCollection<Student> (
          Arg<QueryModel>.Is.Anything,
          Arg<FetchRequestBase[]>.List.Equal (new FetchManyRequest[0]))).Return (new Student[0]);

      _executorMock.Replay ();
      var result = ((IQueryProvider)_queryProvider).Execute (expression);
      _executorMock.VerifyAllExpectations ();

      Assert.That (((IEnumerable<Student>)result).ToArray (), Is.EqualTo (expectedResult));
    }

    [Test]
    public void Execute_IntegrationWithGenericEnumerable ()
    {
      var queryable = from s in _queryableWithExecutorMock select s;

      var expectedResult = new[] { new Student () };
      _executorMock
          .Expect (mock => mock.ExecuteCollection<Student> (Arg<QueryModel>.Is.Anything, Arg<FetchRequestBase[]>.Is.Anything))
          .Return (expectedResult);

      _executorMock.Replay ();
      var result = queryable.ToArray ();
      _executorMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (expectedResult));
    }

    [Test]
    public void Execute_IntegrationWithNonGenericEnumerable ()
    {
      var queryable = from s in _queryableWithExecutorMock select s;

      var expectedResult = new[] { new Student () };
      _executorMock
          .Expect (mock => mock.ExecuteCollection<Student> (Arg<QueryModel>.Is.Anything, Arg<FetchRequestBase[]>.Is.Anything))
          .Return (expectedResult);

      _executorMock.Replay ();

      var result = new List<object> ();
      foreach (var s in (IEnumerable) queryable)
        result.Add (s);

      _executorMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (expectedResult));
    }

    [Test]
    public void Execute_IntegrationWithSingle ()
    {
      var expectedResult = new Student ();
      _executorMock
                .Expect (mock => mock.ExecuteSingle<Student> (Arg<QueryModel>.Is.Anything, Arg<FetchRequestBase[]>.Is.Anything, Arg<bool>.Is.Equal (false)))
                .Return (expectedResult);

      _executorMock.Replay ();
      var result = (from s in _queryableWithExecutorMock select s).Single();
      _executorMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs(expectedResult));
    }

    [Test]
    public void Execute_IntegrationWithScalar ()
    {
      _executorMock
                .Expect (mock => mock.ExecuteScalar<int> (Arg<QueryModel>.Is.Anything, Arg<FetchRequestBase[]>.Is.Anything))
                .Return (7);

      _executorMock.Replay ();
      var result = (from s in _queryableWithExecutorMock select s).Count ();
      _executorMock.VerifyAllExpectations();

      Assert.That (result, Is.EqualTo (7));
    }

    [Test]
    public void Execute_WithDefaultRegistry ()
    {
      Expression expression = ExpressionHelper.MakeExpression (() => from s in ExpressionHelper.CreateStudentQueryable () select s);
      _executorMock
          .Expect (mock => mock.ExecuteCollection<Student> (Arg<QueryModel>.Is.Anything, Arg<IEnumerable<FetchRequestBase>>.Is.Anything))
          .Return (new Student[0]);
      _executorMock.Replay ();
      _queryProvider.Execute<IEnumerable<Student>> (expression);
      _executorMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Could not parse expression 'TestQueryable<Student>().Select(s => s)': This "
        + "overload of the method 'System.Linq.Queryable.Select' is currently not supported, but you can register your own parser if needed.")]
    public void Execute_WithSpecificRegistry ()
    {
      Expression expression = ExpressionHelper.MakeExpression (() => from s in ExpressionHelper.CreateStudentQueryable () select s);
      _executorMock
          .Expect (mock => mock.ExecuteCollection<Student> (Arg<QueryModel>.Is.Anything, Arg<IEnumerable<FetchRequestBase>>.Is.Anything))
          .Return (new Student[0]);
      _executorMock.Replay ();
      _queryProviderWithSpecificRegistry.Execute<IEnumerable<Student>> (expression);
      _executorMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetFetchRequests_None ()
    {
      var originalExpression = ExpressionHelper.CreateExpression ();
      var expression = originalExpression;

      var requests = _queryProvider.GetFetchRequests (ref expression);

      Assert.That (requests, Is.Empty);
      Assert.That (expression, Is.SameAs (originalExpression));
    }

    [Test]
    public void GetFetchRequests_TopLevel ()
    {
      var relatedObjectSelector1 = ExpressionHelper.CreateLambdaExpression<Student, IEnumerable<Student>> (s => s.Friends);
      var relatedObjectSelector2 = ExpressionHelper.CreateLambdaExpression<Student, IEnumerable<int>> (s => s.Scores);
      var innerExpression = ExpressionHelper.CreateExpression ();
      var originalExpression = new FetchManyExpression (new FetchManyExpression (innerExpression, relatedObjectSelector1), relatedObjectSelector2);
      Expression expression = originalExpression;

      var requests = _queryProvider.GetFetchRequests (ref expression);

      Assert.That (requests.Length, Is.EqualTo (2));
      Assert.That (requests.Select (fr => fr.RelationMember).ToArray (), Is.EquivalentTo (new[] { _friendsMember, _scoresMember }));
      Assert.That (expression, Is.Not.SameAs (originalExpression));
      Assert.That (expression, Is.SameAs (innerExpression));
    }

    [Test]
    public void GetFetchRequests_ThenFetch ()
    {
      var relatedObjectSelector1 = ExpressionHelper.CreateLambdaExpression<Student, IEnumerable<Student>> (s => s.Friends);
      var relatedObjectSelector2 = ExpressionHelper.CreateLambdaExpression<Student, IEnumerable<int>> (s => s.Scores);
      var innerExpression = ExpressionHelper.CreateExpression ();
      var originalExpression = new ThenFetchManyExpression (new FetchManyExpression (innerExpression, relatedObjectSelector1), relatedObjectSelector2);
      Expression expression = originalExpression;

      var requests = _queryProvider.GetFetchRequests (ref expression);

      Assert.That (requests.Length, Is.EqualTo (1));
      Assert.That (requests.Single ().RelationMember, Is.SameAs (_friendsMember));
      Assert.That (requests.Single ().InnerFetchRequests.Count (), Is.EqualTo (1));
      Assert.That (requests.Single ().InnerFetchRequests.Single ().RelationMember, Is.SameAs (_scoresMember));
      Assert.That (expression, Is.Not.SameAs (originalExpression));
      Assert.That (expression, Is.SameAs (innerExpression));
    }

    [Test]
    public void Execute_WithFetchRequests ()
    {
      Expression<Func<Student, IEnumerable<Student>>> relatedObjectSelector = s => s.Friends;
      IQueryable<Student> query = SelectTestQueryGenerator.CreateSimpleQuery (_queryableWithExecutorMock).FetchMany (relatedObjectSelector);

      _executorMock.Expect (mock => mock.ExecuteCollection<Student> (
          Arg<QueryModel>.Is.Anything,
          Arg<FetchRequestBase[]>.Matches (frs => frs.Single().RelationMember == _friendsMember)))
          .Return (new Student[0]);

      _executorMock.Replay ();
      _queryProvider.Execute<IEnumerable<Student>> (query.Expression);
      _executorMock.VerifyAllExpectations ();
    }
  }
}
