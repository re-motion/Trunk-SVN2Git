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
using Remotion.Data.DomainObjects.Persistence;
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

    private Customer _originatingCustomer1;
    private Customer _originatingCustomer2;
    
    private ILoadedObjectData _originatingCustomerData1;
    private ILoadedObjectData _originatingCustomerData2;

    private Order _fetchedOrder1;
    private Order _fetchedOrder2;
    private Order _fetchedOrder3;
    
    private LoadedObjectDataWithDataSourceData _fetchedOrderData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData3;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();
      
      _agent = new FetchedCollectionRelationDataRegistrationAgent (_virtualEndPointProviderMock);

      _originatingCustomer1 = DomainObjectMother.CreateFakeObject<Customer> (DomainObjectIDs.Customer1);
      _originatingCustomer2 = DomainObjectMother.CreateFakeObject<Customer> (DomainObjectIDs.Customer2);

      _originatingCustomerData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingCustomer1);
      _originatingCustomerData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingCustomer2);

      _fetchedOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _fetchedOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order3);
      _fetchedOrder3 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order4);

      _fetchedOrderData1 = CreateFetchedOrderData (_fetchedOrder1, _originatingCustomer1.ID);
      _fetchedOrderData2 = CreateFetchedOrderData (_fetchedOrder2, _originatingCustomer2.ID);
      _fetchedOrderData3 = CreateFetchedOrderData (_fetchedOrder3, _originatingCustomer1.ID);

      _endPointDefinition = GetEndPointDefinition (typeof (Customer), "Orders");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrder1, _fetchedOrder3 }));

      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrder2 }));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock1.Replay ();
      collectionEndPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1, _originatingCustomerData2 },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.VerifyAllExpectations ();
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNonOriginatingObject ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrder1, _fetchedOrder3 }));

      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new[] { _fetchedOrder2 }));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock1.Replay ();
      collectionEndPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1, _originatingCustomerData2 },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.VerifyAllExpectations ();
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The fetched mandatory collection property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' on object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' contains no items.")]
    public void GroupAndRegisterRelatedObjects_MandatoryEndPointWithoutRelatedObjects_Throws ()
    {
      var orderItemsEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      Assert.That (orderItemsEndPointDefinition.IsMandatory, Is.True);

      var originatingOrderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);

      _agent.GroupAndRegisterRelatedObjects (
          orderItemsEndPointDefinition,
          new[] { originatingOrderData },
          new LoadedObjectDataWithDataSourceData[0]);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_NonMandatoryEndPointWithoutRelatedObjects_RegistersEmptyCollection ()
    {
      var originatingCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Customer1);
      var ordersEndPointDefinition = GetEndPointDefinition (typeof (Customer), "Orders");
      Assert.That (ordersEndPointDefinition.IsMandatory, Is.False);

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (originatingCustomerData.ObjectID, ordersEndPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);

      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (new DomainObject[0]));

      _agent.GroupAndRegisterRelatedObjects (
          ordersEndPointDefinition,
          new[] { originatingCustomerData },
          new LoadedObjectDataWithDataSourceData[0]);

      collectionEndPointMock.VerifyAllExpectations ();
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
      ExpectGetEndPoint (_originatingCustomer1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (new DomainObject[0]));
      
      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1 },
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var fetchedOrderItemDataPointingToNull = CreateFetchedOrderData (_fetchedOrder1, null);

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Expect (mock => mock.MarkDataComplete (new DomainObject[0]));

      _virtualEndPointProviderMock.Replay ();
      collectionEndPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1 },
          new[] { fetchedOrderItemDataPointingToNull });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      var collectionEndPointMock1 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      var collectionEndPointMock2 = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      ExpectGetEndPoint (_originatingCustomer1.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, true);

      ExpectGetEndPoint (_originatingCustomer2.ID, _endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Expect (mock => mock.MarkDataComplete (new DomainObject[] { _fetchedOrder2 }));

      _virtualEndPointProviderMock.Replay();
      collectionEndPointMock1.Replay();
      collectionEndPointMock2.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1, _originatingCustomerData2 },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      collectionEndPointMock1.AssertWasNotCalled (mock => mock.MarkDataComplete (Arg<DomainObject[]>.Is.Anything));
      collectionEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' for domain object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid'. The end-point belongs to an object of class 'Customer' but the domain object "
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
        "Cannot associate object 'OrderItem|ad620a11-4bc4-4791-bcf4-a0770a08c5b0|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' was expected.")]
    public void GroupAndRegisterRelatedObjects_RelatedObjectOfInvalidType ()
    {
      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingCustomerData1 }, 
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.OrderItem2) });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      Assert.That (
          () =>
          _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingCustomerData1 },
              new[] { _fetchedOrderData1 }), 
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

    private LoadedObjectDataWithDataSourceData CreateFetchedOrderData (Order fetchedObject, ObjectID orderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedObject.ID, "Customer");
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