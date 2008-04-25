using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class FlattenedSerializationWriterTest
  {
    [Test]
    public void InitialWriter ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      int[] data = writer.GetData();
      Assert.IsNotNull (data);
      Assert.IsEmpty (data);
    }

    [Test]
    public void AddSimpleValue ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1 }));
    }

    [Test]
    public void AddSimpleValue_Twice ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int> ();
      writer.AddSimpleValue (1);
      writer.AddSimpleValue (2);
      int[] data = writer.GetData ();
      Assert.IsNotNull (data);
      Assert.That (data, Is.EqualTo (new int[] { 1, 2 }));
    }
  }
}