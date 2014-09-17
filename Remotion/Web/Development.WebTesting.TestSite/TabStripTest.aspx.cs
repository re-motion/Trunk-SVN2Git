using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class TabStripTest : TestWxePage
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
      TestOutputLabel.Text = tabStrip.ID + "/" + tabStrip.SelectedTab.ItemID;
    }
  }
}