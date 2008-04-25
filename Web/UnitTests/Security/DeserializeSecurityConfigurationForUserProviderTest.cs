using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Configuration;
using Remotion.Web.Security;

namespace Remotion.Web.UnitTests.Security
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForUserProviderTest
  {
    [Test]
    public void Test_WithHttpContextUserProvider ()
    {
      SecurityConfiguration configuration = new SecurityConfiguration ();
      string xmlFragment = @"<remotion.security defaultUserProvider=""HttpContext"" />";
      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), configuration.UserProvider);
    }

  }
}