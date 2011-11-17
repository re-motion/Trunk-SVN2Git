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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private CollectionEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      _endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (endPointID);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.CollectionEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void CollectionEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void CollectionEndPointIsFlattenedSerializable ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void CollectionEndPoint_Content ()
    {
      _endPoint.Collection.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem5));

      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);

      Assert.AreEqual (3, deserializedEndPoint.Collection.Count);
      Assert.IsTrue (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem5));
      Assert.IsFalse (deserializedEndPoint.Collection.IsReadOnly);

      Assert.AreEqual (2, deserializedEndPoint.GetCollectionWithOriginalData().Count);
      Assert.IsTrue (deserializedEndPoint.GetCollectionWithOriginalData().Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.GetCollectionWithOriginalData().Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.GetCollectionWithOriginalData().IsReadOnly);
    }

    [Test]
    public void CollectionEndPoint_Touched ()
    {
      _endPoint.Touch ();

      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void CollectionEndPoint_Untouched ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void CollectionEndPoint_ClientTransaction ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint.ClientTransaction);
    }

    [Test]
    public void CollectionEndPoint_DelegatingDataMembers ()
    {
      _endPoint.Collection.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem5));

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (
          deserializedEndPoint.Collection,
          _endPoint.Collection.RequiredItemType,
          deserializedEndPoint.ID);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection ()
    {
      var newOpposites = _endPoint.Collection.Clone();
      _endPoint.CreateSetCollectionCommand (newOpposites).ExpandToAllRelatedObjects().NotifyAndPerform();
      
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasChanged, Is.True);

      var deserializedNewOpposites = deserializedEndPoint.Collection;
      deserializedEndPoint.Rollback ();
      
      Assert.That (deserializedEndPoint.HasChanged, Is.False);
      var deserializedOldOpposites = deserializedEndPoint.Collection;
      Assert.That (deserializedOldOpposites, Is.Not.SameAs (deserializedNewOpposites));
      Assert.That (deserializedOldOpposites, Is.Not.Null);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection_ReferenceEqualityWithOtherCollection ()
    {
      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      var oldOpposites = industrialSector.Companies;
      var newOpposites = industrialSector.Companies.Clone ();
      industrialSector.Companies = newOpposites;

      var tuple = Tuple.Create (ClientTransactionMock, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.Item1.EnterDiscardingScope())
      {
        Assert.That (deserializedTuple.Item2.Companies, Is.SameAs (deserializedTuple.Item4));
        ClientTransaction.Current.Rollback();
        Assert.That (deserializedTuple.Item2.Companies, Is.SameAs (deserializedTuple.Item3));
      }
    }

    [Test]
    public void Serialization_IntegrationWithRelationEndPointMap ()
    {
      var deserializedTransactionMock = Serializer.SerializeAndDeserialize (ClientTransactionMock);
      var deserializedCollectionEndPoint = (CollectionEndPoint) deserializedTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (_endPoint.ID);
      Assert.That (deserializedCollectionEndPoint, Is.Not.Null);
    }

    [Test]
    public void Serialization_InjectedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var originalEndPoint = new CollectionEndPoint (
          ClientTransaction.Current,
          endPointID,
          new SerializableLazyLoaderFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableCollectionEndPointDataKeeperFactoryFake());

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (originalEndPoint);

      var deserializedLoadState = PrivateInvoke.GetNonPublicField (deserializedEndPoint, "_loadState");
      Assert.That (deserializedLoadState, Is.Not.Null);
      
      Assert.That (deserializedEndPoint.LazyLoader, Is.Not.Null);
      Assert.That (deserializedEndPoint.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedEndPoint.DataKeeperFactory, Is.Not.Null);
    }
  }
}
