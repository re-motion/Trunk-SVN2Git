using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
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
          deserializedMap[new RelationEndPointID (DomainObjectIDs.Order1, ReflectionUtility.GetPropertyName (typeof (Order), "OrderItems"))];
      Assert.AreSame (deserializedMap, collectionEndPoint.ChangeDelegate);
    }
  }
}