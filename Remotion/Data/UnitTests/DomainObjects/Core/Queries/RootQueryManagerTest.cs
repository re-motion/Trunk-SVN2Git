// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class RootQueryManagerTest : ClientTransactionBaseTest
  {
    private RootQueryManager _queryManager;
    private string _unitTestStorageProviderStubID;
    private UnitTestStorageProviderStubDefinition _unitTestStorageProviderStubDefinition;
    private DataContainer _official1DataContainer;
    private DataContainer _official2DataContainer;
    private IQuery _officialTestQuery;
    private IQuery _officialFetchTestQuery;
    private IQuery _officialFetchTestQuery2;
    private IRelationEndPointDefinition _officialOrdersRelationEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _queryManager = new RootQueryManager (ClientTransactionMock);

      _unitTestStorageProviderStubID = DomainObjectIDs.Official1.StorageProviderID;
      _unitTestStorageProviderStubDefinition = (UnitTestStorageProviderStubDefinition)
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (_unitTestStorageProviderStubID);

      _official1DataContainer = DataContainer.CreateNew (DomainObjectIDs.Official1);
      _official2DataContainer = DataContainer.CreateNew (DomainObjectIDs.Official2);

      _officialTestQuery = QueryFactory.CreateCollectionQuery ("test query", _unitTestStorageProviderStubID, "TEST QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _officialFetchTestQuery = QueryFactory.CreateCollectionQuery ("fetch query", _unitTestStorageProviderStubID, "FETCH QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _officialFetchTestQuery2 = QueryFactory.CreateCollectionQuery ("fetch query", _unitTestStorageProviderStubID, "FETCH QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _officialOrdersRelationEndPointDefinition = DomainObjectIDs.Official1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Official).FullName + ".Orders");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (ClientTransactionMock, _queryManager.ClientTransaction);
    }

    [Test]
    public void GetScalarWithoutParameter ()
    {
      Assert.AreEqual (42, _queryManager.GetScalar (QueryFactory.CreateQueryFromConfiguration ("QueryWithoutParameter")));
    }

    [Test]
    public void GetCollection ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      var customers = _queryManager.GetCollection (query);

      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers.ToArray ()[0].ID);
      Assert.AreEqual (typeof (Customer), customers.ToArray ()[0].GetPublicDomainObjectType ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetCollectionWithScalarQuery ()
    {
      _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery"));
    }

    [Test]
    public void GetCollectionWithObjectList ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      var customers = _queryManager.GetCollection<Customer> (query).ToObjectList();
      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers[0].ID);
      Assert.IsTrue (query.CollectionType.IsAssignableFrom (customers.GetType ()));
    }

    [Test]
    public void GetCollectionWithObjectListWorksWhenAssignableCollectionType ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderByOfficialQuery");
      query.Parameters.Add ("@officialID", DomainObjectIDs.Official1);

      var orders = _queryManager.GetCollection<Order> (query).ToCustomCollection();
      Assert.AreEqual (5, orders.Count);
      Assert.That (orders, Is.EquivalentTo (new object[]
      {
        Order.GetObject (DomainObjectIDs.Order1),
        Order.GetObject (DomainObjectIDs.Order2),
        Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem),
        Order.GetObject (DomainObjectIDs.Order3),
        Order.GetObject (DomainObjectIDs.Order4),
      }));
      Assert.IsTrue (query.CollectionType.IsAssignableFrom (orders.GetType ()));
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "The query returned an object of type " 
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer', but a query result of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' was expected.")]
    public void GetCollectionWithObjectListThrowsWhenInvalidT ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      _queryManager.GetCollection<Order> (query);
    }

    [Test]
    public void GetCollectionWithObjectList_WorksWhenUnassignableCollectionType ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType");

      var result = _queryManager.GetCollection<Order> (query);
      Assert.That (result.Count, Is.GreaterThan (0));
    }

    [Test]
    public void GetCollection_ClosesConnectionBeforeCallingFilterQueryResult ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (TestQueryFactory.CreateTestQueryResult<DomainObject> ())
          .Do (mi => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      ClientTransactionMock.AddListener (listenerMock);

      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("customerID", DomainObjectIDs.Customer1);
      ClientTransactionMock.QueryManager.GetCollection (query);

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetScalarWithCollectionQuery ()
    {
      _queryManager.GetScalar (QueryFactory.CreateQueryFromConfiguration ("OrderQuery"));
    }

    [Test]
    public void GetStoredProcedureResult ()
    {
      var orders = (OrderCollection) _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection();

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.Order2, orders[1].ID, "Order2");
    }

    [Test]
    public void GetStoredProcedureResultWithParameter ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQueryWithParameter");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);
      var orders = (OrderCollection) _queryManager.GetCollection (query).ToCustomCollection();

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orders[1].ID, "OrderWithoutOrderItem");
    }

    [Test]
    public void QueryingEnlists ()
    {
      Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in transaction

      var orders = (OrderCollection) _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection();
      Assert.AreEqual (2, orders.Count, "Order count");

      foreach (Order order in orders)
        Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

      int orderNumberSum = 0;
      foreach (Order order in orders)
        orderNumberSum += order.OrderNumber;

      Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1).OrderNumber + Order.GetObject (DomainObjectIDs.Order2).OrderNumber, orderNumberSum);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void QueriedObjectsMightNotBeEnlistableInOtherTransaction ()
    {
      var newTransaction = new ClientTransactionMock ();
      using (newTransaction.EnterDiscardingScope ())
      {
        Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in newTransaction
      }

      var orders = (OrderCollection) _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection();
      Assert.AreEqual (2, orders.Count, "Order count");

      using (newTransaction.EnterDiscardingScope ())
      {
        foreach (Order order in orders)
        {
          if (!order.TransactionContext[newTransaction].CanBeUsedInTransaction)
            newTransaction.EnlistDomainObject (order);  // this throws because there is already _another_ instance of Order1 enlisted
        }
      }
    }

    [Test]
    public void GetCollection_ExecutesFetchQueries ()
    {
      _officialTestQuery.EagerFetchQueries.Add (_officialOrdersRelationEndPointDefinition, _officialFetchTestQuery);

      var mainResult = new[] { _official1DataContainer, _official2DataContainer };
      var fetchResult = new DataContainer[0];

      var storageProviderMock = MockRepository.GenerateMock<StorageProvider> (_unitTestStorageProviderStubDefinition);
      storageProviderMock.Expect (mock => mock.ExecuteCollectionQuery (_officialTestQuery)).Return (mainResult);
      storageProviderMock.Expect (mock => mock.ExecuteCollectionQuery (_officialFetchTestQuery)).Return (fetchResult);

      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (storageProviderMock))
      {
        _queryManager.GetCollection<Official> (_officialTestQuery);
      }

      storageProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetCollection_DoesNotExecuteFetchQueries_WhenQueryResultEmpty ()
    {
      _officialTestQuery.EagerFetchQueries.Add (_officialOrdersRelationEndPointDefinition, _officialFetchTestQuery);

      var mainResult = new DataContainer[0];

      var storageProviderMock = MockRepository.GenerateMock<StorageProvider> (_unitTestStorageProviderStubDefinition);
      storageProviderMock.Expect (mock => mock.ExecuteCollectionQuery (_officialTestQuery)).Return (mainResult);

      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (storageProviderMock))
      {
        _queryManager.GetCollection<Official> (_officialTestQuery);
      }

      storageProviderMock.AssertWasNotCalled (mock => mock.ExecuteCollectionQuery (_officialFetchTestQuery));
    }

    [Test]
    public void GetCollection_FetchQueries_Recursive ()
    {
      _officialTestQuery.EagerFetchQueries.Add (_officialOrdersRelationEndPointDefinition, _officialFetchTestQuery);
      _officialFetchTestQuery.EagerFetchQueries.Add (_officialOrdersRelationEndPointDefinition, _officialFetchTestQuery2);

      var mainResult = new[] {DataContainer.CreateNew (DomainObjectIDs.Official1)};

      var storageProviderMock = MockRepository.GenerateMock<StorageProvider> (_unitTestStorageProviderStubDefinition);
      storageProviderMock.Expect (mock => mock.ExecuteCollectionQuery (_officialTestQuery)).Return (mainResult);
      storageProviderMock.Expect (mock => mock.ExecuteCollectionQuery (_officialFetchTestQuery)).Return (new DataContainer[0]);

      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (storageProviderMock))
      {
        _queryManager.GetCollection<Official> (_officialTestQuery);
      }

      storageProviderMock.VerifyAllExpectations();
    }
  }
}
