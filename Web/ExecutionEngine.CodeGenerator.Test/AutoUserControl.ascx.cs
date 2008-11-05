using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  // <WxePageFunction pageType="Test.AutoUserControl" aspxFile="AutoUserControl.ascx" functionBaseType="WxeFunction">
  //   <Parameter name="InArg" type="String" required="true" />
  //   <Parameter name="InOutArg" type="String" required="true" direction="InOut" />
  //   <ReturnValue name="OutArg" type="String"/>
  //   <Variable name="Suffix" type="String" />
  // </WxePageFunction>
  public partial class AutoUserControl : WxeUserControl
  {
    protected void Page_Load (object sender, EventArgs e)
    {

    }
  }
}