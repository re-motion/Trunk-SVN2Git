using System;
using JetBrains.Annotations;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  [UsedImplicitly]
  public class WebButtonTestFunction : WxeFunction
  {
    public WebButtonTestFunction ()
        : base (new NoneTransactionMode())
    {
    }
    
    // Steps
    private WxeStep Step1 = new WxePageStep ("WebButtonTest.aspx");
  }
}