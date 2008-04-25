using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  public class TestBase
  {
    private SecurityConfiguration _configuration;

    [SetUp]
    public virtual void SetUp ()
    {
      _configuration = new SecurityConfiguration();
      SetCurrentSecurityConfiguration (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      SetCurrentSecurityConfiguration (null);
    }

    protected SecurityConfiguration Configuration
    {
      get
      {
        return _configuration;
      }
    }

    private void SetCurrentSecurityConfiguration (SecurityConfiguration configuration)
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", configuration);
    }
  }
}