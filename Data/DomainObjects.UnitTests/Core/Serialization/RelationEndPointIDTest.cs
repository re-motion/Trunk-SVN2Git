using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  [TestFixture]
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private RelationEndPointID _id;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _id = new RelationEndPointID (DomainObjectIDs.Computer1, ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPointID' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointIDIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_id);
    }

    [Test]
    public void RelationEndPointIDIsFlattenedSerializable ()
    {
      RelationEndPointID deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_id);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_id, deserializedEndPoint);
    }

    [Test]
    public void RelationEndPointID_Content ()
    {
      RelationEndPointID deserializedID = FlattenedSerializer.SerializeAndDeserialize (_id);
      Assert.AreEqual (DomainObjectIDs.Computer1, deserializedID.ObjectID);
      Assert.AreSame (_id.Definition, deserializedID.Definition);
    }
  }
}