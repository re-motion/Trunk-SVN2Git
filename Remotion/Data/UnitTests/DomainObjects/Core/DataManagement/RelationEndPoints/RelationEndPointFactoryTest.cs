// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IRelationEndPointProvider _endPointProviderStub;
    private ILazyLoader _lazyLoaderStub;
    private IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _virtualObjectEndPointDataKeeperFactoryStub;
    private IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _collectionEndPointDataKeeperFactoryStub;

    private RelationEndPointFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _lazyLoaderStub = MockRepository.GenerateStub<ILazyLoader> ();

      var virtualObjectEndPointDataKeeper = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      virtualObjectEndPointDataKeeper.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);
      _virtualObjectEndPointDataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>> ();
      _virtualObjectEndPointDataKeeperFactoryStub
          .Stub (stub => stub.Create (Arg<RelationEndPointID>.Is.Anything))
          .Return (virtualObjectEndPointDataKeeper);

      var collectionEndPointDataKeeper = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();
      collectionEndPointDataKeeper.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);
      _collectionEndPointDataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();
      _collectionEndPointDataKeeperFactoryStub
          .Stub (stub => stub.Create (Arg<RelationEndPointID>.Is.Anything))
          .Return (collectionEndPointDataKeeper);

      _factory = new RelationEndPointFactory (
          _clientTransaction,
          _endPointProviderStub,
          _lazyLoaderStub,
          _virtualObjectEndPointDataKeeperFactoryStub,
          _collectionEndPointDataKeeperFactoryStub);
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
    public void CreateVirtualObjectEndPoint_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualObjectEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<VirtualObjectEndPoint> ());
      Assert.That (endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (((VirtualObjectEndPoint) endPoint).LazyLoader, Is.SameAs (_lazyLoaderStub));
      Assert.That (((VirtualObjectEndPoint) endPoint).EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (((VirtualObjectEndPoint) endPoint).DataKeeperFactory, Is.SameAs (_virtualObjectEndPointDataKeeperFactoryStub));
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
    public void CreateCollectionEndPoint_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPoint = _factory.CreateCollectionEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<CollectionEndPoint> ());
      Assert.That (endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (((CollectionEndPoint) endPoint).LazyLoader, Is.SameAs (_lazyLoaderStub));
      Assert.That (((CollectionEndPoint) endPoint).EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (((CollectionEndPoint) endPoint).DataKeeperFactory, Is.SameAs (_collectionEndPointDataKeeperFactoryStub));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateCollectionEndPoint_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPoint = _factory.CreateCollectionEndPoint (endPointID, true);

      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<VirtualObjectEndPoint> ());
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualEndPoint (endPointID, true);

      Assert.That (endPoint, Is.TypeOf<VirtualObjectEndPoint> ());
      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void CreateVirtualEndPoint_Many_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPoint = _factory.CreateVirtualEndPoint (endPointID, false);

      Assert.That (endPoint, Is.TypeOf<CollectionEndPoint> ());
      Assert.That (endPoint.ID, Is.EqualTo (endPointID));
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateVirtualEndPoint_Many_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPoint = _factory.CreateVirtualEndPoint (endPointID, true);

      Assert.That (endPoint, Is.TypeOf<CollectionEndPoint> ());
      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void Serialization ()
    {
      var factory = new RelationEndPointFactory (
          _clientTransaction,
          new SerializableRelationEndPointProviderFake(),
          new SerializableLazyLoaderFake(),
          new SerializableVirtualObjectEndPointDataKeeperFactoryFake(),
          new SerializableCollectionEndPointDataKeeperFactoryFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (factory);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedInstance.LazyLoader, Is.Not.Null);
      Assert.That (deserializedInstance.VirtualObjectEndPointDataKeeperFactory, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionEndPointDataKeeperFactory, Is.Not.Null);
    }
  }
}