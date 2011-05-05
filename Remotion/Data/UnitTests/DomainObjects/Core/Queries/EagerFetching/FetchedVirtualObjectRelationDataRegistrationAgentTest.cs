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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedVirtualObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private FetchedVirtualObjectRelationDataRegistrationAgent _agent;

    private Order _originatingOrder1;
    private Order _originatingOrder2;

    private OrderTicket _fetchedOrderTicket1;
    private OrderTicket _fetchedOrderTicket2;
    private OrderTicket _fetchedOrderTicket3;

    private DataContainer _fetchedOrderTicketDataContainer1;
    private DataContainer _fetchedOrderTicketDataContainer2;
    private DataContainer _fetchedOrderTicketDataContainer3;

    private IDataManager _dataManagerMock;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _agent = new FetchedVirtualObjectRelationDataRegistrationAgent ();

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order> ();

      _fetchedOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _fetchedOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _fetchedOrderTicket3 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket3);

      _fetchedOrderTicketDataContainer1 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket1, _originatingOrder1.ID);
      _fetchedOrderTicketDataContainer2 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket2, _originatingOrder2.ID);
      _fetchedOrderTicketDataContainer3 = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket2, DomainObjectIDs.Order3);

      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager>();

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketDataContainer3);

      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true, _fetchedOrderTicket1);
      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder2.ID, _endPointDefinition, true, _fetchedOrderTicket2);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderTicket1, _fetchedOrderTicket2, _fetchedOrderTicket3 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new DomainObject[] { null },
          new DomainObject[0], 
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true, null);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
        _endPointDefinition,
          new[] { _originatingOrder1 },
        new DomainObject[] { null }, 
        _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var dataContainerPointingToNull = CreateFetchedOrderTicketDataContainer (_fetchedOrderTicket1, null);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (dataContainerPointingToNull);

      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true, null);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1 },
          new[] { _fetchedOrderTicket1 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketDataContainer3);

      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder1.ID, _endPointDefinition, false, _fetchedOrderTicket1);
      ExpectMarkVirtualObjectEndPointComplete (_originatingOrder2.ID, _endPointDefinition, true, _fetchedOrderTicket2);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderTicket1, _fetchedOrderTicket2 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithInvalidDuplicateForeignKey ()
    {
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketDataContainer1);

      _dataManagerMock.Replay ();

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (
              _endPointDefinition,
              new[] { _originatingOrder1, _originatingOrder2 },
              new[] { _fetchedOrderTicket1, _fetchedOrderTicket2 },
              _dataManagerMock),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Two items in the related object result set point back to the same object. This is not allowed in a 1:1 relation. "
              + "Object 1: 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. "
              + "Object 2: 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid'. "
              + "Foreign key property: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'"));

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' for domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. The end-point belongs to an object of class 'Order' but the domain object "
        + "has class 'OrderTicket'.")]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      _dataManagerMock.Replay();

      var invalidOriginalObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { invalidOriginalObject },
          new DomainObject[0], 
          _dataManagerMock);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _dataManagerMock.Replay();

      var invalidFetchedObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 }, 
          new[] { invalidFetchedObject },
          _dataManagerMock);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingOrder1 }, new[] { _fetchedOrderTicket1 }, _dataManagerMock), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only virtual object-valued relation end-points can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_agent);
    }

    private DataContainer CreateFetchedOrderTicketDataContainer (OrderTicket fetchedOrderTicket, ObjectID originatingOrderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedOrderTicket.ID, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, originatingOrderID);
      return dataContainer;
    }

    private void ExpectMarkVirtualObjectEndPointComplete (ObjectID objectID, IRelationEndPointDefinition endPointDefinition, bool result, DomainObject item)
    {
      var relationEndPointID = RelationEndPointID.Create (objectID, endPointDefinition);
      _dataManagerMock.Expect (mock => mock.TrySetVirtualObjectEndPointData (relationEndPointID, item)).Return (result);
    }
  }
}