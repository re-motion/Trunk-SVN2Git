using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullSecurityProviderTest
  {
    private ISecurityProvider _securityProvider;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new NullSecurityProvider();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new NullSecurityProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetAccess_ReturnsEmptyList ()
    {
      AccessType[] accessTypes = _securityProvider.GetAccess (null, null);
      Assert.IsNotNull (accessTypes);
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetRevision_ReturnsZero ()
    {
      Assert.AreEqual (0, _securityProvider.GetRevision());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsTrue (_securityProvider.IsNull);
    }
  }
}