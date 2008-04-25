using System;
using System.Web;
using System.Web.SessionState;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class SessionWxeFunction: WxeFunction
  {
    public SessionWxeFunction ()
    {
    }

    public SessionWxeFunction (params object[] args)
        : base (args)
    {
    }

    [WxeParameter (0, true)]
    public bool ReadOnly
    {
      get { return (bool) Variables["ReadOnly"]; }
      set { Variables["ReadOnly"] = value; }
    }

    // steps

    void Step1()
    {
    }

    class Step2: WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }

    class Step3: WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }
  }
}