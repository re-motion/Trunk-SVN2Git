using System;
using Remotion.Web.ExecutionEngine;

namespace OBWTest
{

[Serializable]
public class CompleteBocTestMainWxeFunction: WxeFunction
{
  public CompleteBocTestMainWxeFunction ()
  {
    ReturnUrl = "StartForm.aspx";
    Variables["id"] = new Guid(0,0,0,0,0,0,0,0,0,0,1).ToString();
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("CompleteBocTestForm.aspx");
  private WxeStep Step2 = new WxePageStep ("CompleteBocTestUserControlForm.aspx");
  private WxeStep Step3 = new WxePageStep ("PersonDetailsForm.aspx");
}

}
