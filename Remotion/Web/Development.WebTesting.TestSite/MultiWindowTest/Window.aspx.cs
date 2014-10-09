using System;
using System.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Window : Page
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      DoSomething.Click += DoSomethingOnClick;
      Close.Click += CloseOnClick;
      CloseAndRefreshParentAsWell.Click += CloseAndRefreshParentAsWellOnClick;
    }

    private void DoSomethingOnClick (object sender, EventArgs eventArgs)
    {
    }

    private void CloseOnClick (object sender, EventArgs eventArgs)
    {
    }

    private void CloseAndRefreshParentAsWellOnClick (object sender, EventArgs eventArgs)
    {
    }
  }
}