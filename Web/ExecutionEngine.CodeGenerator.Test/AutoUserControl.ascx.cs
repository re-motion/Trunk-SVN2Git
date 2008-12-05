using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  // <WxeFunction codeBehindType="Test.AutoUserControl" markupFile="AutoUserControl.ascx" functionBaseType="WxeFunction" mode="UserControl">
  //   <Parameter name="InArg" type="String" required="true"/>
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
      InArgField.Text = InArg + Suffix;
      string inOutParam = InOutArgField.Text + Suffix;
      InOutArgField.Text = inOutParam;
    }

    protected override void LoadViewState (object savedState)
    {
      base.LoadViewState (savedState);
    }
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render (writer);
    }
    protected void Button1_Click (object sender, EventArgs e)
    {
      ReturnValue = "thank you";
      Return ();
    }
  }
}