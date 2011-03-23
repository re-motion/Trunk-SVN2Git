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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.RealObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class SyncStateTest : ClientTransactionBaseTest
  {
    [Test]
    public void CollectionItems_Synchronized_WithUnload ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var endPointID = RelationEndPointID.Create (orderItem, oi => oi.Order);
      var endPoint = (RealObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[endPointID];
      Assert.That (endPoint, Is.Not.Null);

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));

      orderItem.Order.OrderItems.EnsureDataComplete();

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, orderItem.Order.OrderItems.AssociatedEndPointID);

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void CollectionItems_Unsynchronized_WithUnload ()
    {
      SetDatabaseModifyable();

      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderItems.EnsureDataComplete();

      var orderItemID = DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<OrderItem, Order> (order.ID, (oi, o) => oi.Order = o);
      var orderItem = OrderItem.GetObject (orderItemID);
      var endPointID = RelationEndPointID.Create (orderItem, oi => oi.Order);
      var endPoint = (RealObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[endPointID];
      Assert.That (endPoint, Is.Not.Null);

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (UnsynchronizedRealObjectEndPointSyncState)));

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, orderItem.Order.OrderItems.AssociatedEndPointID);

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));

      order.OrderItems.EnsureDataComplete();

      Assert.That (ObjectEndPointTestHelper.GetSyncState (endPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
    }
  }
}