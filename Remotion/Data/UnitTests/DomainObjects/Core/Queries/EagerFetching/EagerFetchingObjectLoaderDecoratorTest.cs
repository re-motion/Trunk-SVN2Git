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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class EagerFetchingObjectLoaderDecoratorTest : StandardMappingTest
  {
    private IFetchedRelationDataRegistrationAgent _registrationAgentMock;
    private IObjectLoader _decoratedObjectLoaderMock;

    private EagerFetchingObjectLoaderDecorator _decorator;

    private IQuery _queryStub;
    private IQuery _fetchQueryStub1;
    private IQuery _fetchQueryStub2;

    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;
    private IRelationEndPointDefinition _industrialSectorEndPointDefinition;

    private Order _originatingOrder1;
    private Order _originatingOrder2;
    private ILoadedObjectData _originatingOrderData1;
    private ILoadedObjectData _originatingOrderData2;

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;
    private ILoadedObjectData _fetchedOrderItemData1;
    private ILoadedObjectData _fetchedOrderItemData2;
    private ILoadedObjectData _fetchedOrderItemData3;

    private Customer _fetchedCustomer;
    private ILoadedObjectData _fetchedCustomerData;
    private IndustrialSector _indirectFetchedIndustrialSector;
    private ILoadedObjectData _indirectFetchedIndustrialSectorData;

    public override void SetUp ()
    {
      base.SetUp();

      _registrationAgentMock = MockRepository.GenerateStrictMock<IFetchedRelationDataRegistrationAgent>();
      _decoratedObjectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader>();

      _decorator = new EagerFetchingObjectLoaderDecorator (_decoratedObjectLoaderMock, _registrationAgentMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();
      _fetchQueryStub1 = MockRepository.GenerateStub<IQuery> ();
      _fetchQueryStub2 = MockRepository.GenerateStub<IQuery> ();

      _orderTicketEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      _industrialSectorEndPointDefinition = GetEndPointDefinition (typeof (Company), "IndustrialSector");

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _originatingOrderData1 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_originatingOrder2);

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem>();

      _fetchedOrderItemData1 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_fetchedOrderItem1);
      _fetchedOrderItemData2 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_fetchedOrderItem2);
      _fetchedOrderItemData3 = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_fetchedOrderItem3);

      _fetchedCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      _fetchedCustomerData = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_fetchedCustomer);

      _indirectFetchedIndustrialSector = DomainObjectMother.CreateFakeObject<IndustrialSector> ();
      _indirectFetchedIndustrialSectorData = LoadedObjectDataTestHelper.CreateLoadedObjectDataStub (_indirectFetchedIndustrialSector);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_PerformsEagerFetching ()
    {
      _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection
                   {
                        { _orderTicketEndPointDefinition, _fetchQueryStub1 },
                        { _customerEndPointDefinition, _fetchQueryStub2 }
                   });
      _fetchQueryStub1
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection ());
      _fetchQueryStub2
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection ());

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData1 = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };
      var relatedObjectsData2 = new[] { _fetchedCustomerData };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub1))
          .Return (relatedObjectsData1);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub2))
          .Return (relatedObjectsData2);
      _decoratedObjectLoaderMock.Replay();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData1));
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjectsData, relatedObjectsData2));
      _registrationAgentMock.Replay();

      var result = _decorator.GetOrLoadCollectionQueryResult (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (originatingObjectsData));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_NestedEagerFetching ()
    {
      _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection { { _customerEndPointDefinition, _fetchQueryStub1 } });
      _fetchQueryStub1
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection { { _industrialSectorEndPointDefinition, _fetchQueryStub2 } });
      _fetchQueryStub2
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection());

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData = new[] { _fetchedCustomerData };
      var indirectRelatedObjectsData = new[] { _indirectFetchedIndustrialSectorData };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub1))
          .Return (relatedObjectsData);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub2))
          .Return (indirectRelatedObjectsData);
      _decoratedObjectLoaderMock.Replay ();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjectsData, relatedObjectsData));
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_industrialSectorEndPointDefinition, relatedObjectsData, indirectRelatedObjectsData));
      _registrationAgentMock.Replay ();

      var result = _decorator.GetOrLoadCollectionQueryResult (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations ();
      _registrationAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (originatingObjectsData));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithExceptionInAgent ()
    {
       _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1 } });
       _fetchQueryStub1
           .Stub (stub => stub.EagerFetchQueries)
           .Return (new EagerFetchQueryCollection());

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub1))
          .Return (relatedObjectsData);
      _decoratedObjectLoaderMock.Replay();

      var invalidOperationException = new InvalidOperationException ("There was a problem registering stuff.");
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData))
          .Throw (invalidOperationException);
      _registrationAgentMock.Replay();

      Assert.That (
          () =>
          _decorator.GetOrLoadCollectionQueryResult (_queryStub),
          Throws.Exception.TypeOf<UnexpectedQueryResultException> ()
              .And.With.InnerException.SameAs (invalidOperationException)
              .And.With.Message.EqualTo ("Eager fetching encountered an unexpected query result: There was a problem registering stuff."));

      _decoratedObjectLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_NoOriginatingObjects ()
    {
      _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1 } });

      var originatingObjects = new DomainObject[0];
      var originatingObjectsData = new ILoadedObjectData[0];

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock.Replay ();
      _registrationAgentMock.Replay ();

      var result = _decorator.GetOrLoadCollectionQueryResult (_queryStub);

      _decoratedObjectLoaderMock.AssertWasNotCalled (mock => mock.GetOrLoadCollectionQueryResult (_fetchQueryStub1));

      _decoratedObjectLoaderMock.VerifyAllExpectations ();
      _registrationAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (originatingObjects));
    }

    [Test]
    public void Serialization ()
    {
      var instance = new EagerFetchingObjectLoaderDecorator (
          new SerializableObjectLoaderFake(), new SerializableFetchedRelationDataRegistrationAgentFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.RegistrationAgent, Is.Not.Null);
      Assert.That (deserializedInstance.DecoratedObjectLoader, Is.Not.Null);
    }
  }
}