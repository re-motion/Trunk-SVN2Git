using System;
using System.Security.Principal;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullPrincipalTest
  {
    private IPrincipal _principal;

    [SetUp]
    public void SetUp()
    {
      _principal = new NullPrincipal();
    }

    [Test]
    public void IsInRole()
    {
      Assert.IsFalse (_principal.IsInRole (string.Empty));
    }

    [Test]
    public void GetIdentity()
    {
      Assert.IsInstanceOfType (typeof (NullIdentity), _principal.Identity);
    }
  }
}