// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
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
      var ordersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo IN (1, 3)",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE o.OrderNo IN (1, 3)",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      var id1 = RelationEndPointID.Create (DomainObjectIDs.Order1, relationEndPointDefinition);
      var id2 = RelationEndPointID.Create (DomainObjectIDs.Order2, relationEndPointDefinition);

      Assert.That (ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Null);
      Assert.That (ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Null);

      var result = ClientTransactionMock.QueryManager.GetCollection (ordersQuery);
      Assert.That (result.ToArray (), Is.EquivalentTo (new[] { Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2) }));

      Assert.That (ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Not.Null);

      Assert.That (((ICollectionEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id1)).Collection,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
      Assert.That (((ICollectionEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (id2)).Collection,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem3) }));
    }

    [Test]
    public void EagerFetching_WithExistingRelationData ()
    {
      // Load - and change - relation data prior to executing the query
      var order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (
          order.OrderItems,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));

      var ordersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo = 1",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      // This will yield different items (none) than the actual relation query above - simulating the database has changed in between
      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE 1 = 0",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      // This executes the fetch query, but should discard the result (since the relation data already exists)
      ClientTransactionMock.QueryManager.GetCollection (ordersQuery);

      Assert.That (
          order.OrderItems,
          Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
    }
  }
}