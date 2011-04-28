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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RealObjectEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

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
    public void CreateNullEndPoint_RealObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.TypeDefinitions[typeof (OrderTicket)].GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");

      var nullObjectEndPoint = RelationEndPointMap.CreateNullEndPoint (ClientTransactionMock, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_VirtualObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.TypeDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      var nullObjectEndPoint = RelationEndPointMap.CreateNullEndPoint (ClientTransactionMock, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_CollectionEndPoint ()
    {
      var orderItemsDefinition = 
          Configuration.TypeDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      var nullObjectEndPoint = RelationEndPointMap.CreateNullEndPoint (ClientTransactionMock, orderItemsDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullCollectionEndPoint)));
      var collectionEndPointID = RelationEndPointID.Create (null, orderItemsDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (collectionEndPointID));
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
      _map.RegisterRealObjectEndPoint (endPointID, RelationEndPointTestHelper.CreateNewDataContainer (endPointID));

      Assert.That (_map.Contains (endPointID), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObjects (RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
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
      _map.GetOriginalRelatedObject (RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.That (ReferenceEquals (originalOrderTicket, orderTicket), Is.True);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
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

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _map.GetRelatedObject (endPointID, false);
    }

    [Test]
    public void GetRelatedObjectIncludeDeletedTrue ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete();

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
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
      _map.GetRelatedObjects (RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetRelatedObjectWithInvalid ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Client newClient = Client.NewObject();
      location.Client = newClient;
      location.Client.Delete();

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
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
      _map.GetRelatedObject (RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"), false);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.TypeDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _map.GetRelationEndPointWithLazyLoad (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullCollectionEndPoint ()
    {
      var endPointDefinition = 
          Configuration.TypeDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _map.GetRelationEndPointWithLazyLoad (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullCollectionEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetRelationEndPointWithLazyLoad cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void GetRelationEndPointWithLazyLoad_DoesNotSupportAnonymousEndPoints ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client) + ".ParentClient");
      IRelationEndPoint unidirectionalEndPoint =
          _map.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition();
      _map.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersCollectionEndPoint ()
    {
      _map.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderItemsEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_map[orderItemsEndPointID], Is.Null);

      var endPoint = _map.GetRelationEndPointWithLazyLoad (orderItemsEndPointID);
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.IsDataComplete, Is.True);
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
      Assert.That (endPoint.IsDataComplete, Is.True);
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
    public void GetRelationEndPointWithMinimumLoading_EndPointAlreadyAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = CallRegisterVirtualObjectEndPoint (_map, endPointID);

      Assert.That (_map[endPointID], Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (endPoint));
      Assert.That (endPoint.IsDataComplete, Is.False);

      var result = _map.GetRelationEndPointWithMinimumLoading (endPointID);
      
      Assert.That (result, Is.SameAs (endPoint));
      Assert.That (_map[endPointID], Is.SameAs (endPoint));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointNotAvailable_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      Assert.That (_map[endPointID], Is.Null);

      var result = _map.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.False);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointNotAvailable_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[endPointID.ObjectID], Is.Null);
      Assert.That (_map[endPointID], Is.Null);

      var result = _map.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[endPointID.ObjectID], Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.True);
    }

    [Test]
    public void RegisterVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var objectEndPoint = CallRegisterVirtualObjectEndPoint (_map, id);

      Assert.That (objectEndPoint.ID, Is.EqualTo (id));
      Assert.That (objectEndPoint.IsDataComplete, Is.False);
      Assert.That (objectEndPoint.EndPointProvider, Is.SameAs (_map.EndPointProvider));
      Assert.That (objectEndPoint.DataKeeperFactory, Is.SameAs (_map.VirtualObjectEndPointDataKeeperFactory));

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void RegisterRealObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (foreignKeyDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"]));
      Assert.That (objectEndPoint.EndPointProvider, Is.SameAs (_map.EndPointProvider));

      Assert.That (_map[id], Is.SameAs (objectEndPoint));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersOppositeVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var expectedOppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeEndPoint = (VirtualObjectEndPoint) _map[expectedOppositeEndPointID];
      Assert.That (oppositeEndPoint, Is.Not.Null);
      Assert.That (oppositeEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));

      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_WithNullValue ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, null);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualObjectEndPoint_VirtualEndPointAlreadyRegistered_NotComplete_RootTransaction_MarkedComplete ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      var domainObjectReference = LifetimeService.GetObjectReference (ClientTransactionMock, id.ObjectID);

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      virtualObjectEndPointMock.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      virtualObjectEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      virtualObjectEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Matches (endPoint => endPoint.ID == id)));
      virtualObjectEndPointMock.Expect (mock => mock.MarkDataComplete (domainObjectReference));
      virtualObjectEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, virtualObjectEndPointMock);

      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      virtualObjectEndPointMock.VerifyAllExpectations ();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualObjectEndPoint_VirtualEndPointAlreadyRegistered_NotComplete_SubTransaction_LeftIncomplete ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction();
      var subTransactionMap = (RelationEndPointMap) ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap;
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      virtualObjectEndPointMock.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      virtualObjectEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      virtualObjectEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Matches (endPoint => endPoint.ID == id)));
      virtualObjectEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (subTransactionMap, virtualObjectEndPointMock);

      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = subTransactionMap.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      virtualObjectEndPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (Arg<DomainObject>.Is.Anything));
      virtualObjectEndPointMock.VerifyAllExpectations ();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualObjectEndPoint_VirtualEndPointAlreadyRegistered_AlreadyComplete ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      virtualObjectEndPointMock.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      virtualObjectEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      virtualObjectEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Matches (endPoint => endPoint.ID == id)));
      virtualObjectEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, virtualObjectEndPointMock);

      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      virtualObjectEndPointMock.VerifyAllExpectations ();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualObjectEndPoint_VirtualObjectNotRegisteredYet ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var virtualObjectEndPoint = (VirtualObjectEndPoint) _map[virtualObjectEndPointID];
      Assert.That (virtualObjectEndPoint.IsDataComplete, Is.True);
      Assert.That (
          ((CompleteVirtualObjectEndPointLoadState) VirtualObjectEndPointTestHelper.GetLoadState (virtualObjectEndPoint)).DataKeeper.
              CurrentOppositeEndPoint,
          Is.SameAs (realObjectEndPoint));

      Assert.That (
          RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint),
          Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)),
          "Because real end-point was registered with virtual end-point.");
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualCollectionEndPoint_CollectionAlreadyRegistered ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      collectionEndPointMock.Stub (stub => stub.ID).Return (collectionEndPointID);
      collectionEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Matches (endPoint => endPoint.ID == id)));
      collectionEndPointMock.Replay();

      RelationEndPointMapTestHelper.AddEndPoint (_map, collectionEndPointMock);

      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      collectionEndPointMock.VerifyAllExpectations();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void RegisterRealObjectEndPoint_RegistersReferenceWithOppositeVirtualCollectionEndPoint_CollectionNotRegisteredYet ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      var collectionEndPoint = (CollectionEndPoint) _map[collectionEndPointID];
      Assert.That (collectionEndPoint.IsDataComplete, Is.False);

      Assert.That (
          ((IncompleteCollectionEndPointLoadState) CollectionEndPointTestHelper.GetLoadState (collectionEndPoint)).OriginalOppositeEndPoints,
          Has.Member (realObjectEndPoint));

      Assert.That (
          RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint),
          Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)),
          "Because collection's state is incomplete.");
    }

    [Test]
    public void RegisterRealObjectEndPoint_WithOppositeAnonymousEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Client1);

      var realObjectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);

      Assert.That (RealObjectEndPointTestHelper.GetSyncState (realObjectEndPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnregistersEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      Assert.That (_map[id], Is.Not.Null);

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[id], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnregistersFromOppositeVirtualObjectEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);
      var realEndPoint = new RealObjectEndPoint (
          ClientTransactionMock, id, foreignKeyDataContainer, ClientTransactionMock.DataManager, ClientTransactionMock.DataManager);
      RelationEndPointMapTestHelper.AddEndPoint (_map, realEndPoint);

      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      oppositeEndPointMock.Stub (stub => stub.CanBeCollected).Return (false);
      oppositeEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (realEndPoint));
      oppositeEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointMock);
      
      _map.UnregisterRealObjectEndPoint (id);

      oppositeEndPointMock.VerifyAllExpectations();
      Assert.That (_map[oppositeEndPointID], Is.Not.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_UnregistersFromOppositeVirtualCollectionEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);
      var realEndPoint = new RealObjectEndPoint (
          ClientTransactionMock, id, foreignKeyDataContainer, ClientTransactionMock.DataManager, ClientTransactionMock.DataManager);
      RelationEndPointMapTestHelper.AddEndPoint (_map, realEndPoint);

      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      oppositeEndPointMock.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      oppositeEndPointMock.Stub (stub => stub.CanBeCollected).Return (false);
      oppositeEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (realEndPoint));
      oppositeEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointMock);

      _map.UnregisterRealObjectEndPoint (id);

      oppositeEndPointMock.VerifyAllExpectations();
      Assert.That (_map[oppositeEndPointID], Is.Not.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_RemovesOppositeVirtualObjectEndPoint_IfCanBeCollected ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);
      var realEndPoint = new RealObjectEndPoint (
          ClientTransactionMock, id, foreignKeyDataContainer, ClientTransactionMock.DataManager, ClientTransactionMock.DataManager);
      RelationEndPointMapTestHelper.AddEndPoint (_map, realEndPoint);

      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointStub.Stub (stub => stub.HasChanged).Return (false);
      oppositeEndPointStub.Stub (stub => stub.CanBeCollected).Return (true);
      oppositeEndPointStub.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointStub);

      _map.UnregisterRealObjectEndPoint (id);

      oppositeEndPointStub.VerifyAllExpectations ();
      Assert.That (_map[oppositeEndPointID], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_OppositeVirtualCollectionEndPointNotLoaded ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      RelationEndPointMapTestHelper.RemoveEndPoint (_map, oppositeEndPointID);
      Assert.That (_map[oppositeEndPointID], Is.Null);

      _map.UnregisterRealObjectEndPoint (id);

      Assert.That (_map[oppositeEndPointID], Is.Null);
    }

    [Test]
    public void UnregisterRealObjectEndPoint_OppositeAnonymousEndPoint ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Client1);

      _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      var oppositeEndPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, id.Definition.GetOppositeEndPointDefinition());
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
      var foreignKeyDataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (id, DomainObjectIDs.Order1);

      var objectEndPoint = _map.RegisterRealObjectEndPoint (id, foreignKeyDataContainer);
      Assert.That (_map[id], Is.Not.Null);

      Assert.That (objectEndPoint.OppositeObjectID, Is.Not.Null);
      RealObjectEndPointTestHelper.SetOppositeObjectID (objectEndPoint, null);
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
    public void RegisterCollectionEndPoint_RegistersEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID);

      Assert.That (_map[endPointID], Is.Not.Null);
      Assert.That (_map[endPointID], Is.SameAs (endPoint));

      Assert.That (endPoint.IsDataComplete, Is.False);
      Assert.That (endPoint.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (endPoint.LazyLoader, Is.SameAs (_map.LazyLoader));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (_map.EndPointProvider));
      Assert.That (endPoint.DataKeeperFactory, Is.SameAs (_map.CollectionEndPointDataKeeperFactory));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);
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
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);

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
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, null);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeEndPointDefinition = endPointID.Definition.GetOppositeEndPointDefinition();
      var expectedID = RelationEndPointID.Create (null, oppositeEndPointDefinition);

      Assert.That (_map[expectedID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (VirtualObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.IsDataComplete, Is.True);
      Assert.That (objectEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (RealObjectEndPoint) _map[endPointID];
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.Not.Null);
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (dataContainer.PropertyValues[typeof (OrderTicket) + ".Order"]));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _map.RegisterEndPointsForDataContainer (dataContainer);

      var collectionEndPoint = (CollectionEndPoint) _map[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataComplete, Is.True);
      Assert.That (collectionEndPoint.Collection, Is.Empty);
    }

    [Test]
    public void GetUnregisterCommandForDataContainer_Existing_IncludesRealObjectEndPoints_IgnoresVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (realEndPointID, DomainObjectIDs.Order2);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var virtualObjectEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      virtualObjectEndPointStub.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      RelationEndPointMapTestHelper.AddEndPoint (_map, virtualObjectEndPointStub);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      collectionEndPointStub.Stub (stub => stub.ID).Return (collectionEndPointID);
      RelationEndPointMapTestHelper.AddEndPoint (_map, collectionEndPointStub);

      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_map));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.Member (realEndPointID));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.No.Member (virtualObjectEndPointID));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.No.Member (collectionEndPointID));
    }

    [Test]
    public void GetUnregisterCommandForDataContainer_New_IncludesRealObjectEndPoints_IncludesVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (realEndPointID);
      _map.RegisterEndPointsForDataContainer (dataContainer);

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_map[virtualObjectEndPointID], Is.Not.Null);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (_map[collectionEndPointID], Is.Not.Null);

      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_map));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.Member (realEndPointID));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.Member (virtualObjectEndPointID));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Has.Member (collectionEndPointID));
    }

    [Test]
    public void GetUnregisterCommandForDataContainer_IgnoresNonRegisteredEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_map));
      Assert.That (((UnregisterEndPointsCommand) command).EndPointIDs, Is.Empty);
    }
    
    [Test]
    public void GetUnregisterCommandForDataContainer_WithUnregisterableEndPoint ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderTicket"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPoint);

      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException>());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because its relations have been changed. Only "
          + "unchanged objects that are not part of changed relations can be unloaded."
          + Environment.NewLine
          + "Changed relations: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'."));
    }

    [Test]
    public void GetUnregisterCommandForDataContainer_WithUnregisterableEndPoint_DueToChangedOpposite ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (false);
      endPoint.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPoint);

      var oppositeEndPoint = MockRepository.GenerateStub<IVirtualEndPoint> ();
      oppositeEndPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
      oppositeEndPoint.Stub (stub => stub.Definition).Return (oppositeEndPoint.ID.Definition);
      oppositeEndPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPoint);

      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException> ());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because its relations have been changed. Only "
          + "unchanged objects that are not part of changed relations can be unloaded."
          + Environment.NewLine
          + "Changed relations: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer'."));
    }

    [Test]
    public void GetUnregisterCommandForDataContainer_WithMultipleUnregisterableEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint1 = MockRepository.GenerateStub<IRelationEndPoint>();
      endPoint1.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderTicket"));
      endPoint1.Stub (stub => stub.Definition).Return (endPoint1.ID.Definition);
      endPoint1.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPoint1);

      var endPoint2 = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPoint2.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderItems"));
      endPoint2.Stub (stub => stub.Definition).Return (endPoint2.ID.Definition);
      endPoint2.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPoint2);

      var endPoint3 = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPoint3.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint3.Stub (stub => stub.Definition).Return (endPoint3.ID.Definition);
      endPoint3.Stub (stub => stub.HasChanged).Return (false);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPoint3);

      var command = _map.GetUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (
          ((ExceptionCommand) command).Exception.Message,
          Is.EqualTo (
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because its relations have been changed. Only "
              + "unchanged objects that are not part of changed relations can be unloaded."
              + Environment.NewLine
              + "Changed relations: "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket', "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'."));

    }

    [Test]
    public void MarkCollectionEndPointComplete_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_map[endPointID], Is.Null);

      _map.MarkCollectionEndPointComplete (endPointID, new DomainObject[0]);

      var collectionEndPoint = (ICollectionEndPoint) _map[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataComplete, Is.True);
      Assert.That (collectionEndPoint.Collection, Is.Empty);
    }

    [Test]
    public void MarkCollectionEndPointComplete_EndPointRegistered_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      collectionEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointMapTestHelper.AddEndPoint (_map, collectionEndPointMock);
      Assert.That (_map[endPointID], Is.SameAs (collectionEndPointMock));

      var items = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order>() };

      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (items));
      collectionEndPointMock.Replay();

      _map.MarkCollectionEndPointComplete (endPointID, items);

      collectionEndPointMock.VerifyAllExpectations();
      Assert.That (_map[endPointID], Is.SameAs (collectionEndPointMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "MarkCollectionEndPointComplete can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void MarkCollectionEndPointComplete_ChecksCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      _map.MarkCollectionEndPointComplete (endPointID, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "MarkCollectionEndPointComplete cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void MarkCollectionEndPointComplete_ChecksAnonymity ()
    {
      var endPointID = RelationEndPointID.Create (
          DomainObjectIDs.Order1, new AnonymousRelationEndPointDefinition (DomainObjectIDs.Customer1.ClassDefinition));
      _map.MarkCollectionEndPointComplete (endPointID, new DomainObject[0]);
    }

    [Test]
    public void CommitAllEndPoints_CommitsEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID);
      endPoint.MarkDataComplete (new DomainObject[0]);

      var addedObject = Order.NewObject();
      endPoint.Collection.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.CommitAllEndPoints();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.Collection, Is.EqualTo (new[] { addedObject }));
    }

    [Test]
    public void RollbackAllEndPoints_RollsBackEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = _map.RegisterCollectionEndPoint (endPointID);
      endPoint.MarkDataComplete (new DomainObject[0]);

      var addedObject = Order.NewObject();
      endPoint.Collection.Add (addedObject);
      Assert.That (endPoint.HasChanged, Is.True);

      _map.RollbackAllEndPoints();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.Collection, Is.Empty);
    }

    [Test]
    public void RemoveEndPoint_RemovesEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID);
      Assert.That (_map[endPointID], Is.Not.Null);

      _map.RemoveEndPoint (endPointID);

      Assert.That (_map[endPointID], Is.Null);
    }

    [Test]
    public void RemoveEndPoint_RaisesNotification_BeforeRemoving ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _map.RegisterCollectionEndPoint (endPointID);
      Assert.That (_map[endPointID], Is.Not.Null);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      listenerMock.Expect (mock => mock.RelationEndPointMapUnregistering (ClientTransactionMock, endPointID))
          .WhenCalled (mi => Assert.That (_map[endPointID], Is.Not.Null));
      ClientTransactionMock.AddListener (listenerMock);

      listenerMock.Replay();

      _map.RemoveEndPoint (endPointID);

      listenerMock.VerifyAllExpectations();
      listenerMock.BackToRecord(); // For Discard
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

    [Test]
    public void GetOppositeEndPointWithLazyLoad ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order2, "OrderItems");

      var objectEndPointStub = MockRepository.GenerateStub<IObjectEndPoint>();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointStub);

      var oppositeEndPointStub = MockRepository.GenerateStub<IRelationEndPoint>();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointStub.Stub (stub => stub.IsDataComplete).Return (false);
      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointStub);

      var result = _map.GetOppositeEndPointWithLazyLoad (objectEndPointStub, DomainObjectIDs.Order2);

      Assert.That (result, Is.SameAs (oppositeEndPointStub));
    }

    [Test]
    public void GetOppositeEndPointWithLazyLoad_Loads ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");

      var objectEndPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointStub);

      var result = _map.GetOppositeEndPointWithLazyLoad (objectEndPointStub, DomainObjectIDs.Order2);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void GetOppositeEndPointWithLazyLoad_Null ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");

      var objectEndPointStub = MockRepository.GenerateStub<IObjectEndPoint>();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointStub);

      var result = _map.GetOppositeEndPointWithLazyLoad (objectEndPointStub, null);

      Assert.That (result, Is.TypeOf (typeof (NullCollectionEndPoint)));
      Assert.That (result.ID, Is.EqualTo (RelationEndPointID.Create (null, endPointID.Definition.GetMandatoryOppositeEndPointDefinition ())));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The end-point is not registered in this map.\r\nParameter name: objectEndPoint"
        )]
    public void GetOppositeEndPointWithLazyLoad_EndPointNotAdded_ThrowsException ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var objectEndPointStub = MockRepository.GenerateStub<IObjectEndPoint>();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);

      _map.GetOppositeEndPointWithLazyLoad (objectEndPointStub, DomainObjectIDs.Order2);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The end-point is not part of a bidirectional relation.\r\nParameter name: objectEndPoint")]
    public void GetOppositeEndPointWithLazyLoad_Unidirectional_ThrowsException ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var objectEndPointStub = MockRepository.GenerateStub<IObjectEndPoint>();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointStub);

      _map.GetOppositeEndPointWithLazyLoad (objectEndPointStub, DomainObjectIDs.Client1);
    }

    private VirtualObjectEndPoint CallRegisterVirtualObjectEndPoint (RelationEndPointMap relationEndPointMap, RelationEndPointID id)
    {
      return (VirtualObjectEndPoint) PrivateInvoke.InvokeNonPublicMethod (relationEndPointMap, "RegisterVirtualObjectEndPoint", id);
    }
  }
}