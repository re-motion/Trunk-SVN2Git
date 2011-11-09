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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
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

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;

    private Customer _fetchedCustomer;
    private IndustrialSector _indirectFetchedIndustrialSector;

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

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem>();

      _fetchedCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      _indirectFetchedIndustrialSector = DomainObjectMother.CreateFakeObject<IndustrialSector> ();
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

      var originatingObjects = new[] { _originatingOrder1, _originatingOrder2 };
      var relatedObjects1 = new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 };
      var relatedObjects2 = new[] { _fetchedCustomer };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<Order> (_queryStub))
          .Return (originatingObjects);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub1))
          .Return (relatedObjects1);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub2))
          .Return (relatedObjects2);
      _decoratedObjectLoaderMock.Replay();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjects, relatedObjects1));
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjects, relatedObjects2));
      _registrationAgentMock.Replay();

      var result = _decorator.GetOrLoadCollectionQueryResult<Order> (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (originatingObjects));
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

      var originatingObjects = new[] { _originatingOrder1, _originatingOrder2 };
      var relatedObjects = new[] { _fetchedCustomer };
      var indirectRelatedObjects = new[] { _indirectFetchedIndustrialSector };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<Order> (_queryStub))
          .Return (originatingObjects);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub1))
          .Return (relatedObjects);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub2))
          .Return (indirectRelatedObjects);
      _decoratedObjectLoaderMock.Replay ();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjects, relatedObjects));
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_industrialSectorEndPointDefinition, relatedObjects, indirectRelatedObjects));
      _registrationAgentMock.Replay ();

      var result = _decorator.GetOrLoadCollectionQueryResult<Order> (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations ();
      _registrationAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (originatingObjects));
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

      var originatingObjects = new[] { _originatingOrder1, _originatingOrder2 };
      var relatedObjects = new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<Order> (_queryStub))
          .Return (originatingObjects);
      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub1))
          .Return (relatedObjects);
      _decoratedObjectLoaderMock.Replay();

      var invalidOperationException = new InvalidOperationException ("There was a problem registering stuff.");
      _registrationAgentMock
          .Expect (
              mock =>
              mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjects, relatedObjects))
          .Throw (invalidOperationException);
      _registrationAgentMock.Replay();

      Assert.That (
          () =>
          _decorator.GetOrLoadCollectionQueryResult<Order> (_queryStub),
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

      var originatingObjects = new Order[0];

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult<Order> (_queryStub))
          .Return (originatingObjects);
      _decoratedObjectLoaderMock.Replay ();
      _registrationAgentMock.Replay ();

      var result = _decorator.GetOrLoadCollectionQueryResult<Order> (_queryStub);

      _decoratedObjectLoaderMock.AssertWasNotCalled (mock => mock.GetOrLoadCollectionQueryResult<DomainObject> (_fetchQueryStub1));

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