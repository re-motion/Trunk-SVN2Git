using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.DataContainerMap' in Assembly "
       + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void DataContainerMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager.DataContainerMap);
    }

    [Test]
    public void DataContainerMapIsFlattenedSerializable ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.IsNotNull (deserializedMap);
    }

    [Test]
    public void DataContainerMap_Content ()
    {
      DataContainerMap map = ClientTransactionMock.DataManager.DataContainerMap;
      Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, map.Count);

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.AreEqual (ClientTransactionMock, PrivateInvoke.GetNonPublicField (deserializedMap, "_clientTransaction"));
      Assert.IsNotNull (PrivateInvoke.GetNonPublicField (deserializedMap, "_transactionEventSink"));
      Assert.AreEqual (1, deserializedMap.Count);
    }
  }
}