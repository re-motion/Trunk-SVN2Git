using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class TabStripTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyTabStrip1.SelectedIndexChanged += MyTabStripOnSelectedIndexChanged;
      MyTabStrip2.SelectedIndexChanged += MyTabStripOnSelectedIndexChanged;
    }

    private void MyTabStripOnSelectedIndexChanged (object sender, EventArgs eventArgs)
    {
      var tabStrip = (WebTabStrip) sender;

      var testOutput = tabStrip.ID + "/" + tabStrip.SelectedTab.ItemID;
      ((Layout)Master).SetTestOutput(testOutput);
    }
  }
}