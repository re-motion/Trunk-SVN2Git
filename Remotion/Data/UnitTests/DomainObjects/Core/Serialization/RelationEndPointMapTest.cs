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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPointMap' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager.RelationEndPointMap);
    }

    [Test]
    public void RelationEndPointMapIsFlattenedSerializable ()
    {
      RelationEndPointMap map = ClientTransactionMock.DataManager.RelationEndPointMap;

      RelationEndPointMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.That (deserializedMap, Is.Not.Null);
      Assert.That (deserializedMap, Is.Not.SameAs (map));
    }

    [Test]
    public void RelationEndPointMap_Content ()
    {
      RelationEndPointMap map = ClientTransactionMock.DataManager.RelationEndPointMap;
      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      Assert.That (map.Count, Is.EqualTo (5));

      var deserializedMap = Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager).RelationEndPointMap;

      Assert.That (deserializedMap.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedMap.ClientTransaction, Is.InstanceOfType (typeof (ClientTransactionMock)));
      Assert.That (deserializedMap.ClientTransaction, Is.Not.SameAs (ClientTransactionMock));

      Assert.That (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"), 
          Is.SameAs (PrivateInvoke.GetNonPublicProperty (deserializedMap.ClientTransaction, "TransactionEventSink")));
      
      Assert.That (deserializedMap.CollectionEndPointChangeDetectionStrategy, Is.Not.Null);
      Assert.That (deserializedMap.CollectionEndPointChangeDetectionStrategy, Is.InstanceOfType (typeof (RootCollectionEndPointChangeDetectionStrategy)));

      Assert.That (deserializedMap.Count, Is.EqualTo (5));

      var collectionEndPoint = (CollectionEndPoint)
          deserializedMap[new RelationEndPointID (DomainObjectIDs.Order1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"))];

      Assert.That (collectionEndPoint.ClientTransaction, Is.SameAs (deserializedMap.ClientTransaction));
    }
  }
}
