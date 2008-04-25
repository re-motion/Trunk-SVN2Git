using System;
using System.Security.Principal;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullIdentityTest
  {
    private IIdentity _identity;

    [SetUp]
    public void SetUp()
    {
      _identity = new NullIdentity();
    }

    [Test]
    public void GetName()
    {
      Assert.AreEqual (string.Empty, _identity.Name);
    }

    [Test]
    public void GetIsAuthenticated()
    {
      Assert.IsFalse (_identity.IsAuthenticated);
    }

    [Test]
    public void GetAuthenticationType()
    {
      Assert.AreEqual (string.Empty, _identity.AuthenticationType);
    }
  }
}