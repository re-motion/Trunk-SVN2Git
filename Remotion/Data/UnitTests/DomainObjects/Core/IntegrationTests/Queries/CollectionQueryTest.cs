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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Queries
{
  [TestFixture]
  public class CollectionQueryTest : QueryTestBase
  {
    [Test]
    public void GetCollectionWithExistingObjects ()
    {
      var computer2 = Computer.GetObject (DomainObjectIDs.Computer2);
      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      IQueryManager queryManager = QueryManager;
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Computer].* FROM [Computer] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", DomainObjectIDs.Computer2); // preloaded
      query.Parameters.Add ("@2", DomainObjectIDs.Computer3);
      query.Parameters.Add ("@3", DomainObjectIDs.Computer1); // preloaded

      var resultArray = queryManager.GetCollection (query).ToArray ();
      Assert.That (resultArray, Is.EqualTo (new[] { computer2, Computer.GetObject (DomainObjectIDs.Computer3), computer1 }));
    }

    [Test]
    public void GetCollectionWithNullValues ()
    {
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add ("@3", DomainObjectIDs.Computer4); // no employee
      query.Parameters.Add ("@2", DomainObjectIDs.Computer1); // employee 3

      var result = QueryManager.GetCollection (query);
      Assert.That (result.ContainsNulls (), Is.True);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { null, null, Employee.GetObject (DomainObjectIDs.Employee3) }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "A database query returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed.")]
    public void GetCollectionWithDuplicates ()
    {
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Order].* FROM [OrderItem] INNER JOIN [Order] ON [OrderItem].[OrderID] = [Order].[ID] WHERE [Order].[OrderNo] = 1",
          new QueryParameterCollection (), typeof (DomainObjectCollection));
      QueryManager.GetCollection (query);
    }

    [Test]
    public void CollectionQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      var customers = QueryManager.GetCollection (query);

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

      var customers = QueryManager.GetCollection<Customer> (query).ToObjectList ();
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

      var orders = QueryManager.GetCollection<Order> (query).ToCustomCollection ();
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

      QueryManager.GetCollection<Order> (query);
    }

    [Test]
    public void CollectionQuery_WithObjectList_WorksWhenUnassignableCollectionType ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType");

      var result = QueryManager.GetCollection<Order> (query);
      Assert.That (result.Count, Is.GreaterThan (0));
    }


    [Test]
    public void GetStoredProcedureResult ()
    {
      var orders = (OrderCollection) QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection ();

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
      var orders = (OrderCollection) QueryManager.GetCollection (query).ToCustomCollection ();

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orders[1].ID, "OrderWithoutOrderItem");
    }

    [Test]
    public void QueryingEnlists ()
    {
      Order.GetObject (DomainObjectIDs.Order1); // ensure Order1 already exists in transaction

      var orders = (OrderCollection) QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery")).ToCustomCollection ();
      Assert.AreEqual (2, orders.Count, "Order count");

      foreach (Order order in orders)
        Assert.IsTrue (TestableClientTransaction.IsEnlisted (order));

      int orderNumberSum = orders.Sum (order => order.OrderNumber);

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
          order1.ID.StorageProviderDefinition,
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
    public void CollectionQuery_AllowsInvalidObjects ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      TestableClientTransaction.DataManager.Commit ();

      var query = QueryFactory.CreateCollectionQuery (
          "test",
          order1.ID.StorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo=1",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));

      var result = ClientTransaction.Current.QueryManager.GetCollection (query);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { order1 }));
    }

    [Test]
    public void CollectionQuery_CallsFilterQueryResult_AndAllowsGetObjectDuringFiltering ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock
          .Expect (mock => mock.FilterQueryResult (Arg.Is (TestableClientTransaction), Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (TestQueryFactory.CreateTestQueryResult<DomainObject> ())
          .WhenCalled (mi => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      TestableClientTransaction.AddListener (listenerMock);

      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("customerID", DomainObjectIDs.Customer1);
      TestableClientTransaction.QueryManager.GetCollection (query);

      listenerMock.VerifyAllExpectations ();
      listenerMock.BackToRecord (); // For Discarding
    }

    [Test]
    public void QueryWithExtensibleEnums ()
    {
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.ClassWithAllDataTypes1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [TableWithAllDataTypes].* FROM [TableWithAllDataTypes] WHERE ([TableWithAllDataTypes].[ExtensibleEnum] = @1)",
          new QueryParameterCollection (), typeof (DomainObjectCollection));

      query.Parameters.Add ("@1", Color.Values.Blue ());

      var result = QueryManager.GetCollection (query);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) }));
    }

  }
}