using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class FlattenedSerializationReaderTest
  {
    [Test]
    public void ReadValue ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue());
    }

    [Test]
    public void ReadValue_MultipleTimes ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue ());
      Assert.AreEqual (2, reader.ReadValue ());
      Assert.AreEqual (3, reader.ReadValue ());
    }

    [Test]
    public void ReadPosition ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (0, reader.ReadPosition);
      reader.ReadValue();
      Assert.AreEqual (1, reader.ReadPosition);
      reader.ReadValue ();
      Assert.AreEqual (2, reader.ReadPosition);
      reader.ReadValue ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no more data in the serialization stream at position 3.")]
    public void ReadValue_TooOften ()
    {
      FlattenedSerializationReader<int> reader = new FlattenedSerializationReader<int> (new int[] { 1, 2, 3 });
      Assert.AreEqual (1, reader.ReadValue ());
      Assert.AreEqual (2, reader.ReadValue ());
      Assert.AreEqual (3, reader.ReadValue ());
      reader.ReadValue ();
    }
  }
}