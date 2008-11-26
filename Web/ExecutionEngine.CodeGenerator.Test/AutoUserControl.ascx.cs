using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  // <WxeFunction codeBehindType="Test.AutoUserControl" markupFile="AutoUserControl.ascx" functionBaseType="WxeFunction" mode="UserControl">
  //   <Parameter name="InArg" type="String" />
  //   <Parameter name="InOutArg" type="String" direction="InOut" />
  //   <ReturnValue type="String"/>
  //   <Variable name="Suffix" type="String" />
  // </WxeFunction>
  public partial class AutoUserControl : WxeUserControl
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      IsPostBackLabel.Text = IsUserControlPostBack.ToString();
      string inOutParam = InOutArgField.Text + Suffix;
      InOutArgField.Text = inOutParam;
    }
    protected void Button1_Click (object sender, EventArgs e)
    {
      ReturnValue = "thank you";
      Return ();
    }
  }
}