using System;
using JetBrains.Annotations;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  [UsedImplicitly]
  public class MainFunction : WxeFunction
  {
    public MainFunction ()
        : base (new NoneTransactionMode())
    {
    }

    // Steps
    private void Step1 ()
    {
      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private WxeStep Step2 = new WxePageStep ("MultiWindowTest/Main.aspx");
  }
}