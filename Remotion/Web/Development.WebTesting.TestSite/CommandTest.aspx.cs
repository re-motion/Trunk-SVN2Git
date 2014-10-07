using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class CommandTest : WxePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (IsPostBack)
      {
        var controlName = Request.Params["__EVENTTARGET"];
        var control = (TestCommand) Page.FindControl (controlName);
        ((Layout) Page.Master).SetTestOutput (control.ItemID);
      }
    }
  }
}