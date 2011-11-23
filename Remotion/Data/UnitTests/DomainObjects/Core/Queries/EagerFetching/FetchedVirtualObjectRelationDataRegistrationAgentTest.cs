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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence;
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

    private ILoadedObjectData _originatingOrderData1;
    private ILoadedObjectData _originatingOrderData2;

    private OrderTicket _fetchedOrderTicket1;
    private OrderTicket _fetchedOrderTicket2;
    private OrderTicket _fetchedOrderTicket3;

    private LoadedObjectDataWithDataSourceData _fetchedOrderTicketData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderTicketData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderTicketData3;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedDataContainerProviderStub = MockRepository.GenerateStub<ILoadedDataContainerProvider> ();
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();

      _agent = new FetchedVirtualObjectRelationDataRegistrationAgent (_loadedDataContainerProviderStub, _virtualEndPointProviderMock);

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      _originatingOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder2);

      _fetchedOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _fetchedOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _fetchedOrderTicket3 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket3);

      _fetchedOrderTicketData1 = CreateFetchedOrderTicketData (_fetchedOrderTicket1, _originatingOrder1.ID);
      _fetchedOrderTicketData2 = CreateFetchedOrderTicketData (_fetchedOrderTicket2, _originatingOrder2.ID);
      _fetchedOrderTicketData3 = CreateFetchedOrderTicketData (_fetchedOrderTicket3, DomainObjectIDs.Order3);

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketData2.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketData3.DataSourceData);

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
          new[] { _originatingOrderData1, _originatingOrderData2 },
          new[] { _fetchedOrderTicketData1, _fetchedOrderTicketData2, _fetchedOrderTicketData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock1.VerifyAllExpectations ();
      endPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithOriginatingWithoutRelated ()
    {
      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock1, false);
      endPointMock1.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { _originatingOrderData1 }, new LoadedObjectDataWithDataSourceData[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock1.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { new NullLoadedObjectData() },
          new LoadedObjectDataWithDataSourceData[0]);

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

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition, 
          new[] { _originatingOrderData1 }, 
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var fetchedOrderTicketDataPointingToNull = CreateFetchedOrderTicketData (_fetchedOrderTicket1, null);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedOrderTicketDataPointingToNull.LoadedObjectData.ObjectID)).Return (fetchedOrderTicketDataPointingToNull.DataSourceData);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { _originatingOrderData1 }, new[] { fetchedOrderTicketDataPointingToNull });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket2.ID)).Return (_fetchedOrderTicketData2.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket3.ID)).Return (_fetchedOrderTicketData3.DataSourceData);

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
          new[] { _originatingOrderData1, _originatingOrderData2 },
          new[] { _fetchedOrderTicketData1, _fetchedOrderTicketData2 });

      _virtualEndPointProviderMock.VerifyAllExpectations();
      endPointMock1.VerifyAllExpectations();
      endPointMock2.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithInvalidDuplicateForeignKey ()
    {
      var fetchedOrderTicketWithDuplicateKey = CreateFetchedOrderTicketData (_fetchedOrderTicket2, _originatingOrder1.ID);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderTicket1.ID)).Return (_fetchedOrderTicketData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedOrderTicketWithDuplicateKey.LoadedObjectData.ObjectID)).Return (fetchedOrderTicketWithDuplicateKey.DataSourceData);

      _virtualEndPointProviderMock.Replay ();

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (
              _endPointDefinition,
              new[] { _originatingOrderData1 },
              new[] { _fetchedOrderTicketData1, fetchedOrderTicketWithDuplicateKey }),
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

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition, 
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.OrderTicket1) }, 
          new LoadedObjectDataWithDataSourceData[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition, 
          new[] { _originatingOrderData1 },  
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order2) });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      Assert.That (
          () =>
          _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingOrderData1 },
              new[] { _fetchedOrderTicketData1 }), 
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

    private LoadedObjectDataWithDataSourceData CreateFetchedOrderTicketData (OrderTicket fetchedObject, ObjectID orderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedObject.ID, "Order");
      var loadedObjectDataStub = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (fetchedObject);
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, orderID);
      return new LoadedObjectDataWithDataSourceData (loadedObjectDataStub, dataContainer);
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