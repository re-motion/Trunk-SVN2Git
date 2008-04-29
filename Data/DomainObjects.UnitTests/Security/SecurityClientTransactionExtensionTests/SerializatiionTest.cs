using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Security.SecurityClientTransactionExtensionTests
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