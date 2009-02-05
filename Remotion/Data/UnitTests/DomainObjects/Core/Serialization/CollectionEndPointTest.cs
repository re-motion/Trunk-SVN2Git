// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
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
      _endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Order1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.CollectionEndPoint' in Assembly "
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
    public void CollectionEndPoint_Touched_Content ()
    {
      _endPoint.OppositeDomainObjects.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem5));
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);

      Assert.AreEqual (3, deserializedEndPoint.OppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem5));
      Assert.IsFalse (deserializedEndPoint.OppositeDomainObjects.IsReadOnly);
      Assert.AreSame (deserializedEndPoint, deserializedEndPoint.OppositeDomainObjects.ChangeDelegate);

      Assert.AreEqual (2, deserializedEndPoint.OriginalOppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.IsReadOnly);
      Assert.AreSame (deserializedEndPoint, deserializedEndPoint.OriginalOppositeDomainObjects.ChangeDelegate);
    }

    [Test]
    public void CollectionEndPoint_Untouched_Content ()
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
    public void CollectionEndPoint_ReplacedCollection ()
    {
      var newOpposites = _endPoint.OppositeDomainObjects.Clone();
      _endPoint.ReplaceOppositeCollection (newOpposites);
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasChanged, Is.True);

      var deserializedNewOpposites = deserializedEndPoint.OppositeDomainObjects;
      deserializedEndPoint.Rollback ();
      
      Assert.That (deserializedEndPoint.HasChanged, Is.False);
      var deserializedOldOpposites = deserializedEndPoint.OppositeDomainObjects;
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

      var tuple = Tuple.NewTuple (ClientTransactionMock, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.A.EnterDiscardingScope())
      {
        Assert.That (deserializedTuple.B.Companies, Is.SameAs (deserializedTuple.D));
        ClientTransaction.Current.Rollback();
        Assert.That (deserializedTuple.B.Companies, Is.SameAs (deserializedTuple.C));
      }
    }

    [Test]
    public void CollectionEndPoint_CollectionChangeDelegates ()
    {
      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      var oldOpposites = industrialSector.Companies;
      var newOpposites = industrialSector.Companies.Clone ();
      industrialSector.Companies = newOpposites;

      var tuple = Tuple.NewTuple (ClientTransactionMock, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.A.EnterDiscardingScope ())
      {
        var propertyName = Configuration.NameResolver.GetPropertyName (typeof (IndustrialSector), "Companies");
        var endPointID = new RelationEndPointID (industrialSector.ID, propertyName);
        var endPoint = ((ClientTransactionMock)ClientTransaction.Current).DataManager.RelationEndPointMap[endPointID];
        Assert.That (deserializedTuple.B.Companies.ChangeDelegate, Is.SameAs (endPoint));
        ClientTransaction.Current.Rollback ();
        Assert.That (deserializedTuple.B.Companies.ChangeDelegate, Is.SameAs (endPoint));
      }
    }

    [Test]
    public void Serialization_IntegrationWithRelationEndPointMap ()
    {
      Assert.That (_endPoint.ChangeDelegate, Is.SameAs (ClientTransactionMock.DataManager.RelationEndPointMap));

      var deserializedTransactionMock = Serializer.SerializeAndDeserialize (ClientTransactionMock);
      var deserializedMap = deserializedTransactionMock.DataManager.RelationEndPointMap;
      var deserializedCollectionEndPoint = (CollectionEndPoint) deserializedMap.GetRelationEndPointWithLazyLoad (_endPoint.ID);
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.SameAs (deserializedMap));
    }

    [Test]
    public void Serialization_WithCustomChangeDelegate ()
    {
      var endPoint = new CollectionEndPoint (ClientTransactionMock, _endPoint.ID, new DomainObjectCollection(), new FakeChangeDelegate ());

      var deserializedCollectionEndPoint = FlattenedSerializer.SerializeAndDeserialize (endPoint);
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.Not.Null);
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.Not.InstanceOfType (typeof (RelationEndPointMap)));
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.InstanceOfType (typeof (FakeChangeDelegate)));
    }

    [Test]
    public void Serialization_IntegrationWithRelationEndPointMap_WithCustomChangeDelegate ()
    {
      PrivateInvoke.SetNonPublicField (_endPoint, "_changeDelegate", new FakeChangeDelegate ());

      var deserializedTransactionMock = Serializer.SerializeAndDeserialize (ClientTransactionMock);
      var deserializedMap = deserializedTransactionMock.DataManager.RelationEndPointMap;
      var deserializedCollectionEndPoint = (CollectionEndPoint) deserializedMap.GetRelationEndPointWithLazyLoad (_endPoint.ID);
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.Not.SameAs (deserializedMap));
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.Not.Null);
      Assert.That (deserializedCollectionEndPoint.ChangeDelegate, Is.InstanceOfType (typeof (FakeChangeDelegate)));
    }
  }
}
