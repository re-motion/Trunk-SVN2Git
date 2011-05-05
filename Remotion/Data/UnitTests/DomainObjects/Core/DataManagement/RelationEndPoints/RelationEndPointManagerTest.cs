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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp();

      _relationEndPointManager = DataManagerTestHelper.GetRelationEndPointManager (ClientTransactionMock.DataManager);
    }

    [Test]
    public void CreateNullEndPoint_RealObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.GetTypeDefinition (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (ClientTransactionMock, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullRealObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_VirtualObjectEndPoint ()
    {
      var orderTicketDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (ClientTransactionMock, orderTicketDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      var objectEndPointID = RelationEndPointID.Create (null, orderTicketDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (objectEndPointID));
    }

    [Test]
    public void CreateNullEndPoint_CollectionEndPoint ()
    {
      var orderItemsDefinition = 
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");

      var nullObjectEndPoint = RelationEndPointManager.CreateNullEndPoint (ClientTransactionMock, orderItemsDefinition);

      Assert.That (nullObjectEndPoint, Is.TypeOf (typeof (NullCollectionEndPoint)));
      var collectionEndPointID = RelationEndPointID.Create (null, orderItemsDefinition);
      Assert.That (nullObjectEndPoint.ID, Is.EqualTo (collectionEndPointID));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);
      var foreignKeyProperty = dataContainer.PropertyValues[endPointID.Definition.PropertyName];

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var endPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.ForeignKeyProperty, Is.SameAs (foreignKeyProperty));
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersOppositeVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, DomainObjectIDs.Order2);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order2, "OrderTicket");
      var oppositeEndPoint = (VirtualObjectEndPoint) _relationEndPointManager.RelationEndPoints[oppositeID];

      Assert.That (oppositeEndPoint, Is.Not.Null);
      Assert.That (oppositeEndPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoOppositeNullObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, null);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var oppositeEndPointDefinition = endPointID.Definition.GetOppositeEndPointDefinition ();
      var expectedID = RelationEndPointID.Create (null, oppositeEndPointDefinition);

      Assert.That (_relationEndPointManager.RelationEndPoints[expectedID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_Existing_RegistersNoVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateExistingDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersVirtualObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (VirtualObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.IsDataComplete, Is.True);
      Assert.That (objectEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersRealObjectEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var objectEndPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.Not.Null);
      Assert.That (objectEndPoint.ForeignKeyProperty, Is.SameAs (dataContainer.PropertyValues[typeof (OrderTicket) + ".Order"]));
    }

    [Test]
    public void RegisterEndPointsForDataContainer_New_RegistersCollectionEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);

      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);

      var collectionEndPoint = (CollectionEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataComplete, Is.True);
      Assert.That (collectionEndPoint.Collection, Is.Empty);
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_Existing_IncludesRealObjectEndPoints_IgnoresVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (realEndPointID, DomainObjectIDs.Order2);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var realEndPoint = _relationEndPointManager.RelationEndPoints[realEndPointID];

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var virtualObjectEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      virtualObjectEndPointStub.Stub (stub => stub.ID).Return (virtualObjectEndPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, virtualObjectEndPointStub);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      collectionEndPointStub.Stub (stub => stub.ID).Return (collectionEndPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, collectionEndPointStub);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (realEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.No.Member (virtualObjectEndPointStub));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.No.Member (collectionEndPointStub));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_New_IncludesRealObjectEndPoints_IncludesVirtualEndPoints ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (realEndPointID);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      var realEndPoint = relationEndPoints[realEndPointID];

      var virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var virtualObjectEndPoint = relationEndPoints[virtualObjectEndPointID];
      Assert.That (virtualObjectEndPoint, Is.Not.Null);

      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var collectionEndPoint = relationEndPoints[collectionEndPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (realEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (virtualObjectEndPoint));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (collectionEndPoint));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_IgnoresNonRegisteredEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Is.Empty);
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnidirectionalEndPoints ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var dataContainer = RelationEndPointTestHelper.CreateNewDataContainer (endPointID);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
      var unidirectionalEndPoint = (RealObjectEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (unidirectionalEndPoint, Is.Not.Null);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) command).RegistrationAgent, Is.SameAs (_relationEndPointManager.RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) command).Map, Is.SameAs (_relationEndPointManager.RelationEndPoints));
      Assert.That (((UnregisterEndPointsCommand) command).EndPoints, Has.Member (unidirectionalEndPoint));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnregisterableEndPoint ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderTicket"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException> ());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because its relations have been changed. Only "
          + "unchanged objects that are not part of changed relations can be unloaded."
          + Environment.NewLine
          + "Changed relations: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'."));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithUnregisterableEndPoint_DueToChangedOpposite ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint.Stub (stub => stub.Definition).Return (endPoint.ID.Definition);
      endPoint.Stub (stub => stub.HasChanged).Return (false);
      endPoint.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint);

      var oppositeEndPoint = MockRepository.GenerateStub<IVirtualEndPoint> ();
      oppositeEndPoint.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
      oppositeEndPoint.Stub (stub => stub.Definition).Return (oppositeEndPoint.ID.Definition);
      oppositeEndPoint.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, oppositeEndPoint);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

      Assert.That (command, Is.TypeOf<ExceptionCommand> ());
      Assert.That (((ExceptionCommand) command).Exception, Is.TypeOf<InvalidOperationException> ());
      Assert.That (((ExceptionCommand) command).Exception.Message, Is.EqualTo (
          "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because its relations have been changed. Only "
          + "unchanged objects that are not part of changed relations can be unloaded."
          + Environment.NewLine
          + "Changed relations: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer'."));
    }

    [Test]
    public void CreateUnregisterCommandForDataContainer_WithMultipleUnregisterableEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPoint1 = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPoint1.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderTicket"));
      endPoint1.Stub (stub => stub.Definition).Return (endPoint1.ID.Definition);
      endPoint1.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint1);

      var endPoint2 = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPoint2.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "OrderItems"));
      endPoint2.Stub (stub => stub.Definition).Return (endPoint2.ID.Definition);
      endPoint2.Stub (stub => stub.HasChanged).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint2);

      var endPoint3 = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPoint3.Stub (stub => stub.ID).Return (RelationEndPointID.Create (dataContainer.ID, typeof (Order), "Customer"));
      endPoint3.Stub (stub => stub.Definition).Return (endPoint3.ID.Definition);
      endPoint3.Stub (stub => stub.HasChanged).Return (false);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPoint3);

      var command = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);

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
    public void GetRelationEndPointWithoutLoading_NullRealObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullRealObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_NullVirtualObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_NullCollectionEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullCollectionEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.SameAs (endPointStub));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);

      var result = _relationEndPointManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullObjectEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
      Assert.That (result.Definition, Is.EqualTo (endPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_NullCollectionEndPoint ()
    {
      var endPointDefinition =
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      var relationEndPointID = RelationEndPointID.Create (null, endPointDefinition);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

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
          _relationEndPointManager.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (client.ID, parentClientEndPointDefinition));

      Client parentClient = client.ParentClient;
      Assert.That (parentClient, Is.Not.Null);

      var anonymousEndPointDefinition = unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition ();
      _relationEndPointManager.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (parentClient.ID, anonymousEndPointDefinition));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersCollectionEndPoint ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderItemsEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[orderItemsEndPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (orderItemsEndPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.IsDataComplete, Is.True);
      Assert.That (relationEndPoints[orderItemsEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPoint ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // preload Order1 before lazily loading its virtual end point

      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[orderTicketEndPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (orderTicketEndPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (endPoint.IsDataComplete, Is.True);
      Assert.That (relationEndPoints[orderTicketEndPointID], Is.SameAs (endPoint));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_RegistersVirtualObjectEndPointWithNull ()
    {
      _relationEndPointManager.ClientTransaction.EnsureDataAvailable (DomainObjectIDs.Employee1); // preload Employee before lazily loading its virtual end point

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.Null);

      var endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);

      Assert.That (endPoint, Is.Not.Null);
      Assert.That (relationEndPoints[endPointID], Is.SameAs (endPoint));
      Assert.That (((ObjectEndPoint) endPoint).OppositeObjectID, Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_LoadsData_OfObjectsWithRealEndPointNotYetRegistered ()
    {
      var locationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      Assert.That (locationEndPointID.Definition.IsVirtual, Is.False);
      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Location1], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (locationEndPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Location1], Is.Not.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_DoesNotLoadData_OfObjectsWithVirtualEndPointNotYetRegistered_IfNotNeeded ()
    {
      OrderTicket.GetObject (DomainObjectIDs.OrderTicket1); // ensure opposite real end point is available
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var realEndPoint = _relationEndPointManager.GetRelationEndPointWithoutLoading (realEndPointID);
      Assert.That (realEndPoint, Is.Not.Null);
      Assert.That (realEndPoint.Definition.IsVirtual, Is.False);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (endPointID.Definition.IsVirtual, Is.True);
      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad_LoadsData_OfObjectsWithVirtualEndPointNotYetRegistered_IfNeeded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      Assert.That (endPointID.Definition.IsVirtual, Is.True);
      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (result, Is.Not.Null);

      Assert.That (ClientTransactionMock.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointAlreadyAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.SameAs (endPointMock));

      var result = _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.SameAs (endPointMock));
      Assert.That (relationEndPoints[endPointID], Is.SameAs (endPointMock));
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointNotAvailable_Null ()
    {
      var endPointID = RelationEndPointID.Create (
          null,
          Configuration.GetTypeDefinition (typeof (Order)).GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems"));

      var result = _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.TypeOf<NullCollectionEndPoint> ());
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointNotAvailable_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (relationEndPoints[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.False);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointNotAvailable_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      Assert.That (ClientTransactionMock.DataManager.DataContainers[endPointID.ObjectID], Is.Null);
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.Null);

      var result = _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.DataContainers[endPointID.ObjectID], Is.Not.Null);
      Assert.That (relationEndPoints[endPointID], Is.SameAs (result));
      Assert.That (result.IsDataComplete, Is.True);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_Anonymous ()
    {
      var unidirectionalRelationDefinition = Configuration
          .GetTypeDefinition (typeof (Location))
          .GetRelationEndPointDefinition (typeof (Location).FullName + ".Client")
          .GetOppositeEndPointDefinition ();
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, unidirectionalRelationDefinition);

      Assert.That (() => _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID), Throws.ArgumentException.With.Message.EqualTo (
          "GetRelationEndPointWithMinimumLoading cannot be called for anonymous end points.\r\nParameter name: endPointID"));
    }

    [Test]
    public void CommitAllEndPoints_CommitsEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Commit ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      _relationEndPointManager.CommitAllEndPoints ();

      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void RollbackAllEndPoints_RollsbackEndPoints ()
    {
      RelationEndPointID endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Commit ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      _relationEndPointManager.CommitAllEndPoints ();

      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void TrySetCollectionEndPointData_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_relationEndPointManager.RelationEndPoints[endPointID], Is.Null);

      var items = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order> () };

      var result = _relationEndPointManager.TrySetCollectionEndPointData (endPointID, items);

      Assert.That (result, Is.True);
      var collectionEndPoint = (ICollectionEndPoint) _relationEndPointManager.RelationEndPoints[endPointID];
      Assert.That (collectionEndPoint, Is.Not.Null);
      Assert.That (collectionEndPoint.IsDataComplete, Is.True);
      Assert.That (collectionEndPoint.GetData(), Is.EqualTo (items));
    }

    [Test]
    public void TrySetCollectionEndPointData_EndPointRegistered_NotComplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      collectionEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, collectionEndPointMock);
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.SameAs (collectionEndPointMock));

      var items = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order>() };

      collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (items));
      collectionEndPointMock.Replay();

      var result = _relationEndPointManager.TrySetCollectionEndPointData (endPointID, items);

      collectionEndPointMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (relationEndPoints[endPointID], Is.SameAs (collectionEndPointMock));
    }

    [Test]
    public void TrySetCollectionEndPointData_EndPointRegistered_AlreadyComplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      collectionEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, collectionEndPointMock);
      var relationEndPoints = _relationEndPointManager.RelationEndPoints;
      Assert.That (relationEndPoints[endPointID], Is.SameAs (collectionEndPointMock));

      var items = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order> () };

      collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      collectionEndPointMock.Replay ();

      var result = _relationEndPointManager.TrySetCollectionEndPointData (endPointID, items);

      collectionEndPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "SetCollectionEndPointData can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void TrySetCollectionEndPointData_ChecksCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      _relationEndPointManager.TrySetCollectionEndPointData (endPointID, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "SetCollectionEndPointData cannot be called for anonymous end points.\r\nParameter name: endPointID")]
    public void TrySetCollectionEndPointData_ChecksAnonymity ()
    {
      var endPointID = RelationEndPointID.Create (
          DomainObjectIDs.Order1, new AnonymousRelationEndPointDefinition (DomainObjectIDs.Customer1.ClassDefinition));
      _relationEndPointManager.TrySetCollectionEndPointData (endPointID, new DomainObject[0]);
    }
  }
}