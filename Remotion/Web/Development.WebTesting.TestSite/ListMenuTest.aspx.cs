using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class ListMenuTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyListMenu.EventCommandClick += MyListMenuOnCommandClick;
      MyListMenu.WxeFunctionCommandClick += MyListMenuOnCommandClick;
    }

    private void MyListMenuOnCommandClick (object sender, WebMenuItemClickEventArgs webMenuItemClickEventArgs)
    {
      ((Layout) Master).SetTestOutput (webMenuItemClickEventArgs.Item.ItemID + "|" + webMenuItemClickEventArgs.Command.Type);
    }
  }
}