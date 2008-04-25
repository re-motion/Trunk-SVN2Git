using System;
using Remotion.Web.ExecutionEngine;

namespace OBWTest.Design
{

[Serializable]
public class DesignTestFunction: WxeFunction
{
  public DesignTestFunction ()
  {
    ReturnUrl = "StartForm.aspx";
  }

  // steps
  private WxeStep Step1 = new WxePageStep ("Design/DesignTestEnumValueForm.aspx");//Enum

  private WxeStep Step2 = new WxePageStep ("Design/DesignTestBooleanValueForm.aspx");
  private WxeStep Step3 = new WxePageStep ("Design/DesignTestCheckBoxForm.aspx");
  private WxeStep Step4 = new WxePageStep ("Design/DesignTestDateTimeValueForm.aspx");
  private WxeStep Step5 = new WxePageStep ("Design/DesignTestDateValueForm.aspx");
  private WxeStep Step6 = new WxePageStep ("Design/DesignTestEnumValueForm.aspx");
  private WxeStep Step7 = new WxePageStep ("Design/DesignTestMultilineTextValueForm.aspx");
  private WxeStep Step8 = new WxePageStep ("Design/DesignTestReferenceValueForm.aspx");
  private WxeStep Step9 = new WxePageStep ("Design/DesignTestTextValueForm.aspx");
  private WxeStep Step10 = new WxePageStep ("Design/DesignTestListForm.aspx");
  private WxeStep Step11 = new WxePageStep ("Design/DesignTestTreeViewForm.aspx");
}

}
