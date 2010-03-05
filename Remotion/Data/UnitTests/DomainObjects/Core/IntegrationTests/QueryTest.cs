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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.DataManagement;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    private IQueryManager _queryManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _queryManager = ClientTransactionMock.QueryManager;
    }

    [Test]
    public void GetCollectionWithExistingObjects ()
    {
      var computer2 = Computer.GetObject (DomainObjectIDs.Computer2);
      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      IQueryManager queryManager = _queryManager;
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Computer].* FROM [Computer] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", DomainObjectIDs.Computer2); // preloaded
      query.Parameters.Add ("@2", DomainObjectIDs.Computer3);
      query.Parameters.Add ("@3", DomainObjectIDs.Computer1); // preloaded

      var resultArray = queryManager.GetCollection (query).ToArray();
      Assert.That (resultArray, Is.EqualTo (new[] {computer2, Computer.GetObject (DomainObjectIDs.Computer3), computer1}));
    }

    [Test]
    public void GetCollectionWithNullValues ()
    {
      IQueryManager queryManager = _queryManager;
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(), typeof (DomainObjectCollection));
      
      query.Parameters.Add ("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add ("@3", DomainObjectIDs.Computer4); // no employee
      query.Parameters.Add ("@2", DomainObjectIDs.Computer1); // employee 3
      
      var result = queryManager.GetCollection (query);
      Assert.That (result.ContainsNulls (), Is.True);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { null, null, Employee.GetObject (DomainObjectIDs.Employee3)}));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not supported.")]
    public void GetCollectionWithDuplicates ()
    {
      IQueryManager queryManager = _queryManager;
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Order].* FROM [OrderItem] INNER JOIN [Order] ON [OrderItem].[OrderID] = [Order].[ID] WHERE [Order].[OrderNo] = 1",
          new QueryParameterCollection (), typeof (DomainObjectCollection));
      queryManager.GetCollection (query);
    }

    [Test]
    public void EagerFetching ()
    {
      var ordersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderID,
          "SELECT * FROM [Order] WHERE OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition =
          DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          DomainObjectIDs.OrderItem1.StorageProviderID,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE o.OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      var id1 = new RelationEndPointID (DomainObjectIDs.Order1, relationEndPointDefinition);
      var id2 = new RelationEndPointID (DomainObjectIDs.Order2, relationEndPointDefinition);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Null);

      var result = ClientTransactionMock.QueryManager.GetCollection (ordersQuery);
      Assert.That (result.ToArray(), Is.EquivalentTo (new[] {Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2)} ));

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Not.Null);

      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id1]).OppositeDomainObjects,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id2]).OppositeDomainObjects,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem3) }));
    }

    [Test]
    public void QueryWithExtensibleEnums ()
    {
      IQueryManager queryManager = _queryManager;
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.ClassWithAllDataTypes1.ClassDefinition.StorageProviderID,
          "SELECT [TableWithAllDataTypes].* FROM [TableWithAllDataTypes] WHERE ([TableWithAllDataTypes].[ExtensibleEnum] = @1)",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", Color.Values.Blue());

      var result = queryManager.GetCollection (query);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) }));
    }

    [Test]
    public void QueryWithExtensibleEnums_Linq ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                  where cwadt.ExtensibleEnumProperty == Color.Values.Red()
                  select cwadt;

      var result = query.ToArray();
      Assert.That (result, Is.EqualTo (new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1) }));
    }

    [Test]
    public void ScalarQueryWithoutParameter ()
    {
      Assert.AreEqual (42, _queryManager.GetScalar (QueryFactory.CreateQueryFromConfiguration ("QueryWithoutParameter")));
    }

    [Test]
    public void CollectionQuery ()
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
    public void CollectionQuery_WithObjectList ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      var customers = _queryManager.GetCollection<Customer> (query).ToObjectList ();
      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers[0].ID);
      Assert.IsTrue (query.CollectionType.IsAssignableFrom (customers.GetType ()));
    }

    [Test]
    public void CollectionQuery_WithObjectList_WorksWhenAssignableCollectionType ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderByOfficialQuery");
      query.Parameters.Add ("@officialID", DomainObjectIDs.Official1);

      var orders = _queryManager.GetCollection<Order> (query).ToCustomCollection ();
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
    public void CollectionQuery_WithObjectList_ThrowsWhenInvalidT ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      _queryManager.GetCollection<Order> (query);
    }

    [Test]
    public void CollectionQuery_WithObjectList_WorksWhenUnassignableCollectionType ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType");

      var result = _queryManager.GetCollection<Order> (query);
      Assert.That (result.Count, Is.GreaterThan (0));
    }

    [Test]
    public void GetStoredProcedureResult ()
    {
      var orders = (OrderCollection) _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection ();

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
      var orders = (OrderCollection) _queryManager.GetCollection (query).ToCustomCollection ();

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orders[1].ID, "OrderWithoutOrderItem");
    }

    [Test]
    public void QueryingEnlists ()
    {
      Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in transaction

      var orders = (OrderCollection) _queryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection ();
      Assert.AreEqual (2, orders.Count, "Order count");

      foreach (Order order in orders)
        Assert.IsTrue (ClientTransactionMock.IsEnlisted (order));

      int orderNumberSum = 0;
      foreach (Order order in orders)
        orderNumberSum += order.OrderNumber;

      Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1).OrderNumber + Order.GetObject (DomainObjectIDs.Order2).OrderNumber, orderNumberSum);
    }

    [Test]
    public void CollectionQuery_ReturnsDeletedObjects ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete (); // mark as deleted
      var order2 = Order.GetObject (DomainObjectIDs.Order2);

      var query = QueryFactory.CreateCollectionQuery (
          "test",
          order1.ID.StorageProviderID,
          "SELECT * FROM [Order] WHERE OrderNo=1 OR OrderNo=3 ORDER BY OrderNo ASC",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      var result = ClientTransaction.Current.QueryManager.GetCollection (query);

      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result.ToArray (), Is.EqualTo (new[] { order1, order2 }));
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));
      Assert.That (order2.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void CollectionQuery_ThrowsOnDiscardedObjects ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      ClientTransactionMock.DataManager.Discard (order1.InternalDataContainer);

      var query = QueryFactory.CreateCollectionQuery (
          "test",
          order1.ID.StorageProviderID,
          "SELECT * FROM [Order] WHERE OrderNo=1 OR OrderNo=3 ORDER BY OrderNo ASC",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      ClientTransaction.Current.QueryManager.GetCollection (query);
    }

    [Test]
    public void CollectionQuery_CallsFilterQueryResult_AndAllowsGetObjectDuringFiltering ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (TestQueryFactory.CreateTestQueryResult<DomainObject> ())
          .WhenCalled (mi => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      ClientTransactionMock.AddListener (listenerMock);

      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("customerID", DomainObjectIDs.Customer1);
      ClientTransactionMock.QueryManager.GetCollection (query);

      listenerMock.VerifyAllExpectations ();
      listenerMock.BackToRecord(); // For Discarding
    }
  }
}
