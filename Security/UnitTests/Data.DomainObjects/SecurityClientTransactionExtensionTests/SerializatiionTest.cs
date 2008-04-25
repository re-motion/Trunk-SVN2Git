using System;
using NUnit.Framework;
using Remotion.Security.Data.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Data.DomainObjects.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class SerializatiionTest
  {
    [Test]
    public void Serialization ()
    {
      SecurityClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      SecurityClientTransactionExtension deserializedExtension = Serializer.SerializeAndDeserialize (extension);

      Assert.AreNotSame (extension, deserializedExtension);
    }
  }
}