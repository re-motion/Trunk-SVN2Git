using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.WebMenuItemTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      WebConfigurationMock.Current = null;
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}