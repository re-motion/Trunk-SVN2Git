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
  public class EagerFetcherTest : StandardMappingTest
  {
    private IFetchedRelationDataRegistrationAgent _registrationAgentMock;
    private IObjectLoader _objectLoaderMock;

    private EagerFetcher _eagerFetcher;

    private IQuery _queryStub;

    private IRelationEndPointDefinition _endPointDefinition;

    private Order _originatingOrder1;
    private Order _originatingOrder2;

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;

    public override void SetUp ()
    {
      base.SetUp();

      _registrationAgentMock = MockRepository.GenerateStrictMock<IFetchedRelationDataRegistrationAgent>();
      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader>();

      _eagerFetcher = new EagerFetcher (_registrationAgentMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem>();
    }

    [Test]
    public void PerformEagerFetching ()
    {
      var originatingObjects = new[] { _originatingOrder1, _originatingOrder2 };
      var relatedObjects = new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 };

      _objectLoaderMock
          .Expect (
              mock =>
              mock.GetOrLoadCollectionQueryResult<DomainObject> (
                  _queryStub))
          .Return (relatedObjects);
      _objectLoaderMock.Replay();

      _registrationAgentMock.Expect (
          mock =>
          mock.GroupAndRegisterRelatedObjects (
              _endPointDefinition, originatingObjects, relatedObjects));
      _registrationAgentMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          originatingObjects,
          _endPointDefinition,
          _queryStub,
          _objectLoaderMock);

      _objectLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_WithExceptionInAgent ()
    {
      var originatingObjects = new[] { _originatingOrder1, _originatingOrder2 };
      var relatedObjects = new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 };

      _objectLoaderMock
          .Expect (
              mock =>
              mock.GetOrLoadCollectionQueryResult<DomainObject> (
                  _queryStub))
          .Return (relatedObjects);
      _objectLoaderMock.Replay();

      var invalidOperationException = new InvalidOperationException ("There was a problem registering stuff.");
      _registrationAgentMock
          .Expect (
              mock =>
              mock.GroupAndRegisterRelatedObjects (
                  _endPointDefinition, originatingObjects, relatedObjects))
          .Throw (invalidOperationException);
      _registrationAgentMock.Replay();

      Assert.That (
          () =>
          _eagerFetcher.PerformEagerFetching (
              originatingObjects,
              _endPointDefinition,
              _queryStub,
              _objectLoaderMock),
          Throws.Exception.TypeOf<UnexpectedQueryResultException>()
              .And.With.InnerException.SameAs (invalidOperationException)
              .And.With.Message.EqualTo ("Eager fetching encountered an unexpected query result: There was a problem registering stuff."));

      _objectLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void Serialization ()
    {
      var instance = new EagerFetcher (new SerializableFetchedRelationDataRegistrationAgentFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.RegistrationAgent, Is.Not.Null);
    }
  }
}