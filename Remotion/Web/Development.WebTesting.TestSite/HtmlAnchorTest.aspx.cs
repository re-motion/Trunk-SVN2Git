using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class HtmlAnchorTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyWebLinkButton.Command += Command;
      MyAspLinkButton.Command += Command;
      MyHtmlAnchor.ServerClick += ServerClick;
    }

    private void Command (object sender, CommandEventArgs e)
    {
      ((Layout) Master).SetTestOutput (((Control) sender).ID + "|" + e.CommandName);
    }

    private void ServerClick (object sender, EventArgs eventArgs)
    {
      ((Layout) Master).SetTestOutput (((Control) sender).ID);
    }
  }
}