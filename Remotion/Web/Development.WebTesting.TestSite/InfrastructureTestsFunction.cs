﻿using System;
using JetBrains.Annotations;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  [UsedImplicitly]
  public class InfrastructureTestsFunction : WxeFunction
  {
    public InfrastructureTestsFunction ()
        : base (new NoneTransactionMode())
    {
    }
    
    // Steps
    private void Step1 ()
    {
      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private WxeStep Step2 = new WxePageStep ("InfrastructureTests.aspx");
  }
}