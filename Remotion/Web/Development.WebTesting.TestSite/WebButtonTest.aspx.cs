using System;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class WebButtonTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyWebButton1Sync.Command += Command;
      MyWebButton2Async.Command += Command;
    }

    private void Command (object sender, CommandEventArgs e)
    {
      ((Layout) Master).SetTestOutput (e.CommandName);
    }
  }
}