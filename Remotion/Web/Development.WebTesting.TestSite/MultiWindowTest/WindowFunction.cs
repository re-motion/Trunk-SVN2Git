using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public class WindowFunction : WxeFunction
  {
    public WindowFunction ()
        : base (new NoneTransactionMode())
    {
    }

    // Steps
    private WxeStep Step1 = new WxePageStep ("MultiWindowTest/Window.aspx");
  }
}