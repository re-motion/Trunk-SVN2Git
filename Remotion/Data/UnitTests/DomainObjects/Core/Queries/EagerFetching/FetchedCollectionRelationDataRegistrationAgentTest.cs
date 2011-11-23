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
  public class FetchedCollectionRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private IVirtualEndPointProvider _virtualEndPointProviderMock;

    private FetchedCollectionRelationDataRegistrationAgent _agent;

    private Order _originatingOrder1;
    private Order _originatingOrder2;
    
    private ILoadedObjectData _originatingOrderData1;
    private ILoadedObjectData _originatingOrderData2;

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;
    
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData3;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();
      
      _agent = new FetchedCollectionRelationDataRegistrationAgent (_virtualEndPointProviderMock);

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      _originatingOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder2);

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem2);
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem3);

      _fetchedOrderItemData1 = CreateFetchedOrderItemData (_fetchedOrderItem1, _originatingOrder1.ID);
      _fetchedOrderItemData2 = CreateFetchedOrderItemData (_fetchedOrderItem2, _originatingOrder2.ID);
      _fetchedOrderItemData3 = CreateFetchedOrderItemData (_fetchedOrderItem3, _originatingOrder1.ID);

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrderItem1, _fetchedOrderItem3 }));

      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrderItem2 }));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock1.Replay ();
      collectionEndPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderData1, _originatingOrderData2 },
          new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.VerifyAllExpectations ();
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNonOriginatingObject ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrderItem1, _fetchedOrderItem3 }));

      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrderItem2 }));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock1.Replay ();
      collectionEndPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderData1, _originatingOrderData2 },
          new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.VerifyAllExpectations ();
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (_endPointDefinition, new[] { new NullLoadedObjectData() }, new LoadedObjectDataWithDataSourceData[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (new DomainObject[0]));
      
      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderData1 },
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var fetchedOrderItemDataPointingToNull = CreateFetchedOrderItemData (_fetchedOrderItem1, null);

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (new DomainObject[0]));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderData1 },
          new[] { fetchedOrderItemDataPointingToNull });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingOrder1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, true);

      ExpectGetEndPoint (_originatingOrder2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new DomainObject[] { _fetchedOrderItem2 }));

      _virtualEndPointProviderMock.Replay();
      collectionEndPointMock1.Replay();
      collectionEndPointMock2.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrderData1, _originatingOrderData2 },
          new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.AssertWasNotCalled (mock => mock.MarkDataComplete (Arg<DomainObject[]>.Is.Anything));
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' for domain object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid'. The end-point belongs to an object of class 'Order' but the domain object "
        + "has class 'OrderItem'.")]
    public void GroupAndRegisterRelatedObjects_OriginatingObjectOfInvalidType ()
    {
      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.OrderItem1) },
          new LoadedObjectDataWithDataSourceData[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' with the relation end-point " 
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem' was expected.")]
    public void GroupAndRegisterRelatedObjects_RelatedObjectOfInvalidType ()
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
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      Assert.That (
          () =>
          _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingOrderData1 },
              new[] { _fetchedOrderItemData1 }), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only collection-valued relations can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      var agent = new FetchedCollectionRelationDataRegistrationAgent (new SerializableVirtualEndPointProviderFake());
      
      var deserializedInstance = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }

    private LoadedObjectDataWithDataSourceData CreateFetchedOrderItemData (OrderItem fetchedObject, ObjectID orderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedObject.ID, "Order");
      var loadedObjectDataStub = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (fetchedObject);
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, orderID);
      return new LoadedObjectDataWithDataSourceData (loadedObjectDataStub, dataContainer);
    }

    private void ExpectGetEndPoint (
        ObjectID objectID,
        IRelationEndPointDefinition endPointDefinition,
        IVirtualEndPointProvider relationEndPointProviderMock,
        ICollectionEndPoint collectionEndPointMock,
        bool expectedIsDataComplete)
    {
      var relationEndPointID = RelationEndPointID.Create (objectID, endPointDefinition);
      relationEndPointProviderMock.Expect (mock => mock.GetOrCreateVirtualEndPoint (relationEndPointID)).Return (collectionEndPointMock);
      collectionEndPointMock.Expect (mock => mock.IsDataComplete).Return (expectedIsDataComplete);
    }
  }
}