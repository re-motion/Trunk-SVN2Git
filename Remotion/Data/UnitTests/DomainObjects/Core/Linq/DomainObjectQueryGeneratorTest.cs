// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class DomainObjectQueryGeneratorTest : StandardMappingTest
  {
    private ISqlQueryGenerator _sqlQueryGeneratorMock;
    private TypeConversionProvider _typeConversionProvider;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;

    private DomainObjectQueryGenerator _generator;

    private ClassDefinition _customerClassDefinition;
    private QueryModel _customerQueryModel;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlQueryGeneratorMock = MockRepository.GenerateStrictMock<ISqlQueryGenerator> ();
      _typeConversionProvider = TypeConversionProvider.Create();
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();

      _generator = new DomainObjectQueryGenerator (_sqlQueryGeneratorMock, _typeConversionProvider, _storageTypeInformationProviderStub);

      _customerClassDefinition = GetTypeDefinition (typeof (Customer));
      _customerQueryModel = QueryModelObjectMother.Create (Expression.Constant (null, typeof (Customer)));
    }

    [Test]
    public void CreateScalarQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (Expression.Parameter (typeof (IDatabaseResultRow), "row"), "SELECT x");
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateScalarQuery<int> ("id", TestDomainStorageProviderDefinition, _customerQueryModel);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (ScalarQueryAdapter<int>)));
      Assert.That (result.ID, Is.EqualTo ("id"));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (TestDomainStorageProviderDefinition));
      Assert.That (result.Statement, Is.EqualTo ("SELECT x"));
      Assert.That (result.Parameters, Is.Empty);
      Assert.That (result.QueryType, Is.EqualTo (QueryType.Scalar));
      Assert.That (result.EagerFetchQueries, Is.Empty);
    }

    [Test]
    public void CreateScalarQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (Expression.Parameter (typeof (IDatabaseResultRow), "row"), parameters: new[] { new CommandParameter ("p0", "paramval") });

      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateScalarQuery<int> ("id", TestDomainStorageProviderDefinition, _customerQueryModel);

      Assert.That (result.Parameters, Is.EqualTo (new[] { new QueryParameter ("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateSequenceQuery_EntityQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult ("SELECT x");
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var id = "id";
      var fetchQueryModelBuilders = Enumerable.Empty<FetchQueryModelBuilder> ();
      var result = _generator.CreateSequenceQuery<int> (id, _customerClassDefinition, _customerQueryModel, fetchQueryModelBuilders);

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf (typeof (DomainObjectSequenceQueryAdapter<int>)));
      Assert.That (result.ID, Is.EqualTo (id));
      Assert.That (result.StorageProviderDefinition, Is.EqualTo (_customerClassDefinition.StorageEntityDefinition.StorageProviderDefinition));
      Assert.That (result.Statement, Is.EqualTo ("SELECT x"));
      Assert.That (result.Parameters, Is.Empty);
      Assert.That (result.QueryType, Is.EqualTo (QueryType.Collection));
      Assert.That (result.EagerFetchQueries, Is.Empty);
    }

    [Test]
    public void CreateSequenceQuery_EntityQueryWithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (parameters: new[] { new CommandParameter ("p0", "paramval") });

      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ());

      Assert.That (result.Parameters, Is.EqualTo (new[] { new QueryParameter ("p0", "paramval", QueryParameterType.Value) }));
    }


    [Test]
    public void CreateSequenceQuery_NoEntityQuery_NoParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (Expression.Parameter (typeof (IDatabaseResultRow), "row"), "SELECT x", null,SqlQueryGeneratorResult.QueryKind.Other);
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var id = "id";
      var fetchQueryModelBuilders = Enumerable.Empty<FetchQueryModelBuilder> ();
      var result = _generator.CreateSequenceQuery<int> (id, _customerClassDefinition, _customerQueryModel, fetchQueryModelBuilders);

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf (typeof (CustomSequenceQueryAdapter<int>)));
      Assert.That (result.ID, Is.EqualTo (id));
      Assert.That (result.StorageProviderDefinition, Is.EqualTo (_customerClassDefinition.StorageEntityDefinition.StorageProviderDefinition));
      Assert.That (result.Statement, Is.EqualTo ("SELECT x"));
      Assert.That (result.Parameters, Is.Empty);
      Assert.That (result.QueryType, Is.EqualTo (QueryType.Custom));
      Assert.That (result.EagerFetchQueries, Is.Empty);
    }

    [Test]
    public void CreateSequenceQuery_NoEntityQuery_WithParameters ()
    {
      var fakeSqlQueryResult = CreateSqlQueryGeneratorResult (Expression.Parameter (typeof (IDatabaseResultRow), "row"), parameters: new[] { new CommandParameter ("p0", "paramval") }, queryKind: SqlQueryGeneratorResult.QueryKind.Other);

      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQueryResult);

      var result = _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, Enumerable.Empty<FetchQueryModelBuilder> ());

      Assert.That (result.Parameters, Is.EqualTo (new[] { new QueryParameter ("p0", "paramval", QueryParameterType.Value) }));
    }

    [Test]
    public void CreateSequenceQuery_WithFetchRequests ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder ((Customer o) => o.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult ("FETCH");

      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel ();
            CheckActualFetchQueryModel (actualQueryModel, fetchQueryModel);
          });

      var result = _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
      CheckSingleFetchRequest (result.EagerFetchQueries, typeof (Company), "Ceo", "FETCH");
    }

    [Test]
    public void CreateSequenceQuery_WithFetchRequestWithSortExpression ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchManyQueryModelBuilder ((Customer o) => o.Orders);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult ("FETCH");

      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel ();

            Assert.That (actualQueryModel.MainFromClause.FromExpression, Is.TypeOf<SubQueryExpression> ());
            CheckActualFetchQueryModel (((SubQueryExpression) actualQueryModel.MainFromClause.FromExpression).QueryModel, fetchQueryModel);

            Assert.That (actualQueryModel.BodyClauses, Has.Some.TypeOf<OrderByClause> ());
            var orderByClause = (OrderByClause) actualQueryModel.BodyClauses.Single ();
            var endPointDefinition = ((VirtualRelationEndPointDefinition) GetEndPointDefinition (typeof (Customer), "Orders"));
            Assert.That (endPointDefinition.SortExpressionText, Is.EqualTo ("OrderNumber asc"));
            var orderNumberMember = MemberInfoFromExpressionUtility.GetProperty ((Order o) => o.OrderNumber);
            Assert.That (((MemberExpression) orderByClause.Orderings[0].Expression).Member, Is.SameAs (orderNumberMember));
            Assert.That (orderByClause.Orderings[0].OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
          });

      _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSequenceQuery_WithNestedFetchRequests ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder ((Customer c) => c.Ceo);
      var fakeFetchSqlQueryResult = CreateSqlQueryGeneratorResult ("FETCH");
      _sqlQueryGeneratorMock.Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything)).Return (fakeFetchSqlQueryResult).Repeat.Once ();

      var innerFetchRequest = CreateFetchOneRequest ((Ceo c) => c.Company);
      fetchQueryModelBuilder.FetchRequest.GetOrAddInnerFetchRequest (innerFetchRequest);

      var fakeInnerFetchSqlQueryResult = CreateSqlQueryGeneratorResult ("INNER FETCH");
      _sqlQueryGeneratorMock
          .Expect (mock => mock.CreateSqlQuery (Arg<QueryModel>.Is.Anything))
          .Return (fakeInnerFetchSqlQueryResult)
          .WhenCalled (mi =>
          {
            var actualQueryModel = (QueryModel) mi.Arguments[0];
            Assert.That (((StreamedSequenceInfo) actualQueryModel.GetOutputDataInfo ()).ItemExpression.Type, Is.SameAs (typeof (Company)));
          });

      var result = _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });

      _sqlQueryGeneratorMock.VerifyAllExpectations ();

      var fetchQuery = result.EagerFetchQueries.Single ();
      CheckSingleFetchRequest (fetchQuery.Value.EagerFetchQueries, typeof (Ceo), "Company", "INNER FETCH");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The member 'CtorCalled' is a 'Field', which cannot be fetched by this LINQ provider. Only properties can be fetched.")]
    public void CreateSequenceQuery_WithInvalidFetchRequest_MemberIsNoPropertyInfo ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder ((Customer o) => o.CtorCalled);

      _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Name' is not a relation end point. "
        + "Fetching it is not supported by this LINQ provider.")]
    public void CreateSequenceQuery_WithInvalidFetchRequest_MemberIsNoRelationProperty ()
    {
      var fakeSqlQuery = CreateSqlQueryGeneratorResult ();
      _sqlQueryGeneratorMock.Stub (stub => stub.CreateSqlQuery (_customerQueryModel)).Return (fakeSqlQuery);

      var fetchQueryModelBuilder = CreateFetchOneQueryModelBuilder ((Customer o) => o.Name);

      _generator.CreateSequenceQuery<int> ("id", _customerClassDefinition, _customerQueryModel, new[] { fetchQueryModelBuilder });
    }

    private SqlQueryGeneratorResult CreateSqlQueryGeneratorResult (
        string commandText = null, 
        CommandParameter[] parameters = null,
        SqlQueryGeneratorResult.QueryKind queryKind = SqlQueryGeneratorResult.QueryKind.EntityQuery)
    {
      return new SqlQueryGeneratorResult (CreateSqlCommandData (Expression.Parameter (typeof (Order), "o"), commandText, parameters), queryKind);
    }

    private SqlQueryGeneratorResult CreateSqlQueryGeneratorResult (
      ParameterExpression inMemoryProjectionParameter,  
      string commandText = null,
        CommandParameter[] parameters = null,
        SqlQueryGeneratorResult.QueryKind queryKind = SqlQueryGeneratorResult.QueryKind.EntityQuery)
    {
      return new SqlQueryGeneratorResult (CreateSqlCommandData (inMemoryProjectionParameter, commandText, parameters), queryKind);
    }

    private SqlCommandData CreateSqlCommandData (ParameterExpression inMemoryProjectionParameter, string commandText = null, CommandParameter[] parameters = null)
    {
      return new SqlCommandData (
          commandText ?? "bla", parameters ?? new CommandParameter[0], inMemoryProjectionParameter, Expression.Constant (null));
    }

    private void CheckActualFetchQueryModel (QueryModel actualQueryModel, QueryModel fetchQueryModel)
    {
      Assert.That (actualQueryModel, Is.Not.SameAs (fetchQueryModel));
      Assert.That (fetchQueryModel.ResultOperators, Has.No.TypeOf<DistinctResultOperator> ());
      Assert.That (actualQueryModel.ResultOperators, Has.Some.TypeOf<DistinctResultOperator> ());
      Assert.That (actualQueryModel.MainFromClause.ToString (), Is.EqualTo (fetchQueryModel.MainFromClause.ToString ()));
      Assert.That (actualQueryModel.SelectClause.ToString (), Is.EqualTo (fetchQueryModel.SelectClause.ToString ()));
    }

    private FetchQueryModelBuilder CreateFetchOneQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var fetchRequest = CreateFetchOneRequest(memberExpression);
      return new FetchQueryModelBuilder (fetchRequest, _customerQueryModel, 0);
    }

    private FetchOneRequest CreateFetchOneRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = MemberInfoFromExpressionUtility.GetMember (memberExpression);
      return new FetchOneRequest (relationMember);
    }

    private FetchQueryModelBuilder CreateFetchManyQueryModelBuilder<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var fetchRequest = CreateFetchManyRequest (memberExpression);
      return new FetchQueryModelBuilder (fetchRequest, _customerQueryModel, 0);
    }

    private FetchManyRequest CreateFetchManyRequest<TSource, TDest> (Expression<Func<TSource, TDest>> memberExpression)
    {
      var relationMember = MemberInfoFromExpressionUtility.GetProperty (memberExpression);
      return new FetchManyRequest (relationMember);
    }

    private void CheckSingleFetchRequest (EagerFetchQueryCollection fetchQueryCollection, Type sourceType, string fetchedProperty, string expectedFetchQueryText)
    {
      Assert.That (fetchQueryCollection.Count, Is.EqualTo (1));
      var fetchQuery = fetchQueryCollection.Single ();
      Assert.That (fetchQuery.Key, Is.EqualTo (GetEndPointDefinition (sourceType, fetchedProperty)));
      Assert.That (fetchQuery.Value.Statement, Is.EqualTo (expectedFetchQueryText));
    }

  }
}