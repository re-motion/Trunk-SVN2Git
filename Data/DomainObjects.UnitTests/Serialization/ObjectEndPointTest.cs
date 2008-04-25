using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Computer.GetObject (DomainObjectIDs.Computer1);
      _endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Computer1, ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.ObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void ObjectEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void ObjectEndPointIsFlattenedSerializable ()
    {
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void ObjectEndPoint_Untouched_Content ()
    {
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void ObjectEndPoint_Touched_Content ()
    {
      _endPoint.OppositeObjectID = DomainObjectIDs.Employee1;
      ObjectEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Employee1, _endPoint.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Employee3, _endPoint.OriginalOppositeObjectID);
    }
  }
}