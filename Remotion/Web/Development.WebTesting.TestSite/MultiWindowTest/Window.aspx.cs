using System;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Window : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Close.Click += CloseOnClick;
      CloseAndRefreshMainAsWell.Click += CloseAndRefreshMainAsWellOnClick;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput (WindowSmartLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      Controls.Add (postBackEventHandler);
    }

    private void CloseOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
    }

    private void CloseAndRefreshMainAsWellOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
      Variables["Refresh"] = true;
    }
  }
}