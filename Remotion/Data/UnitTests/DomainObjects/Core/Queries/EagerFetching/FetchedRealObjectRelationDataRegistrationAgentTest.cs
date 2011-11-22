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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedRealObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private FetchedRealObjectRelationDataRegistrationAgent _agent;

    private ILoadedObjectData _originatingOrderTicketData1;
    private ILoadedObjectData _originatingOrderTicketData2;

    private ILoadedObjectData _fetchedOrderData1;
    private ILoadedObjectData _fetchedOrderData2;
    private ILoadedObjectData _fetchedOrderData3;

    private IVirtualEndPointProvider _virtualEndPointProviderMock;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _agent = new FetchedRealObjectRelationDataRegistrationAgent ();

      var originatingOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      var originatingOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);

      _originatingOrderTicketData1 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (originatingOrderTicket1);
      _originatingOrderTicketData2 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (originatingOrderTicket2);

      var fetchedOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fetchedOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);
      var fetchedOrder3 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order3);

      _fetchedOrderData1 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (fetchedOrder1);
      _fetchedOrderData2 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (fetchedOrder2);
      _fetchedOrderData3 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (fetchedOrder3);
      
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IRelationEndPointProvider> ();

      _endPointDefinition = GetEndPointDefinition (typeof (OrderTicket), "Order");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          _endPointDefinition,
          new[] { _originatingOrderTicketData1, _originatingOrderTicketData2 },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { new NullLoadedObjectData() }, new ILoadedObjectData[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { _originatingOrderTicketData1 }, new[] { new NullLoadedObjectData() });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' for domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. The end-point belongs to an object of class 'OrderTicket' but the domain object "
        + "has class 'Order'.")]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { _fetchedOrderData1 }, new ILoadedObjectData[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' with the relation end-point " 
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderTicketData1 }, 
          new[] { _originatingOrderTicketData2 });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongVirtuality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingOrderTicketData1 }, new[] { _fetchedOrderData1 }), 
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