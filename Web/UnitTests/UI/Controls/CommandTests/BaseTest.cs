using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.AspNetFramework;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.CommandTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      WebConfigurationMock.Current = null;
      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}