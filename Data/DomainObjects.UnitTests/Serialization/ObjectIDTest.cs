using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [Test]
    public void ObjectIDIsSerializable ()
    {
      ObjectID id = Serializer.SerializeAndDeserialize (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, id);
    }

    [Test]
    public void ObjectIDIsFlattenedSerializable ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserializedID = FlattenedSerializer.SerializeAndDeserialize (id);

      Assert.IsNotNull (deserializedID);
    }

    [Test]
    public void DeserializedContent_Value ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserializedID = FlattenedSerializer.SerializeAndDeserialize (id);
     
      Assert.AreEqual (id.Value, deserializedID.Value);
    }

    [Test]
    public void DeserializedContent_ClassDefinition ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserializedID = FlattenedSerializer.SerializeAndDeserialize (id);

      Assert.AreEqual (id.ClassDefinition, deserializedID.ClassDefinition);
    }
  }
}