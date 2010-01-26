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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

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
      base.SetUp ();
      
      _dataManager = ClientTransactionMock.DataManager;
      _dataContainerMap = _dataManager.DataContainerMap;
      _relationEndPointMap = _dataManager.RelationEndPointMap;

    }
    
    [Test]
    public void Perform_NotLoaded ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (_dataManager.ClientTransaction);

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform ();

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Perform_RemovesDataContainer ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform ();

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
      command.Perform ();

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
      command.Perform ();

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
      command.Perform ();

      Assert.That (_relationEndPointMap[virtualEndPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void Perform_NotUnchanged ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      _dataContainerMap[DomainObjectIDs.Order1].MarkAsChanged ();
      Assert.That (_dataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));

      var command = CreateCommand (DomainObjectIDs.Order1);
      command.Perform ();
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
      command.Perform ();

      Assert.That (collectionEndPoint.IsDataAvailable, Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void Perform_ChangedCollection ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPoint = (CollectionEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (collectionEndPointID);
      collectionEndPoint.OppositeDomainObjects.Add (OrderItem.NewObject ());

      Assert.That (collectionEndPoint.HasChanged, Is.True);
      Assert.That (_dataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));

      var command = CreateCommand (DomainObjectIDs.OrderItem1);
      command.Perform ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer'.")]
    public void Perform_ChangedVirtualNullEndPoint ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee3, "Computer");

      var virtualEndPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (virtualEndPointID);
      Assert.That (virtualEndPoint.OppositeObjectID, Is.Not.Null);
      virtualEndPoint.OppositeObjectID = null;

      Assert.That (virtualEndPoint.HasChanged, Is.True);
      Assert.That (_dataContainerMap[DomainObjectIDs.Employee3].State, Is.EqualTo (StateType.Unchanged));

      var command = CreateCommand (DomainObjectIDs.Employee3);
      command.Perform ();
    }

    [Test]
    public void Perform_Many ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Not.Null);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      command.Perform ();

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

      _dataContainerMap[DomainObjectIDs.Order2].MarkAsChanged ();

      try
      {
        var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        command.Perform ();
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (_dataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (_dataContainerMap[DomainObjectIDs.Order2], Is.Not.Null);
    }

    [Test]
    public void GetAndCheckAffectedDataContainers_GetsDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      var result = command.GetAndCheckAffectedDataContainers ();

      Assert.That (result, Is.EqualTo (new[] { 
          ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], 
          ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order2]}));
    }

    [Test]
    public void GetAndCheckAffectedDataContainers_IgnoresUnloadedDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);

      var command = CreateCommand (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });
      var result = command.GetAndCheckAffectedDataContainers ();

      Assert.That (result, Is.EqualTo (new[] { ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1] }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void GetAndCheckAffectedDataContainers_ThrowsOnChangedDataContainers ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1].MarkAsChanged ();

      var command = CreateCommand (new[] { DomainObjectIDs.Order1 });
      command.GetAndCheckAffectedDataContainers ();
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      EnsureDataAvailable (DomainObjectIDs.Order2);

      var dataContainer1 = ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1];
      var dataContainer2 = ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order2];

      var command = CreateCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      var result = command.GetAndCheckAffectedEndPointIDs (new[] { dataContainer1, dataContainer2 });

      var expectedEndPoint1 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer1.ID, "Official");
      var expectedEndPoint2 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer2.ID, "Official");
      var notExpectedEndPoint1 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer1.ID, "OrderItems");
      var notExpectedEndPoint2 = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer2.ID, "OrderTicket");

      Assert.That (result, List.Contains (expectedEndPoint1));
      Assert.That (result, List.Contains (expectedEndPoint2));
      Assert.That (result, List.Not.Contains (notExpectedEndPoint1));
      Assert.That (result, List.Not.Contains (notExpectedEndPoint2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void GetAndCheckAffectedEndPointIDs_ThrowsOnChangedEndPoints ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);

      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1];

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);

      endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      var command = CreateCommand (dataContainer.ID);
      command.GetAndCheckAffectedEndPointIDs (new[] { dataContainer });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Real end point has not been registered.")]
    public void GetAndCheckAffectedEndPointIDs_One_NotRegistered ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var command = CreateCommand (dataContainer.ID);
      command.GetAndCheckAffectedEndPointIDs (dataContainer);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_RealEndPoint_Returned ()
    {
      var realEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (realEndPoint.Definition.IsVirtual, Is.False);

      GetAffectedEndPointIDs_Returns (DomainObjectIDs.OrderTicket1, realEndPoint);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_VirtualEndPointWithNullValue_Returned ()
    {
      var loadedVirtualEndPointWithNullValue = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (loadedVirtualEndPointWithNullValue.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (loadedVirtualEndPointWithNullValue);
      Assert.That (((ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[loadedVirtualEndPointWithNullValue]).OppositeObjectID, Is.Null);

      GetAffectedEndPointIDs_Returns (DomainObjectIDs.Employee1, loadedVirtualEndPointWithNullValue);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_OppositeVirtualObjectEndPoint_Returned ()
    {
      var oppositeVirtualObjectEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (oppositeVirtualObjectEndPoint.Definition.Cardinality, Is.EqualTo (CardinalityType.One));
      Assert.That (oppositeVirtualObjectEndPoint.Definition.IsVirtual, Is.True);

      GetAffectedEndPointIDs_Returns (DomainObjectIDs.OrderTicket1, oppositeVirtualObjectEndPoint);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_OppositeVirtualCollectionEndPoint_Returned ()
    {
      var oppositeCollectionEndPoint = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (oppositeCollectionEndPoint.Definition.Cardinality, Is.EqualTo (CardinalityType.Many));
      Assert.That (oppositeCollectionEndPoint.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (oppositeCollectionEndPoint);

      GetAffectedEndPointIDs_Returns (DomainObjectIDs.OrderItem1, oppositeCollectionEndPoint);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_VirtualObjectEndPoint_NotReturned ()
    {
      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (virtualObjectEndPointID.Definition.IsVirtual, Is.True);

      EnsureEndPointAvailable (virtualObjectEndPointID);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.Order1, virtualObjectEndPointID);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_VirtualObjectEndPoint_NullAndNotLoaded_NotReturned ()
    {
      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[virtualObjectEndPointID], Is.Null);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.Employee1, virtualObjectEndPointID);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_VirtualCollectionEndPoint_NotReturned ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      EnsureEndPointAvailable (collectionEndPointID);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.Order1, collectionEndPointID);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_OppositeRealObjectEndPoint_NotReturned ()
    {
      var oppositeRealObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeRealObjectEndPointID.Definition.IsVirtual, Is.False);

      EnsureEndPointAvailable (oppositeRealObjectEndPointID);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.Order1, oppositeRealObjectEndPointID);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_OppositeVirtualObjectEndPoint_OfNull_NotReturned ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Computer4, "Employee");
      var oppositeVirtualObjectEndPointID = new RelationEndPointID (null, realEndPointID.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeVirtualObjectEndPointID.Definition.IsVirtual, Is.True);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.Computer4, oppositeVirtualObjectEndPointID);

      Assert.That (((ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID]).OppositeObjectID, Is.Null);
    }

    [Test]
    public void GetAndCheckAffectedEndPointIDs_One_OppositeVirtualCollectionEndPoint_NotLoaded_NotReturned ()
    {
      var oppositeCollectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[oppositeCollectionEndPointID], Is.Null);

      GetAffectedEndPointIDs_NotReturns (DomainObjectIDs.OrderItem1, oppositeCollectionEndPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be unloaded because one of its relations has been changed. "
        + "Only unchanged objects can be unloaded. Changed end point: "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'.")]
    public void GetAndCheckAffectedEndPointIDs_One_ChangedAssociatedEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);

      endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[endPoint.ObjectID];
      var command = CreateCommand (dataContainer.ID);
      command.GetAndCheckAffectedEndPointIDs (dataContainer);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because one of its relations has been changed. "
        + "Only unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void GetAndCheckAffectedEndPointIDs_One_ChangedOppositeEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);

      endPoint.OppositeDomainObjects.Add (OrderItem.NewObject ());

      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.OrderItem1];
      var command = CreateCommand (dataContainer.ID);
      command.GetAndCheckAffectedEndPointIDs (dataContainer);
    }

    private void GetAffectedEndPointIDs_Returns (ObjectID unloadedObject, RelationEndPointID expectedEndPointID)
    {
      var domainObject = RepositoryAccessor.GetObject (ClientTransactionMock, unloadedObject, false);
      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID];
      var command = CreateCommand (dataContainer.ID);
      var endPointIDs = command.GetAndCheckAffectedEndPointIDs (dataContainer);

      Assert.That (endPointIDs, List.Contains (expectedEndPointID));
    }

    private void GetAffectedEndPointIDs_NotReturns (ObjectID unloadedObject, RelationEndPointID unexpectedEndPointID)
    {
      var domainObject = RepositoryAccessor.GetObject (ClientTransactionMock, unloadedObject, false);
      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID];
      var command = CreateCommand (dataContainer.ID);
      var endPointIDs = command.GetAndCheckAffectedEndPointIDs (dataContainer);

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
      return new UnloadCommand (objectIDs, _dataContainerMap, _relationEndPointMap);
    }

  }
}