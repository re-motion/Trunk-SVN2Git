using System;
using Remotion.ObjectBinding;
using Remotion.Web.ExecutionEngine;

namespace OBWTest
{

[Serializable]
public class ViewPersonsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("objects", true, WxeParameterDirection.In, typeof (IBusinessObject[]))};

  public ViewPersonsWxeFunction()
  {
  }

  // parameters and local variables
  public override WxeParameterDeclaration[] ParameterDeclarations
  {
    get { return s_parameters; }
  }

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public IBusinessObject[] Objects
  {
    get { return (IBusinessObject[]) Variables["objects"]; }
    set { Variables["objects"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonsForm.aspx");
}
}
