using System;
using JetBrains.Annotations;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public class FrameFunction : WxeFunction
  {
    [UsedImplicitly]
    public FrameFunction ()
        : base (new NoneTransactionMode())
    {
    }

    public FrameFunction (params object[] args)
        : base (new NoneTransactionMode(), args)
    {
    }

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public bool AlwaysRefreshMain
    {
      get { return (bool) Variables["AlwaysRefreshMain"]; }
      [UsedImplicitly] set { Variables["AlwaysRefreshMain"] = value; }
    }

    // Steps
    private WxeStep Step1 = new WxePageStep ("MultiWindowTest/Frame.aspx");
  }
}