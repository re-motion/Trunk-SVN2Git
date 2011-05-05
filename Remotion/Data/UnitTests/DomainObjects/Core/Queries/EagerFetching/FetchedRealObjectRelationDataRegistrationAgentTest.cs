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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedRealObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private FetchedRealObjectRelationDataRegistrationAgent _agent;

    private OrderTicket _originatingOrderTicket1;
    private OrderTicket _originatingOrderTicket2;

    private Order _fetchedOrder1;
    private Order _fetchedOrder2;
    private Order _fetchedOrder3;

    private IDataManager _dataManagerMock;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _agent = new FetchedRealObjectRelationDataRegistrationAgent ();

      _originatingOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _originatingOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _fetchedOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _fetchedOrder2 = DomainObjectMother.CreateFakeObject<Order> ();
      _fetchedOrder3 = DomainObjectMother.CreateFakeObject<Order> ();

      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager>();

      _endPointDefinition = GetEndPointDefinition (typeof (OrderTicket), "Order");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          _endPointDefinition,
          new[] { _originatingOrderTicket1, _originatingOrderTicket2 },
          new[] { _fetchedOrder1, _fetchedOrder2, _fetchedOrder3 },
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
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
        _endPointDefinition,
          new[] { _originatingOrderTicket1 },
        new DomainObject[] { null }, 
        _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' for domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. The end-point belongs to an object of class 'OrderTicket' but the domain object "
        + "has class 'Order'.")]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      _dataManagerMock.Replay();

      var invalidOriginalObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { invalidOriginalObject },
          new DomainObject[0], 
          _dataManagerMock);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _dataManagerMock.Replay();

      var invalidFetchedObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderTicket1, _originatingOrderTicket2 }, 
          new[] { invalidFetchedObject },
          _dataManagerMock);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongVirtuality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingOrderTicket1 }, new[] { _fetchedOrder1 }, _dataManagerMock), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only non-virtual object-valued relation end-points can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_agent);
    }
  }
}