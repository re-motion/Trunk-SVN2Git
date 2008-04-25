using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      WebConfigurationMock.Current = null;
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}