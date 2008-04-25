using System;
using Remotion.ObjectBinding.Sample;
using Remotion.Web.ExecutionEngine;

namespace OBWTest
{

[Serializable]
public class TestTabbedFormWxeFunction: WxeFunction
{
  public TestTabbedFormWxeFunction ()
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
    ReturnUrl = "StartForm.aspx";
  }

  public TestTabbedFormWxeFunction (params object[] parameters)
    : base (parameters)
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
  }

//  public TestTabbedFormWxeFunction (object Object, object ReadOnly, object Action)
//    : base (Object, ReadOnly, Action)
//  {
//  }

  public TestTabbedFormWxeFunction (object ReadOnly)
    : base (ReadOnly)
  {
  }

  // parameters

  public BindableXmlObject Object 
  {
    get { return (BindableXmlObject) Variables["Object"]; }
    set { Variables["Object"] = value; }
  }

  [WxeParameter (1, true)]
  public bool ReadOnly
  {
    get { return (bool) Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

//  [WxeParameter (3, false)]
//  public CnObject Action
//  {
//    get { return (CnObject) Variables["Action"]; }
//    set { Variables["Action"] = value; }
//  }

  // steps

  [Serializable]
  class Step1: WxeStepList
  {
    TestTabbedFormWxeFunction Function { get { return (TestTabbedFormWxeFunction) ParentFunction; } }

    WxeStep Step1_ = new WxePageStep ("TestTabbedForm.aspx");
  }
}

}
