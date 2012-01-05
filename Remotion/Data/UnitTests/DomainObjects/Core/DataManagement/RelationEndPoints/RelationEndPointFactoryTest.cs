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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IRelationEndPointProvider _endPointProviderStub;
    private ILazyLoader _lazyLoaderStub;
    private IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager> _virtualObjectEndPointDataManagerFactoryStub;
    private IVirtualEndPointDataManagerFactory<ICollectionEndPointDataManager> _collectionEndPointDataManagerFactoryStub;
    private ICollectionEndPointCollectionProvider _collectionEndPointCollectionProviderStub;
    private IAssociatedCollectionDataStrategyFactory _associatedCollectionStrategyFactoryStub;

    private RelationEndPointFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _lazyLoaderStub = MockRepository.GenerateStub<ILazyLoader> ();

      var virtualObjectEndPointDataManager = MockRepository.GenerateStub<IVirtualObjectEndPointDataManager> ();
      virtualObjectEndPointDataManager.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);
      _virtualObjectEndPointDataManagerFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager>> ();
      _virtualObjectEndPointDataManagerFactoryStub
          .Stub (stub => stub.Create (Arg<RelationEndPointID>.Is.Anything))
          .Return (virtualObjectEndPointDataManager);

      var collectionEndPointDataManager = MockRepository.GenerateStub<ICollectionEndPointDataManager> ();
      collectionEndPointDataManager.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);
      _collectionEndPointDataManagerFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataManagerFactory<ICollectionEndPointDataManager>> ();
      _collectionEndPointDataManagerFactoryStub
          .Stub (stub => stub.Create (Arg<RelationEndPointID>.Is.Anything))
          .Return (collectionEndPointDataManager);

      _collectionEndPointCollectionProviderStub = MockRepository.GenerateStub<ICollectionEndPointCollectionProvider>();
      _associatedCollectionStrategyFactoryStub = MockRepository.GenerateStub<IAssociatedCollectionDataStrategyFactory>();

      _factory = new RelationEndPointFactory (
          _clientTransaction,
          _endPointProviderStub,
          _lazyLoaderStub,
          _virtualObjectEndPointDataManagerFactoryStub,
          _collectionEndPointDataManagerFactoryStub, 
          _collectionEndPointCollectionProviderStub,
          _associatedCollectionStrategyFactoryStub);
    }

    [Test]
    public void CreateRealObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      
      var endPoint = _factory.CreateRealObjectEndPoint (endPointID, dataContainer);

      Assert.That (endPoint, Is.TypeOf<RealObjectEndPoint> ());
      Assert.That (endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (((RealObjectEndPoint) endPoint).ForeignKeyDataContainer, Is.SameAs (dataContainer));
      Assert.That (((RealObjectEndPoint) endPoint).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void CreateRealObjectEndPoint_NonRealEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);

      Assert.That (
          () => _factory.CreateRealObjectEndPoint (endPointID, dataContainer), 
          Throws.ArgumentException.With.Message.EqualTo ("End point ID must refer to a non-virtual end point.\r\nParameter name: id"));
    }

    [Test]
    public void CreateVirtualObjectEndPoint_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualObjectEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<VirtualObjectEndPoint> ());
      Assert.That (endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (((VirtualObjectEndPoint) endPoint).LazyLoader, Is.SameAs (_lazyLoaderStub));
      Assert.That (((VirtualObjectEndPoint) endPoint).EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (((VirtualObjectEndPoint) endPoint).DataManagerFactory, Is.SameAs (_virtualObjectEndPointDataManagerFactoryStub));
      Assert.That (
          ((VirtualObjectEndPoint) endPoint).StateUpdateListener,
          Is.TypeOf<VirtualEndPointStateUpdateListener> ()
              .With.Property<VirtualEndPointStateUpdateListener> (l => l.ClientTransaction).SameAs (_clientTransaction)
              .And.Property<VirtualEndPointStateUpdateListener> (l => l.EndPointID).EqualTo (endPointID));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateVirtualObjectEndPoint_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualObjectEndPoint (endPointID, true);

      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void CreateVirtualObjectEndPoint_NonVirtualEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      Assert.That (
          () => _factory.CreateVirtualObjectEndPoint (endPointID, false), 
          Throws.ArgumentException.With.Message.EqualTo ("End point ID must refer to a virtual end point.\r\nParameter name: id"));
    }

    [Test]
    public void CreateVirtualObjectEndPoint_NonObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      Assert.That (
          () => _factory.CreateVirtualObjectEndPoint (endPointID, false),
          Throws.ArgumentException.With.Message.EqualTo ("End point ID must refer to an end point with cardinality 'One'.\r\nParameter name: id"));
    }

    [Test]
    public void CreateCollectionEndPoint_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPoint = _factory.CreateCollectionEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<CollectionEndPoint> ());
      Assert.That (endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (
          ((CollectionEndPoint) endPoint).CollectionManager, 
          Is.TypeOf<CollectionEndPointCollectionManager>()
            .With.Property<CollectionEndPointCollectionManager> (p => p.CollectionProvider).SameAs (_collectionEndPointCollectionProviderStub)
            .And.Property<CollectionEndPointCollectionManager> (p => p.DataStrategyFactory).SameAs (_associatedCollectionStrategyFactoryStub));
      Assert.That (((CollectionEndPoint) endPoint).LazyLoader, Is.SameAs (_lazyLoaderStub));
      Assert.That (((CollectionEndPoint) endPoint).EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (((CollectionEndPoint) endPoint).DataManagerFactory, Is.SameAs (_collectionEndPointDataManagerFactoryStub));
      Assert.That (
          ((CollectionEndPoint) endPoint).StateUpdateListener, 
          Is.TypeOf<VirtualEndPointStateUpdateListener>()
              .With.Property<VirtualEndPointStateUpdateListener> (l => l.ClientTransaction).SameAs (_clientTransaction)
              .And.Property<VirtualEndPointStateUpdateListener> (l => l.EndPointID).EqualTo (endPointID));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateCollectionEndPoint_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      _collectionEndPointCollectionProviderStub.Stub (stub => stub.GetCollection (endPointID)).Return (new DomainObjectCollection());

      var endPoint = _factory.CreateCollectionEndPoint (endPointID, true);

      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void CreateCollectionEndPoint_NonCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      Assert.That (
          () => _factory.CreateCollectionEndPoint (endPointID, false),
          Throws.ArgumentException.With.Message.EqualTo ("End point ID must refer to an end point with cardinality 'Many'.\r\nParameter name: id"));
    }

    [Test]
    public void CreateCollectionEndPoint_AnonymousEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateAnonymousEndPointID();

      Assert.That (
          () => _factory.CreateCollectionEndPoint (endPointID, false),
          Throws.ArgumentException.With.Message.EqualTo ("End point ID must not refer to an anonymous end point.\r\nParameter name: id"));
    }

    [Test]
    public void Serialization ()
    {
      IRelationEndPointProvider endPointProvider = new SerializableRelationEndPointProviderFake();
      var factory = new RelationEndPointFactory (
          _clientTransaction,
          endPointProvider,
          new SerializableLazyLoaderFake(),
          new SerializableVirtualObjectEndPointDataManagerFactoryFake(),
          new SerializableCollectionEndPointDataManagerFactoryFake(), 
          new SerializableCollectionEndPointCollectionProviderFake(),
          new SerializableAssociatedCollectionDataStrategyFactoryFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (factory);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedInstance.LazyLoader, Is.Not.Null);
      Assert.That (deserializedInstance.VirtualObjectEndPointDataManagerFactory, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionEndPointDataManagerFactory, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionEndPointCollectionProvider, Is.Not.Null);
      Assert.That (deserializedInstance.AssociatedCollectionDataStrategyFactory, Is.Not.Null);
    }
  }
}