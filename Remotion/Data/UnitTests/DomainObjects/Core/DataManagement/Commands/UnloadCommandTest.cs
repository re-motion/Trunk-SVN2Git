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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class UnloadCommandTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;
    private DataContainerMap _dataContainerMap;
    private RelationEndPointMap _relationEndPointMap;

    public override void SetUp ()
    {
      base.SetUp();

      _dataManager = ClientTransactionMock.DataManager;
      _dataContainerMap = _dataManager.DataContainerMap;
      _relationEndPointMap = _dataManager.RelationEndPointMap;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void Initialization_ThrowsOnChangedDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      _dataContainerMap[DomainObjectIDs.Order1].MarkAsChanged();

      CreateCommand (new[] { DomainObjectIDs.Order1 });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void Initialization_ThrowsOnChangedAssociatedEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);

      endPoint.OppositeDomainObjects.Add (OrderItem.NewObject());

      CreateCommand (DomainObjectIDs.Order1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because one of its relations has been changed. "
        + "Only unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void Initialization_ThrowsOnChangedOppositeEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);

      endPoint.OppositeDomainObjects.Add (OrderItem.NewObject());

      CreateCommand (DomainObjectIDs.OrderItem1); // OrderItem1.Order has not changed, but OrderItem1.Order.OrderItems has...
    }

    [Test]
    public void AffectedDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      Assert.That (
          command.AffectedDataContainers,
          Is.EqualTo (
              new[]
              {
                  _dataContainerMap[DomainObjectIDs.Order1],
                  _dataContainerMap[DomainObjectIDs.Order2]
              }));
    }

    [Test]
    public void AffectedDataContainers_IgnoresUnloadedDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);

      var command = CreateCommand (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      Assert.That (command.AffectedDataContainers, Is.EqualTo (new[] { _dataContainerMap[DomainObjectIDs.Order1] }));
    }

    [Test]
    public void AffectedEndPointIDs ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);

      var dataContainer1 = _dataContainerMap[DomainObjectIDs.Order1];
      var dataContainer2 = _dataContainerMap[DomainObjectIDs.Order2];

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      var expectedEndPoint1 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer1.ID, "Official");
      var expectedEndPoint2 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer2.ID, "Official");
      var notExpectedEndPoint1 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer1.ID, "OrderItems");
      var notExpectedEndPoint2 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer2.ID, "OrderTicket");

      var affectedEndPointIDs = command.AffectedEndPointIDs;
      Assert.That (affectedEndPointIDs, List.Contains (expectedEndPoint1));
      Assert.That (affectedEndPointIDs, List.Contains (expectedEndPoint2));
      Assert.That (affectedEndPointIDs, List.Not.Contains (notExpectedEndPoint1));
      Assert.That (affectedEndPointIDs, List.Not.Contains (notExpectedEndPoint2));
    }

    [Test]
    public void AffectedEndPointIDs_RealEndPoint ()
    {
      var realEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (realEndPoint.Definition.IsVirtual, Is.False);

      CheckAffectedEndPointIDsContains (DomainObjectIDs.OrderTicket1, realEndPoint);
    }

    [Test]
    public void AffectedEndPointIDs_VirtualEndPointWithNullValue ()
    {
      var loadedVirtualEndPointWithNullValue = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (loadedVirtualEndPointWithNullValue.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (loadedVirtualEndPointWithNullValue);
      Assert.That (
          ((ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[loadedVirtualEndPointWithNullValue]).OppositeObjectID, Is.Null);

      CheckAffectedEndPointIDsContains (DomainObjectIDs.Employee1, loadedVirtualEndPointWithNullValue);
    }

    [Test]
    public void AffectedEndPointIDs_OppositeVirtualObjectEndPoint ()
    {
      var oppositeVirtualObjectEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (oppositeVirtualObjectEndPoint.Definition.Cardinality, Is.EqualTo (CardinalityType.One));
      Assert.That (oppositeVirtualObjectEndPoint.Definition.IsVirtual, Is.True);

      CheckAffectedEndPointIDsContains (DomainObjectIDs.OrderTicket1, oppositeVirtualObjectEndPoint);
    }

    [Test]
    public void AffectedEndPointIDs_OppositeVirtualCollectionEndPoint ()
    {
      var oppositeCollectionEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (oppositeCollectionEndPoint.Definition.Cardinality, Is.EqualTo (CardinalityType.Many));
      Assert.That (oppositeCollectionEndPoint.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (oppositeCollectionEndPoint);

      CheckAffectedEndPointIDsContains (DomainObjectIDs.OrderItem1, oppositeCollectionEndPoint);
    }

    [Test]
    public void AffectedEndPointIDs_VirtualObjectEndPoint_NotContained ()
    {
      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (virtualObjectEndPointID.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (virtualObjectEndPointID);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.Order1, virtualObjectEndPointID);
    }

    [Test]
    public void AffectedEndPointIDs_VirtualObjectEndPoint_NullAndNotLoaded_NotContained ()
    {
      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[virtualObjectEndPointID], Is.Null);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.Employee1, virtualObjectEndPointID);
    }

    [Test]
    public void AffectedEndPointIDs_VirtualCollectionEndPoint_NotContained ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      EnsureEndPointAvailable (collectionEndPointID);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.Order1, collectionEndPointID);
    }

    [Test]
    public void AffectedEndPointIDs_OppositeRealObjectEndPoint_NotContained ()
    {
      var oppositeRealObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeRealObjectEndPointID.Definition.IsVirtual, Is.False);

      EnsureEndPointAvailable (oppositeRealObjectEndPointID);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.Order1, oppositeRealObjectEndPointID);
    }

    [Test]
    public void AffectedEndPointIDs_OppositeVirtualObjectEndPoint_OfNull_NotContained ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Computer4, "Employee");
      var oppositeVirtualObjectEndPointID = new RelationEndPointID (null, realEndPointID.Definition.GetOppositeEndPointDefinition());
      Assert.That (oppositeVirtualObjectEndPointID.Definition.IsVirtual, Is.True);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.Computer4, oppositeVirtualObjectEndPointID);

      Assert.That (((ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID]).OppositeObjectID, Is.Null);
    }

    [Test]
    public void AffectedEndPointIDs_OppositeVirtualCollectionEndPoint_NotLoaded_NotContained ()
    {
      var oppositeCollectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[oppositeCollectionEndPointID], Is.Null);

      CheckAffectedEndPointIDsNotContains (DomainObjectIDs.OrderItem1, oppositeCollectionEndPointID);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);
      var command = CreateCommand (order1.ID, order2.ID, DomainObjectIDs.Order3);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      ClientTransactionMock.AddListener (listenerMock);

      command.NotifyClientTransactionOfBegin();

      listenerMock.AssertWasCalled (mock => mock.ObjectsUnloading (Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order1, order2 })));
    }

    [Test]
    public void NotifyClientTransactionOfBegin_NoEventsIfNoAffectedObjects ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      command.NotifyClientTransactionOfBegin();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);
      var command = CreateCommand (order1.ID, order2.ID, DomainObjectIDs.Order3);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      ClientTransactionMock.AddListener (listenerMock);

      command.NotifyClientTransactionOfEnd();

      listenerMock.AssertWasCalled (mock => mock.ObjectsUnloaded (Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order1, order2 })));
    }

    [Test]
    public void NotifyClientTransactionOfEnd_NoEventsIfNoAffectedObjects ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      command.NotifyClientTransactionOfEnd();
    }

    [Test]
    public void Begin ()
    {
      var loadedObject = Order.GetObject (DomainObjectIDs.Order1);
      
      var unloadedObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      ClientTransactionMock.EnlistDomainObject (unloadedObject);
      Assert.That (unloadedObject.State, Is.EqualTo (StateType.NotLoadedYet));

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (loadedObject.ID, unloadedObject.ID);
      command.Begin ();

      Assert.That (loadedObject.UnloadingCalled, Is.True);

      Assert.That (loadedObject.UnloadedCalled, Is.False);

      Assert.That (unloadedObject.UnloadingCalled, Is.False);
      Assert.That (unloadedObject.UnloadedCalled, Is.False);
    }

    [Test]
    public void Begin_Transaction ()
    {
      var loadedObject = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (loadedObject.ID);

      using (ClientTransactionScope.EnterNullScope ())
      {
        command.Begin();
      }

      Assert.That (loadedObject.UnloadingTx, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void Begin_Sequence ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (order1.ID, order2.ID);
      command.Begin ();

      Assert.That (order1.UnloadingCalled, Is.True);
      Assert.That (order2.UnloadingCalled, Is.True);

      Assert.That (order1.UnloadingDateTime, Is.LessThan (order2.UnloadingDateTime), "order1 was called first");
    }

    [Test]
    public void End ()
    {
      var loadedObject = Order.GetObject (DomainObjectIDs.Order1);

      var unloadedObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      ClientTransactionMock.EnlistDomainObject (unloadedObject);
      Assert.That (unloadedObject.State, Is.EqualTo (StateType.NotLoadedYet));

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (loadedObject.ID, unloadedObject.ID);
      command.End ();

      Assert.That (loadedObject.UnloadedCalled, Is.True);

      Assert.That (loadedObject.UnloadingCalled, Is.False);

      Assert.That (unloadedObject.UnloadingCalled, Is.False);
      Assert.That (unloadedObject.UnloadedCalled, Is.False);
    }

    [Test]
    public void End_Transaction ()
    {
      var loadedObject = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (loadedObject.ID);
      using (ClientTransactionScope.EnterNullScope ())
      {
        command.End();
      }

      Assert.That (loadedObject.UnloadedTx, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void End_Sequence ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      var command = CreateCommand (order1.ID, order2.ID);
      command.End ();

      Assert.That (order1.UnloadedCalled, Is.True);
      Assert.That (order2.UnloadedCalled, Is.True);

      Assert.That (order1.UnloadedDateTime, Is.GreaterThan (order2.UnloadedDateTime), "order2 was called first");
    }

    [Test]
    public void Perform_NotLoaded ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (_dataManager.ClientTransaction);

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform();

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Perform_RemovesDataContainer ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform();

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Perform_RemovesRelationsWithForeignKeys ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var oppositeVirtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      EnsureDataAvailable (DomainObjectIDs.OrderTicket1);
      Assert.That (_relationEndPointMap[realEndPointID], Is.Not.Null);
      Assert.That (_relationEndPointMap[oppositeVirtualEndPointID], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.OrderTicket1);
      command.Perform();

      Assert.That (_relationEndPointMap[realEndPointID], Is.Null);
      Assert.That (_relationEndPointMap[oppositeVirtualEndPointID], Is.Null);
    }

    [Test]
    public void Perform_KeepsRelationsWithoutForeignKeys ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeRealEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      EnsureDataAvailable (DomainObjectIDs.OrderTicket1);
      Assert.That (_relationEndPointMap[oppositeRealEndPointID], Is.Not.Null);
      Assert.That (_relationEndPointMap[virtualEndPointID], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform();

      Assert.That (_relationEndPointMap[oppositeRealEndPointID], Is.Not.Null);
      Assert.That (_relationEndPointMap[virtualEndPointID], Is.Not.Null);
    }

    [Test]
    public void Perform_RemovesNullVirtualEndPoints ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      EnsureDataAvailable (DomainObjectIDs.Employee1);

      _relationEndPointMap.GetRelationEndPointWithLazyLoad (virtualEndPointID);

      Assert.That (_relationEndPointMap[virtualEndPointID], Is.Not.Null);
      Assert.That (((ObjectEndPoint) _relationEndPointMap[virtualEndPointID]).OppositeObjectID, Is.Null);

      var command = CreateCommand (DomainObjectIDs.Employee1);
      command.Perform();

      Assert.That (_relationEndPointMap[virtualEndPointID], Is.Null);
    }

    [Test]
    public void Perform_UnloadsOwningCollections ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPoint = (CollectionEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (collectionEndPointID);

      Assert.That (collectionEndPoint.HasChanged, Is.False);
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);
      Assert.That (_dataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));

      var command = CreateCommand (DomainObjectIDs.OrderItem1);
      command.Perform();

      Assert.That (collectionEndPoint.IsDataAvailable, Is.False);
    }

    [Test]
    public void Perform_Many ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      command.Perform();

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Null);
    }

    [Test]
    public void Perform_Many_ChecksPerformed_BeforePerformingAnything ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Not.Null);

      _dataContainerMap[DomainObjectIDs.Order2].MarkAsChanged();

      try
      {
        var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        command.Perform();
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Not.Null);
    }

    private void CheckAffectedEndPointIDsContains (ObjectID unloadedObject, RelationEndPointID expectedEndPointID)
    {
      var domainObject = RepositoryAccessor.GetObject (ClientTransactionMock, unloadedObject, false);
      var command = CreateCommand (domainObject.ID);
      var endPointIDs = command.AffectedEndPointIDs;

      Assert.That (endPointIDs, List.Contains (expectedEndPointID));
    }

    private void CheckAffectedEndPointIDsNotContains (ObjectID unloadedObject, RelationEndPointID unexpectedEndPointID)
    {
      var domainObject = RepositoryAccessor.GetObject (ClientTransactionMock, unloadedObject, false);
      var command = CreateCommand (domainObject.ID);
      var endPointIDs = command.AffectedEndPointIDs;

      Assert.That (endPointIDs, List.Not.Contains (unexpectedEndPointID));
    }

    private void EnsureDataAvailable (ObjectID objectID)
    {
      ClientTransactionMock.EnsureDataAvailable (objectID);
    }

    private void EnsureEndPointAvailable (RelationEndPointID endPointID)
    {
      ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
    }

    private UnloadCommand CreateCommand (params ObjectID[] objectIDs)
    {
      return new UnloadCommand (objectIDs, ClientTransactionMock, _dataContainerMap, _relationEndPointMap);
    }
  }
}