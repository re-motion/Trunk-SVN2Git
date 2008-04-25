using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{
  public class BaseTest : WebControlTest
  {
    [TearDown]
    public override void TearDown ()
    {
      base.TearDown ();

      WebConfigurationMock.Current = null;
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}