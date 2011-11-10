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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedVirtualObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private ILoadedDataContainerProvider _loadedDataContainerProviderStub;
    private IVirtualEndPointProvider _virtualEndPointProviderMock;

    private FetchedVirtualObjectRelationDataRegistrationAgent _agent;

    private Order _originatingOrder1;
    private Order _originatingOrder2;

    private OrderTicket _fetchedOrderTicket1;
    private OrderTicket _fetchedOrderTicket2;
    private OrderTicket _fetchedOrderTicket3;

    private DataContainer _fetchedOrderTicketDataContainer1;
    private DataContainer _fetchedOrderTicketDataContainer2;
    private DataContainer _fetchedOrderTicketDataContainer3;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedDataContainerProviderStub = MockRepository.GenerateStub<ILoadedDataContainerProvider> ();
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();

      _agent = new FetchedVirtualObjectRelationDataRegistrationAgent (_loadedDataContainerProviderStub, _virtualEndPointProviderMock);

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order> ();

      _fetchedOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _fetchedOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _fetchedOrderTicket3 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket3);

      _fetchedOrderTicketDataContainer1 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket1, _originatingOrder1.ID);
      _fetchedOrderTicketDataContainer2 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket2, _originatingOrder2.ID);
      _fetchedOrderTicketDataContainer3 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket2, DomainObjectIDs.Order3);

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer2);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketDataContainer3);

      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock1, false);
      endPointMock1.Expect (mock => mock.MarkDataComplete (_fetchedOrderTicket1));

      var endPointMock2 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingOrder2.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock2, false);
      endPointMock2.Expect (mock => mock.MarkDataComplete (_fetchedOrderTicket2));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();
      endPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderTicket1, _fetchedOrderTicket2, _fetchedOrderTicket3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock1.VerifyAllExpectations ();
      endPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new DomainObject[] { null },
          new DomainObject[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { _originatingOrder1 }, new DomainObject[] { null });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var dataContainerPointingToNull = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket1, null);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (dataContainerPointingToNull);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1 },
          new[] { _fetchedOrderTicket1 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer2);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketDataContainer3);

      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock1, true);

      var endPointMock2 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingOrder2.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock2, false);
      endPointMock2.Expect (mock => mock.MarkDataComplete (_fetchedOrderTicket2));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();
      endPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderTicket1, _fetchedOrderTicket2 });

      _virtualEndPointProviderMock.VerifyAllExpectations();
      endPointMock1.VerifyAllExpectations();
      endPointMock2.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithInvalidDuplicateForeignKey ()
    {
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer1);

      _virtualEndPointProviderMock.Replay ();

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (
              _endPointDefinition,
              new[] { _originatingOrder1, _originatingOrder2 },
              new[] { _fetchedOrderTicket1, _fetchedOrderTicket2 }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Two items in the related object result set point back to the same object. This is not allowed in a 1:1 relation. "
              + "Object 1: 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. "
              + "Object 2: 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid'. "
              + "Foreign key property: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'"));

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' for domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. The end-point belongs to an object of class 'Order' but the domain object "
        + "has class 'OrderTicket'.")]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay();

      var invalidOriginalObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { invalidOriginalObject },
          new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _virtualEndPointProviderMock.Replay();

      var invalidFetchedObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 }, 
          new[] { invalidFetchedObject });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      Assert.That (
          () =>
          _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingOrder1 },
              new[] { _fetchedOrderTicket1 }), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only virtual object-valued relation end-points can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      var agent = new FetchedVirtualObjectRelationDataRegistrationAgent (
          new SerializableLoadedDataContainerProviderFake (), new SerializableVirtualEndPointProviderFake ());

      var deserializedInstance = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedInstance.LoadedDataContainerProvider, Is.Not.Null);
      Assert.That (deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }

    private DataContainer CreateFetchedOrderTicketDataContainer (OrderTicket fetchedOrderTicket, ObjectID originatingOrderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedOrderTicket.ID, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, originatingOrderID);
      return dataContainer;
    }

    private void ExpectGetEndPoint (
        ObjectID objectID,
        IRelationEndPointDefinition endPointDefinition,
        IVirtualEndPointProvider virtualEndPointProviderMock,
        IVirtualObjectEndPoint virtualObjectEndPointMock,
        bool expectedIsDataComplete)
    {
      var relationEndPointID = RelationEndPointID.Create (objectID, endPointDefinition);
      virtualEndPointProviderMock.Expect (mock => mock.GetOrCreateVirtualEndPoint (relationEndPointID)).Return (virtualObjectEndPointMock);
      virtualObjectEndPointMock.Expect (mock => mock.IsDataComplete).Return (expectedIsDataComplete);
    }
  }
}