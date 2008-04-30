using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  [TestFixture]
  public class ObjectDeletedExceptionTest : StandardMappingTest
  {
    [Test]
    public void Serialization ()
    {
      ObjectDeletedException exception = new ObjectDeletedException (DomainObjectIDs.Order1);

      using (MemoryStream memoryStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (memoryStream, exception);
        memoryStream.Seek (0, SeekOrigin.Begin);

        formatter = new BinaryFormatter ();

        exception = (ObjectDeletedException) formatter.Deserialize (memoryStream);

        Assert.AreEqual (DomainObjectIDs.Order1, exception.ID);
      }
    }
  }
}
