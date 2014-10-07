using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class DropDownMenuTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyDropDownMenu.EventCommandClick += MyDropDownMenuOnCommandClick;
      MyDropDownMenu.WxeFunctionCommandClick += MyDropDownMenuOnCommandClick;
    }

    private void MyDropDownMenuOnCommandClick (object sender, WebMenuItemClickEventArgs webMenuItemClickEventArgs)
    {
      ((Layout) Master).SetTestOutput (webMenuItemClickEventArgs.Item.ItemID + "|" + webMenuItemClickEventArgs.Command.Type);
    }
  }
}