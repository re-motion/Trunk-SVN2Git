using System;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class HtmlAnchorTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyHtmlAnchor.Command += Command;
    }

    private void Command (object sender, CommandEventArgs e)
    {
      ((Layout) Master).SetTestOutput (e.CommandName);
    }
  }
}