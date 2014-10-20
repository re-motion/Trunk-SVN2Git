using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class TabbedMenuTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyTabbedMenu.EventCommandClick += MyTabbedMenuOnEventCommandClick;
    }

    private void MyTabbedMenuOnEventCommandClick (object sender, MenuTabClickEventArgs menuTabClickEventArgs)
    {
      ((Layout) Master).SetTestOutput (menuTabClickEventArgs.Tab.ItemID + "|" + menuTabClickEventArgs.Command.Type);
    }
  }
}