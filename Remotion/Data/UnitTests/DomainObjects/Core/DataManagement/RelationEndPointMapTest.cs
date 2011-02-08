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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp();

      _map = (RelationEndPointMap) ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    public void Contains_False ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      Assert.That (_map.Contains (endPointID), Is.False);
    }

    [Test]
    public void Contains_True ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      _map.RegisterRealObjectEndPoint (endPointID, CreateNewDataContainer (endPointID));

      Assert.That (_map.Contains (endPointID), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObjects (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      DomainObjectCollection originalOrderItems = _map.GetOriginalRelatedObjects (endPointID);
      DomainObjectCollection orderItems = _map.GetRelatedObjects (endPointID);

      Assert.That (ReferenceEquals (originalOrderItems, orderItems), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObject (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.That (ReferenceEquals (originalOrderTicket, orderTicket), Is.True);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (orderTicket, Is.SameAs (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1)));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetRelatedObjectIncludeDeletedFalse ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _map.GetRelatedObject (endPointID, false);
    }

    [Test]
    public void GetRelatedObjectIncludeDeletedTrue ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.That (client, Is.Not.Null);
      Assert.That (client.ID, Is.EqualTo (DomainObjectIDs.Client1));
      Assert.That (client.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObjects (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetRelatedObjectWithInvalid ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Client newClient = Client.NewObject();
      location.Client = newClient;
      location.Client.Delete();

      var endPointID = new RelationEndPointID (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.That (client, Is.Not.Null);
      Assert.That (client, Is.SameAs (newClient));
      Assert.That (client.IsInvalid, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObject (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"), false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "GetRelationEndPointWithLazyLoad cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void GetRelationEndPointWithLazyLoad_DoesNotSupportAnonymousEndPoints ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client) + ".ParentClient");
      IRelationEndPoint unidirectionalEndPoint = _map.GetRelationEndPointWithLazyLoad (new RelationEndPointID (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition();
      _map.GetRelationEndPointWithLazyLoad (new RelationEndPointID (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersCollectionEndPoint ()
    {
      _map.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderItemsEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_map[orderItemsEndPointID], Is.Null);

      var endPoint = _map.GetRelationEndPointWithLazyLoad (orderItemsEndPointID);
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (_map[orderItemsEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPoint ()
    {
      _map.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (_map[orderTicketEndPointID], Is.Null);

      var endPoint = _map.GetRelationEndPointWithLazyLoad (orderTicketEndPointID);
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (_map[orderTicketEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPointWithNull ()
    {
      _map.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Employee1); // preload Employee before lazily loading its virtual end point

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (_map[endPointID], Is.Null);

      var endPoint = _map.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (endPoint));
      Assert.That (((ObjectEndPoint) endPoint).OppositeObjectID, Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_LoadsData_OfObjectsWithRealEndPointNotYetRegistered ()
    {
      var locationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      Assert.That (locationEndPointID.Definition.IsVirtual, Is.False);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Location1], Is.Null);

      var result = _map.GetRelationEndPointWithLazyLoad (locationEndPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Location1], Is.Not.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_DoesNotLoadData_OfObjectsWithVirtualEndPointNotYetRegistered_IfNotNeeded ()
    {
      OrderTicket.GetObject (DomainObjectIDs.OrderTicket1); // ensure opposite real end point is available
      Assert.That (_map[RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order")], Is.Not.Null);
      Assert.That (_map[RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order")].Definition.IsVirtual, Is.False);
      
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (endPointID.Definition.IsVirtual, Is.True);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      var result = _map.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_LoadsData_OfObjectsWithVirtualEndPointNotYetRegistered_IfNeeded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (endPointID.Definition.IsVirtual, Is.True);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      var result = _map.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
    }

    [Test]
    public void RegisterVirtualObjectEndPoint_CreatesVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var objectEndPoint = _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);

      Assert.That (objectEndPoint.ID, Is.EqualTo (id));
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterVirtualObjectEndPoint_RegistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var objectEndPoint = _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void UnregisterVirtualObjectEndPoint_RemovesEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);
      Assert.That (_map[id], Is.Not.Null);
      
      _map.UnregisterVirtualObjectEndPoint (id);

      Assert.That (_map[id], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end-point is not part of this map.\r\nParameter name: endPointID")]
    public void UnregisterVirtualObjectEndPoint_ThrowsWhenNotRegistered ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _map.UnregisterVirtualObjectEndPoint (id);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot remove end-point "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' because it has "
        + "changed. End-points can only be unregistered when they are unchanged.")]
    public void UnregisterVirtualObjectEndPoint_ThrowsWhenChanged ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var objectEndPoint = _map.RegisterVirtualObjectEndPoint (id, DomainObjectIDs.OrderTicket1);
      Assert.That (_map[id], Is.Not.Null);

      objectEndPoint.OppositeObjectID = null;
      Assert.That (objectEndPoint.HasChanged, Is.True);

      try
      {
        _map.UnregisterVirtualObjectEndPoint (id);
      }
      finally
      {
        Assert.That (_map[id], Is.SameAs (objectEndPoint));
      }
    }

    [Test]
    public void RegisterRealObjectEndPoint_CreatesRealObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (foreignKeyDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"]));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersOppositeVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var expectedOppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeEndPoint = (VirtualObjectEndPoint) _map[expectedOppositeEndPointID];
      Assert.That (oppositeEndPoint, Is.Not.Null);
      Assert.That (oppositeEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualCollectionEndPoint_CollectionAlreadyRegistered ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPoint = _map.RegisterCollectionEndPoint (collectionEndPointID, new DomainObject[0]);

      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var itemReference = LifetimeService.GetObjectReference (_map.ClientTransaction, DomainObjectIDs.OrderItem1);
      Assert.That (itemReference.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (collectionEndPoint.OppositeDomainObjects, List.Contains (itemReference));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualCollectionEndPoint_CollectionNotRegisteredYet ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var itemReference = ClientTransactionMock.GetEnlistedDomainObject (DomainObjectIDs.OrderItem1);
      Assert.That (itemReference, Is.Not.Null);
      Assert.That (itemReference.State, Is.EqualTo (StateType.NotLoadedYet));

      var collectionEndPoint = (CollectionEndPoint) _map[collectionEndPointID];
      Assert.That (collectionEndPoint.IsDataAvailable, Is.False);
      var dataKeeper = RelationEndPointTestHelper.GetCollectionEndPointDataKeeper (collectionEndPoint);
      Assert.That (dataKeeper.CollectionData.ToArray(), List.Contains (itemReference));
    }

    [Test]
    public void RegisterRealObjectEndPoint_WithOppositeAnonymousEndPoint()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Client1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnregistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      Assert.That (_map[id], Is.Not.Null);
      
      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[id], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnregistersOppositeVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);
      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (_map[oppositeEndPointID], Is.Not.Null);

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[oppositeEndPointID], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnloadsOppositeVirtualCollectionEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);
      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var itemReference = LifetimeService.GetObjectReference (_map.ClientTransaction, DomainObjectIDs.OrderItem1);
      
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var oppositeCollectionEndPoint = (CollectionEndPoint)  _map[oppositeEndPointID];
      Assert.That (oppositeCollectionEndPoint, Is.Not.Null);
      oppositeCollectionEndPoint.MarkDataAvailable ();
      Assert.That (oppositeCollectionEndPoint.IsDataAvailable, Is.True);
      Assert.That (oppositeCollectionEndPoint.OppositeDomainObjects, List.Contains (itemReference));
      Assert.That (itemReference.State, Is.EqualTo (StateType.NotLoadedYet));

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[oppositeEndPointID], Is.SameAs (oppositeCollectionEndPoint));
      Assert.That (oppositeCollectionEndPoint.IsDataAvailable, Is.False);

      var dataKeeper = RelationEndPointTestHelper.GetCollectionEndPointDataKeeper (oppositeCollectionEndPoint);
      Assert.That (dataKeeper.CollectionData.ToArray(), List.Not.Contains (itemReference));
      Assert.That (itemReference.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnregisterRealObjectEndPoint_OppositeVirtualCollectionEndPointNotLoaded ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _map.UnregisterCollectionEndPoint (oppositeEndPointID); // remove the CollectionEndPoint that was added by RegisterRealObjectEndPoint
      Assert.That (_map[oppositeEndPointID], Is.Null);

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[oppositeEndPointID], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_OppositeAnonymousEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Client1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      var oppositeEndPointID = new RelationEndPointID (DomainObjectIDs.Client1, id.Definition.GetOppositeEndPointDefinition ());
      Assert.That (_map[oppositeEndPointID], Is.Null);

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[oppositeEndPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end-point is not part of this map.\r\nParameter name: endPointID")]
    public void UnregisterRealObjectEndPoint_ThrowsWhenNotRegistered ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      _map.UnregisterRealObjectEndPoint (id);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot remove end-point "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' because "
        + "it has changed. End-points can only be unregistered when they are unchanged.")]
    public void UnregisterRealObjectEndPoint_ThrowsWhenChanged ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      Assert.That (_map[id], Is.Not.Null);

      Assert.That (objectEndPoint.OppositeObjectID, Is.Not.Null);
      objectEndPoint.OppositeObjectID = null;
      Assert.That (objectEndPoint.HasChanged, Is.True);

      try
      {
        _map.UnregisterRealObjectEndPoint (id);
      }
      finally
      {
        Assert.That (_map[id], Is.SameAs (objectEndPoint));
      }
    }

    [Test]
    public void RegisterCollectionEndPoint_RegistersEndPoint_NullInitialContents ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, null);

      Assert.That (_map[endPointID], Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (endPoint));

      Assert.That (endPoint.IsDataAvailable, Is.False);
    }

    [Test]
    public void RegisterCollectionEndPoint_RegistersEndPoint_NonNullInitialContents ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order> ();
      
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new[] { item });

      Assert.That (_map[endPointID], Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (endPoint));

      Assert.That (endPoint.IsDataAvailable, Is.True);
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { item }));
    }

    [Test]
    public void RegisterCollectionEndPoint_UsesChangeDetectionStrategy ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      Assert.That (endPoint.ChangeDetectionStrategy, Is.SameAs (_map.CollectionEndPointChangeDetectionStrategy));
    }

    [Test]
    public void UnregisterCollectionObjectEndPoint_UnregistersEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterCollectionEndPoint (endPointID);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end-point is not part of this map.\r\nParameter name: endPointID")]
    public void UnregisterCollectionObjectEndPoint_ThrowsWhenNotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _map.UnregisterCollectionEndPoint (endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot remove end-point "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' because "
        + "it has changed. End-points can only be unregistered when they are unchanged.")]
    public void UnregisterCollectionEndPoint_ThrowsWhenChanged ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var item = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionEndPoint = _map.RegisterCollectionEndPoint (endPointID, new[] { item });
      Assert.That (_map[endPointID], Is.Not.Null);

      collectionEndPoint.CreateRemoveCommand (item).Perform(); // no bidirectional changes, no events - it's only a fake item
      Assert.That (collectionEndPoint.HasChanged, Is.True);

      try
      {
        _map.UnregisterCollectionEndPoint (endPointID);
      }
      finally
      {
        Assert.That (_map[endPointID], Is.SameAs (collectionEndPoint));
      }
    }

    [Test]
    public void MarkCollectionEndPointComplete_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_map[endPointID], Is.Null);

      _map.MarkCollectionEndPointComplete (endPointID);

      var collectionEndPoint = (CollectionEndPoint) _map[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);
      Assert.That (collectionEndPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    public void MarkCollectionEndPointComplete_EndPointRegistered_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      var item = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionEndPoint = _map.RegisterCollectionEndPoint (endPointID, new[] { item });
      collectionEndPoint.Unload ();
      Assert.That (_map[endPointID], Is.SameAs (collectionEndPoint));
      Assert.That (collectionEndPoint.IsDataAvailable, Is.False);

      _map.MarkCollectionEndPointComplete (endPointID);

      Assert.That (_map[endPointID], Is.SameAs (collectionEndPoint));
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);
    }

    [Test]
    public void MarkCollectionEndPointComplete_EndPointRegistered_AlreadyComplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      var item = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionEndPoint = _map.RegisterCollectionEndPoint (endPointID, new[] { item });
      Assert.That (_map[endPointID], Is.SameAs (collectionEndPoint));
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);

      _map.MarkCollectionEndPointComplete (endPointID);

      Assert.That (_map[endPointID], Is.SameAs (collectionEndPoint));
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);
      Assert.That (collectionEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { item }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "MarkCollectionEndPointComplete can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void MarkCollectionEndPointComplete_ChecksCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      _map.MarkCollectionEndPointComplete (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "MarkCollectionEndPointComplete cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void MarkCollectionEndPointComplete_ChecksAnonymity ()
    {
      var endPointID = new RelationEndPointID (DomainObjectIDs.Order1, new AnonymousRelationEndPointDefinition (DomainObjectIDs.Customer1.ClassDefinition));
      _map.MarkCollectionEndPointComplete (endPointID);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);
      var foreignKeyProperty = dataContainer.PropertyValues[endPointID.Definition.PropertyName];

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var endPoint = (RealObjectEndPoint) _map[endPointID];

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.ForeignKeyProperty, Is.SameAs (foreignKeyProperty));
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersOppositeVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order2, "OrderTicket");
      var oppositeEndPoint = (VirtualObjectEndPoint) _map[oppositeID];

      Assert.That (oppositeEndPoint, Is.Not.Null);
      Assert.That (oppositeEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoOppositeNullObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, null);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeEndPointDefinition = endPointID.Definition.GetOppositeEndPointDefinition();
      var expectedID = new RelationEndPointID (null, oppositeEndPointDefinition);

      Assert.That (_map[expectedID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = CreateExistingDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = CreateExistingDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (VirtualObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (RealObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.Not.Null);
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (dataContainer.PropertyValues[typeof (OrderTicket) + ".Order"]));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var collectionEndPoint = (CollectionEndPoint) _map[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_Existing_UnregistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_Existing_UnregistersOppositeVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order2, "OrderTicket");
      Assert.That (_map[oppositeID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[oppositeID], Is.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_Existing_UnregistersNoCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = CreateExistingDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      _map.RegisterCollectionEndPoint (endPointID, null);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Not.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_Existing_UnregistersNoOriginalNonNullVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = CreateExistingDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      var virtualEndPoint = _map.RegisterVirtualObjectEndPoint (endPointID, DomainObjectIDs.OrderTicket1);
      virtualEndPoint.OppositeObjectID = null; // the current value is ignored
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Not.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_Existing_UnregistersOriginalNullVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = CreateExistingDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      _map.RegisterVirtualObjectEndPoint (endPointID, null);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_New_UnregistersVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_New_UnregistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void UnregisterEndPointsForDataContainer_New_UnregistersCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.UnregisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot unregister the following relation end-points: "
        + "'RealObjectEndPoint: "
        + "OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'. "
        + "Relation end-points can only be removed when they are unchanged.")]
    public void UnregisterEndPointsForDataContainer_WithUnregisterableEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (ObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      objectEndPoint.OppositeObjectID = DomainObjectIDs.Order1;
      Assert.That (objectEndPoint.HasChanged, Is.True);

      _map.UnregisterEndPointsForDataContainer (dataContainer);
    }

    [Test]
    public void GetNonUnregisterableEndPointsForDataContainer()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (ObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      objectEndPoint.OppositeObjectID = DomainObjectIDs.Order1;
      Assert.That (objectEndPoint.HasChanged, Is.True);

      var result = _map.GetNonUnregisterableEndPointsForDataContainer (dataContainer);

      Assert.That (result, Is.EqualTo (new[] { objectEndPoint }));
    }

    [Test]
    public void GetNonUnregisterableEndPointsForDataContainer_OppositeCollectionEndPoint ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (realEndPointID, DomainObjectIDs.Order1);
      var realEndPoint = (ObjectEndPoint) _map.RegisterRealObjectEndPoint (realEndPointID, dataContainer);

      var oppositeCollectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var oppositeCollectionEndPoint = (CollectionEndPoint) _map[oppositeCollectionEndPointID];
      oppositeCollectionEndPoint.MarkDataAvailable (); // mark the current state as the "full" state of the collection

      var item2 = DomainObjectMother.GetObjectReference<OrderItem> (ClientTransactionMock, DomainObjectIDs.OrderItem2);

      oppositeCollectionEndPoint.CreateAddCommand (item2).Perform ();
      Assert.That (realEndPoint.HasChanged, Is.False);
      Assert.That (oppositeCollectionEndPoint.HasChanged, Is.True);

      var result = _map.GetNonUnregisterableEndPointsForDataContainer (dataContainer);

      Assert.That (result, Is.EqualTo (new[] { realEndPoint })); // realEndPoint has not changed, but its opposite end-point has
    }

    [Test]
    public void GetNonUnregisterableEndPointsForDataContainer_None ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var result = _map.GetNonUnregisterableEndPointsForDataContainer (dataContainer);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetNonUnregisterableEndPointsForDataContainer_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateNewDataContainer (endPointID);
      Assert.That (_map[endPointID], Is.Null);

      var result = _map.GetNonUnregisterableEndPointsForDataContainer (dataContainer);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void CheckForConflictingForeignKeys_NoConflicts ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order1);
      
      _map.CheckForConflictingForeignKeys (dataContainer);
    }

    [Test]
    public void CheckForConflictingForeignKeys_NoConflicts_WithVirtualEndPoints_AndOppositeCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      // use DataContainer with State New because that owns virtual end-points
      var dataContainer = CreateNewDataContainer (endPointID);

      _map.CheckForConflictingForeignKeys (dataContainer);
    }

    [Test]
    public void CheckForConflictingForeignKeys_NoConflicts_Nulls ()
    {
      var endPointID1 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer1 = CreateExistingForeignKeyDataContainer (endPointID1, null);
      _map.RegisterEndPointsForDataContainer (dataContainer1);

      var endPointID2 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket2, "Order");
      var dataContainer2 = CreateExistingForeignKeyDataContainer (endPointID2, null);

      _map.CheckForConflictingForeignKeys (dataContainer2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The data of object 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' conflicts with existing data: It has a foreign key "
        + "property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' which points to object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. However, that object has previously been determined to point back to object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. These two pieces of information contradict each other.")]
    public void CheckForConflictingForeignKeys_Conflict ()
    {
      var endPointID1 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer1 = CreateExistingForeignKeyDataContainer (endPointID1, DomainObjectIDs.Order1);
      _map.RegisterEndPointsForDataContainer (dataContainer1);

      var endPointID2 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket2, "Order");
      var dataContainer2 = CreateExistingForeignKeyDataContainer (endPointID2, DomainObjectIDs.Order1);

      _map.CheckForConflictingForeignKeys (dataContainer2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The data of object 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' conflicts with existing data: It has a foreign key "
        + "property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' which points to object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. However, that object has previously been determined to point back to object "
        + "'<null>'. These two pieces of information contradict each other.")]
    public void CheckForConflictingForeignKeys_Conflict_WithNull ()
    {
      var endPointID1 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _map.RegisterVirtualObjectEndPoint (endPointID1, null);

      var endPointID2 = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket2, "Order");
      var dataContainer2 = CreateExistingForeignKeyDataContainer (endPointID2, DomainObjectIDs.Order1);

      _map.CheckForConflictingForeignKeys (dataContainer2);
    }

    [Test]
    public void CommitAllEndPoints_CommitsEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      var addedObject = Order.NewObject();
      endPoint.OppositeDomainObjects.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.CommitAllEndPoints();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { addedObject }));
    }

    [Test]
    public void RollbackAllEndPoints_RollsBackEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);

      var addedObject = Order.NewObject();
      endPoint.OppositeDomainObjects.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.RollbackAllEndPoints();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    public void RemoveEndPoint_RemovesEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.RemoveEndPoint (endPointID);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RemoveEndPoint_RaisesNotification_BeforeRemoving ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      Assert.That (_map[endPointID], Is.Not.Null);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      listenerMock.Expect (mock => mock.RelationEndPointMapUnregistering (ClientTransactionMock, endPointID))
          .WhenCalled (mi => Assert.That (_map[endPointID], Is.Not.Null));
      ClientTransactionMock.AddListener (listenerMock);

      listenerMock.Replay();

      _map.RemoveEndPoint (endPointID);

      listenerMock.VerifyAllExpectations();
      listenerMock.BackToRecord (); // For Discard
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' is "
        + "not part of this map.\r\nParameter name: endPointID")]
    public void RemoveEndPoint_NonExistingEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RemoveEndPoint (endPointID);
    }

    private DataContainer CreateNewDataContainer (RelationEndPointID id)
    {
      var foreignKeyDataContainer = DataContainer.CreateNew (id.ObjectID);
      return foreignKeyDataContainer;
    }

    private DataContainer CreateExistingForeignKeyDataContainer (RelationEndPointID id, ObjectID initialForeignKeyValue)
    {
      var foreignKeyDataContainer = DataContainer.CreateForExisting (
          id.ObjectID, 
          null, 
          pd => pd.PropertyName == id.Definition.PropertyName ? initialForeignKeyValue : pd.DefaultValue);
      return foreignKeyDataContainer;
    }

    private DataContainer CreateExistingDataContainer (RelationEndPointID id)
    {
      var foreignKeyDataContainer = DataContainer.CreateForExisting (
          id.ObjectID,
          null,
          pd => pd.DefaultValue);
      return foreignKeyDataContainer;
    }
  }
}