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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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
      Assert.IsNotNull (deserializedMap);
      Assert.AreNotSame (map, deserializedMap);
    }

    [Test]
    public void RelationEndPointMap_Content ()
    {
      RelationEndPointMap map = ClientTransactionMock.DataManager.RelationEndPointMap;
      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      Assert.AreEqual (5, map.Count);

      RelationEndPointMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.AreEqual (ClientTransactionMock, PrivateInvoke.GetNonPublicField (deserializedMap, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"));
      Assert.AreEqual (5, deserializedMap.Count);

      CollectionEndPoint collectionEndPoint = (CollectionEndPoint)
          deserializedMap[new RelationEndPointID (DomainObjectIDs.Order1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"))];
      Assert.AreSame (deserializedMap, collectionEndPoint.ChangeDelegate);
    }
  }
}
