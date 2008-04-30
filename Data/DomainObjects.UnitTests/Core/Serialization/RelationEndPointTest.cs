using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  [TestFixture]
  public class RelationEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      RelationEndPointID id = new RelationEndPointID (DomainObjectIDs.Computer1, ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));
      _endPoint = new RelationEndPointStub (ClientTransactionMock, id);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void RelationEndPointIsFlattenedSerializable ()
    {
      RelationEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void RelationEndPoint_Content ()
    {
      RelationEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (ClientTransactionMock, deserializedEndPoint.ClientTransaction);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.AreEqual (_endPoint.ID, deserializedEndPoint.ID);
    }
  }
}