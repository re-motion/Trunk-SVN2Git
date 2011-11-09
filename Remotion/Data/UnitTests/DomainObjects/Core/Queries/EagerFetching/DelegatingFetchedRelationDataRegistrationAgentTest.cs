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
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class DelegatingFetchedRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IFetchedRelationDataRegistrationAgent _realObjectAgentMock;
    private IFetchedRelationDataRegistrationAgent _virtualObjectAgentMock;
    private IFetchedRelationDataRegistrationAgent _collectionAgentMock;

    private DelegatingFetchedRelationDataRegistrationAgent _agent;

    private ILoadedDataContainerProvider _loadedDataContainerProviderStub;
    private IRelationEndPointProvider _relationEndPointProviderStub;
    private DomainObject[] _originatingObjects;
    private DomainObject[] _relatedObjects;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _realObjectAgentMock = _mockRepository.StrictMock<IFetchedRelationDataRegistrationAgent> ();
      _virtualObjectAgentMock = _mockRepository.StrictMock<IFetchedRelationDataRegistrationAgent> ();
      _collectionAgentMock = _mockRepository.StrictMock<IFetchedRelationDataRegistrationAgent> ();

      _agent = new DelegatingFetchedRelationDataRegistrationAgent (_realObjectAgentMock, _virtualObjectAgentMock, _collectionAgentMock);

      _loadedDataContainerProviderStub = MockRepository.GenerateStub<ILoadedDataContainerProvider> ();
      _relationEndPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _originatingObjects = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order> () };
      _relatedObjects = new DomainObject[] { DomainObjectMother.CreateFakeObject<Order> () };
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_AnonymousEndPoints ()
    {
      var endPointDefinition = Configuration
          .GetTypeDefinition (typeof (Location))
          .PropertyAccessorDataCache
          .GetPropertyAccessorData (typeof (Location), "Client")
          .RelationEndPointDefinition
          .GetOppositeEndPointDefinition();

      _mockRepository.ReplayAll();

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition, 
              _originatingObjects, 
              _relatedObjects),
          Throws.InvalidOperationException.With.Message.EqualTo ("Anonymous relation end-points cannot have data registered."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_RealObjectEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (OrderItem), "Order");

      _realObjectAgentMock
          .Expect (
              mock => mock.GroupAndRegisterRelatedObjects (
                  endPointDefinition,
                  _originatingObjects,
                  _relatedObjects));

      _mockRepository.ReplayAll ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition,
          _originatingObjects,
          _relatedObjects);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_VirtualObjectEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      _virtualObjectAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              _originatingObjects,
              _relatedObjects));

      _mockRepository.ReplayAll ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, _originatingObjects, _relatedObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_CollectionEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      _collectionAgentMock
          .Expect (
              mock =>
              mock.GroupAndRegisterRelatedObjects (
                  endPointDefinition, _originatingObjects, _relatedObjects));

      _mockRepository.ReplayAll ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, _originatingObjects, _relatedObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_CollectionEndPoints_EmptyRelatedObjects ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      var relatedObjects = new DomainObject[0];
      _collectionAgentMock
          .Expect (
              mock =>
              mock.GroupAndRegisterRelatedObjects (
                  endPointDefinition, _originatingObjects, relatedObjects));

      _mockRepository.ReplayAll ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, _originatingObjects, relatedObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serialization ()
    {
      var agent = new DelegatingFetchedRelationDataRegistrationAgent (
          new FetchedRealObjectRelationDataRegistrationAgent(),
          new FetchedVirtualObjectRelationDataRegistrationAgent (
              new SerializableLoadedDataContainerProviderFake(), new SerializableVirtualEndPointProviderFake()),
          new FetchedCollectionRelationDataRegistrationAgent (
              new SerializableLoadedDataContainerProviderFake(), new SerializableVirtualEndPointProviderFake()));

      var deserializedAgent = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedAgent.RealObjectDataRegistrationAgent, Is.Not.Null);
      Assert.That (deserializedAgent.VirtualObjectDataRegistrationAgent, Is.Not.Null);
      Assert.That (deserializedAgent.CollectionDataRegistrationAgent, Is.Not.Null);
    }
  }
}