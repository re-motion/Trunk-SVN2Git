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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CollectionEndPointDataKeeperFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;

    private CollectionEndPointDataKeeperFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _changeDetectionStrategy = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy>();

      _factory = new CollectionEndPointDataKeeperFactory (_clientTransaction, _changeDetectionStrategy);
    }

    [Test]
    public void Create ()
    {
      var relationEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Customer1, 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");

      var result = _factory.Create (relationEndPointID);

      Assert.That (result, Is.TypeOf (typeof (CollectionEndPointDataKeeper)));
      Assert.That (((CollectionEndPointDataKeeper) result).EndPointID, Is.SameAs (relationEndPointID));
      Assert.That (((CollectionEndPointDataKeeper) result).ChangeDetectionStrategy, Is.SameAs (_changeDetectionStrategy));

      var updateListener = ((ChangeCachingCollectionDataDecorator) ((CollectionEndPointDataKeeper) result).CollectionData).StateUpdateListener;
      Assert.That (updateListener, Is.TypeOf (typeof (VirtualEndPointStateUpdateListener)));
      Assert.That (((VirtualEndPointStateUpdateListener) updateListener).ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (((VirtualEndPointStateUpdateListener) updateListener).EndPointID, Is.SameAs (relationEndPointID));
    }

    [Test]
    public void Serializable ()
    {
      var changeDetectionStrategy = new SerializableCollectionEndPointChangeDetectionStrategyFake();
      var factory = new CollectionEndPointDataKeeperFactory (_clientTransaction, changeDetectionStrategy);

      var deserializedInstance = Serializer.SerializeAndDeserialize (factory);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.ChangeDetectionStrategy, Is.Not.Null);
    }
  }
}