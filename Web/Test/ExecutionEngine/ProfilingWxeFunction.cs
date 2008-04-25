using System;
using System.Web;
using System.Web.SessionState;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class ProfilingWxeFunction: WxeFunction
  {
    private DateTime _start;
    private DateTime _end;

    public ProfilingWxeFunction ()
    {
      ReturnUrl = "~/Start.aspx";
    }

    public ProfilingWxeFunction (params object[] args)
        : base (args)
    {
      ReturnUrl = "~/Start.aspx";
    }

    // steps

    void Step10()
    {
      _start = DateTime.Now;
    }

    WxeStep Step21 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step22 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step23 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step24 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step25 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step26 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step27 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step28 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");
    WxeStep Step29 = new WxePageStep ("~/ExecutionEngine/ProfilingForm.aspx");

    // Tracing: 100ms/start-end
    // Profiling: 1sek/start-end
    void Step30()
    {
      _end = DateTime.Now;
      TimeSpan diff = _end - _start;
      System.Diagnostics.Debug.WriteLine (string.Format ("Runtime: {0} ms", diff.Ticks / 10000));
    }
  }
}