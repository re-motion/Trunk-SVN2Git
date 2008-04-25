using System;
using System.Web;
using Remotion.ObjectBinding.Sample;
using Remotion.Web.ExecutionEngine;

namespace OBWTest
{

public class ClientFormWxeFunction: WxeFunction
{
  public ClientFormWxeFunction ()
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
    ReturnUrl = "javascript:window.close();";
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

  // steps

  void Step1()
  {
    HttpContext.Current.Session["key"] = 123456789;
  }

  class Step2: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }

  class Step3: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }
}

public class ClientFormClosingWxeFunction: WxeFunction
{
  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

public class ClientFormKeepAliveWxeFunction: WxeFunction
{
  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

}
