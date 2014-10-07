using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class DropDownMenuTestFunction : WxeFunction
  {
    public DropDownMenuTestFunction ()
        : base (new NoneTransactionMode())
    {
    }
    
    // Steps
    private void Step1 ()
    {
      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private WxeStep Step2 = new WxePageStep ("DropDownMenuTest.aspx");
  }
}