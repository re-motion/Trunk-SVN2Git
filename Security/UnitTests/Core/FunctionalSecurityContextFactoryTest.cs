using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class FunctionalSecurityContextFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      ISecurityContext context = factory.CreateSecurityContext ();
      Assert.IsNotNull (context);
      Assert.AreEqual ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests", context.Class);
    }

    [Test]
    public void Serialization ()
    {
      FunctionalSecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));
      FunctionalSecurityContextFactory deserializedFactory = Serializer.SerializeAndDeserialize (factory);
      Assert.AreNotSame (factory, deserializedFactory);

      ISecurityContext context1 = factory.CreateSecurityContext ();
      ISecurityContext context2 = deserializedFactory.CreateSecurityContext ();
      Assert.AreNotSame (context1, context2);
      Assert.AreEqual (context1, context2);
    }
  }
}