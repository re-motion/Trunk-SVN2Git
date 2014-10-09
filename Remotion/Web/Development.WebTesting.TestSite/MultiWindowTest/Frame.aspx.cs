using System;
using System.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Frame : Page
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      OpenNewWindowFromFrame.Click += OpenNewWindowFromFrameOnClick;
      OpenNewWindowAndFunctionFromFrame.Click += OpenNewWindowAndFunctionFromFrameOnClick;
      RefreshMainAsWell.Click += RefreshMainAsWellOnClick;
    }

    private void OpenNewWindowFromFrameOnClick (object sender, EventArgs eventArgs)
    {
    }

    private void OpenNewWindowAndFunctionFromFrameOnClick (object sender, EventArgs eventArgs)
    {
    }

    private void RefreshMainAsWellOnClick (object sender, EventArgs eventArgs)
    {
    }
  }
}