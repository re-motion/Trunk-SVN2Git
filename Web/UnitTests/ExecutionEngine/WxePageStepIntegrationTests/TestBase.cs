using System;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepIntegrationTests
{
  public class TestBase
  {
    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      UrlMappingConfiguration.SetCurrent (null);
      SafeContext.Instance.SetData (typeof (WxeFunctionStateManager).AssemblyQualifiedName, null);
    }
  }
}