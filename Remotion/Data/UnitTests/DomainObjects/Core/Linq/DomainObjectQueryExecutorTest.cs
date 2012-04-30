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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryExecutorTest : StandardMappingTest
  {
    private ClassDefinition _orderClassDefinition;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private IDomainObjectQueryGenerator _queryGeneratorMock;
    private DomainObjectQueryExecutor _queryExecutor;

    private IQueryManager _queryManagerMock;
    private ClientTransactionScope _transactionScope;

    private QueryModel _someQueryModel;
    private IQuery _someQueryStub;
    private Order _someOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();
      _queryGeneratorMock = MockRepository.GenerateStrictMock<IDomainObjectQueryGenerator>();
      _queryExecutor = new DomainObjectQueryExecutor (_orderClassDefinition, _storageTypeInformationProviderStub, _queryGeneratorMock);

      _queryManagerMock = MockRepository.GenerateStrictMock<IQueryManager> ();
      var transaction = ClientTransactionObjectMother.CreateTransactionWithQueryManager<ClientTransaction> (_queryManagerMock);
      _transactionScope = transaction.EnterDiscardingScope ();

      _someQueryModel = QueryModelObjectMother.Create();
      _someQueryStub = MockRepository.GenerateStub<IQuery>();
      _someOrder = DomainObjectMother.CreateFakeObject<Order>();
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown ();
    }

    [Test]
    public void ExecuteScalar ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder>(), QueryType.Scalar))
          .Return (_someQueryStub);
      
      var fakeResult = 7;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations();
      _queryManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void ExecuteScalar_WithFetchRequests ()
    {
      ExpectCreateQueryWithFetchQueryModelBuilders (_someQueryModel, _someQueryStub);

      var fakeResult = 7;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void ExecuteScalar_WithDBNull ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar))
          .Return (_someQueryStub);

      var fakeResult = DBNull.Value;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int?> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void ExecuteScalar_WithConvertibleResult ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar))
          .Return (_someQueryStub);

      var fakeResult = 0;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<bool> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void ExecuteScalar_WithNonNullResult_NullableType ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar))
          .Return (_someQueryStub);

      var fakeResult = 0;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int?> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void ExecuteScalar_WithNullResult_NullableType ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar))
          .Return (_someQueryStub);

      object fakeResult = null;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteScalar<int?> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteScalar_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteScalar<int> (_someQueryModel);
      }
    }

    [Test]
    public void ExecuteCollection ()
    {
      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection))
          .Return (_someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[] { _someOrder });
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteCollection<Order> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _someOrder }));
    }

    [Test]
    public void ExecuteCollection_WithFetchRequests ()
    {
      ExpectCreateQueryWithFetchQueryModelBuilders (_someQueryModel, _someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[] { _someOrder });
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteCollection<Order> (_someQueryModel);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _someOrder }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteCollection_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteCollection<Order> (_someQueryModel);
      }
    }

    [Test]
    public void ExecuteSingle_WithNativelySupportedType ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.IsTypeSupported (typeof (Order))).Return (true);

      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Scalar))
          .Return (_someQueryStub);

      var fakeResult = _someOrder;
      _queryManagerMock
          .Expect (mock => mock.GetScalar (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_someOrder));
    }

    [Test]
    public void ExecuteSingle_WithNonNativelySupportedType ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.IsTypeSupported (typeof (Order))).Return (false);

      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection))
          .Return (_someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[] { _someOrder });
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);

      _queryGeneratorMock.VerifyAllExpectations ();
      _queryManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_someOrder));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void ExecuteSingle_WithNonNativelySupportedType_MoreThanOneItem ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.IsTypeSupported (typeof (Order))).Return (false);

      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection))
          .Return (_someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[] { _someOrder, DomainObjectMother.CreateFakeObject<Order>() });
      _queryManagerMock
          .Stub (stub => stub.GetCollection (_someQueryStub))
          .Return (fakeResult);

      _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
    }

    [Test]
    public void ExecuteSingle_WithNonNativelySupportedType_ZeroItems_ReturnDefaultTrue ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.IsTypeSupported (typeof (Order))).Return (false);

      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection))
          .Return (_someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[0]);
      _queryManagerMock
          .Stub (stub => stub.GetCollection (_someQueryStub))
          .Return (fakeResult);

      var result = _queryExecutor.ExecuteSingle<Order> (_someQueryModel, true);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteSingle_WithNonNativelySupportedType_ZeroItems_ReturnDefaultFalse ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.IsTypeSupported (typeof (Order))).Return (false);

      _queryGeneratorMock
          .Expect (mock => mock.CreateQuery (
              "<dynamic query>", _orderClassDefinition, _someQueryModel, Enumerable.Empty<FetchQueryModelBuilder> (), QueryType.Collection))
          .Return (_someQueryStub);

      var fakeResult = new QueryResult<DomainObject> (_someQueryStub, new DomainObject[0]);
      _queryManagerMock
          .Stub (stub => stub.GetCollection (_someQueryStub))
          .Return (fakeResult);

      _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void ExecuteSingle_NoActiveClientTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _queryExecutor.ExecuteSingle<Order> (_someQueryModel, false);
      }
    }

    private void CheckFetchQueryModelBuilder (
    FetchQueryModelBuilder builder, FetchRequestBase expectedFetchRequest, QueryModel expectedQueryModel, int expectedResultOperatorPosition)
    {
      Assert.That (builder.FetchRequest, Is.SameAs (expectedFetchRequest));
      Assert.That (builder.SourceItemQueryModel, Is.SameAs (expectedQueryModel));
      Assert.That (builder.ResultOperatorPosition, Is.EqualTo (expectedResultOperatorPosition));
    }

    private FetchRequestBase AddFetchRequest ()
    {
      var relationMember = MemberInfoFromExpressionUtility.GetProperty ((Order o) => o.OrderTicket);
      var fetchRequest = new FetchOneRequest (relationMember);
      _someQueryModel.ResultOperators.Add (fetchRequest);
      return fetchRequest;
    }

    private ResultOperatorBase AddSomeResultOperator ()
    {
      var someResultOperator = new DistinctResultOperator ();
      _someQueryModel.ResultOperators.Add (someResultOperator);
      return someResultOperator;
    }

    private void ExpectCreateQueryWithFetchQueryModelBuilders (QueryModel expectedQueryModel, IQuery fakeResult)
    {
      var nonTrailingFetchRequest = AddFetchRequest();
      var someResultOperator = AddSomeResultOperator ();
      var trailingFetchRequest1 = AddFetchRequest ();
      var trailingFetchRequest2 = AddFetchRequest ();
      Assert.That (
          expectedQueryModel.ResultOperators,
          Is.EqualTo (new[] { nonTrailingFetchRequest, someResultOperator, trailingFetchRequest1, trailingFetchRequest2 }));

      _queryGeneratorMock
          .Expect (
              mock => mock.CreateQuery (
                  Arg<string>.Is.Anything,
                  Arg<ClassDefinition>.Is.Anything,
                  Arg.Is (expectedQueryModel),
                  Arg<IEnumerable<FetchQueryModelBuilder>>.Is.Anything,
                  Arg<QueryType>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi =>
          {
            Assert.That (expectedQueryModel.ResultOperators, Is.EqualTo (new[] { nonTrailingFetchRequest, someResultOperator }));

            var builders = ((IEnumerable<FetchQueryModelBuilder>) mi.Arguments[3]).ToArray ();
            Assert.That (builders, Has.Length.EqualTo (2));
            CheckFetchQueryModelBuilder (builders[0], trailingFetchRequest2, expectedQueryModel, 3);
            CheckFetchQueryModelBuilder (builders[1], trailingFetchRequest1, expectedQueryModel, 2);
          });
    }
  }
}