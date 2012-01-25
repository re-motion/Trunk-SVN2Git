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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.EagerFetching
{
  [TestFixture]
  public class EagerFetchingTest : ClientTransactionBaseTest
  {
    [Test]
    public void EagerFetching ()
    {
      var ordersQuery = CreateOrdersQuery ("OrderNo IN (1, 3)");
      AddOrderItemsFetchQuery  (ordersQuery, "o.OrderNo IN (1, 3)");

      var id1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var id2 = RelationEndPointID.Create (DomainObjectIDs.Order2, typeof (Order), "OrderItems");

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Null);

      var result = TestableClientTransaction.QueryManager.GetCollection (ordersQuery);
      Assert.That (result.ToArray (), Is.EquivalentTo (new[] { Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2) }));

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Not.Null);

      Assert.That (((ICollectionEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1)).Collection,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
      Assert.That (((ICollectionEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2)).Collection,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem3) }));
    }

    [Test]
    public void EagerFetching_WithExistingRelationData ()
    {
      // Load relation data prior to executing the query.
      var order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (
          order.OrderItems,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));

      var ordersQuery = CreateOrdersQuery ("OrderNo = 1");
      // This will return a different relation collection (an empty one).
      AddOrderItemsFetchQuery (ordersQuery, "1 = 0");

      // This executes the fetch query, but should discard the result (since the relation data already exists).
      TestableClientTransaction.QueryManager.GetCollection (ordersQuery);

      Assert.That (
          order.OrderItems,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
    }

    [Test]
    public void EagerFetching_UsesForeignKeyDataFromDatabase_NotTransaction ()
    {
      // This will retrieve Order1.
      var ordersQuery = CreateOrdersQuery ("OrderNo = 1");
      // This will fetch OrderItem1 and OrderItem2, both pointing to Order1.
      AddOrderItemsFetchQuery (ordersQuery, "o.OrderNo = 1");

      // Fake OrderItem2 to point to Order2 in memory.
      var orderItem2 = RegisterFakeOrderItem (DomainObjectIDs.OrderItem2, DomainObjectIDs.Order2);

      var result = TestableClientTransaction.QueryManager.GetCollection (ordersQuery);

      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (result.ToArray(), Is.EquivalentTo (new[] { order1 }));

      var orderItemsEndPointID = RelationEndPointID.Resolve (order1, o => o.OrderItems);
      var orderItemsEndPoint = TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (orderItemsEndPointID);
      Assert.That (orderItemsEndPoint, Is.Not.Null);
      Assert.That (orderItemsEndPoint.IsDataComplete, Is.True);

      // The relation contains the fetched result, disregarding the in-memory data. This makes it an unsynchronized relation.
      Assert.That (
          order1.OrderItems, 
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), orderItem2 }));

      Assert.That (orderItem2.Order, Is.Not.SameAs (order1));
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (TestableClientTransaction, orderItemsEndPointID), Is.False);
    }

    private OrderItem RegisterFakeOrderItem (ObjectID objectID, ObjectID fakeOrderID)
    {
      var orderItem = (OrderItem) LifetimeService.GetObjectReference (TestableClientTransaction, objectID);
      var fakeDataContainer = DataContainer.CreateForExisting (
          orderItem.ID, 
          null, 
          pd => pd.PropertyName.EndsWith ("Order") ? fakeOrderID : pd.DefaultValue);
      fakeDataContainer.SetDomainObject (orderItem);
      ClientTransactionTestHelper.RegisterDataContainer (TestableClientTransaction, fakeDataContainer);
      return orderItem;
    }

    private IQuery CreateOrdersQuery (string whereCondition)
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Order] WHERE " + whereCondition,
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }

    private void AddOrderItemsFetchQuery (IQuery ordersQuery, string whereCondition)
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE " + whereCondition,
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);
    }

  }
}