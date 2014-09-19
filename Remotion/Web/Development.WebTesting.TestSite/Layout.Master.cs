using System;
using System.Web.UI;
using Remotion.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class Layout : MasterPage
  {
    protected override void OnPreRender (EventArgs e)
    {
      HtmlHeadAppender.Current.RegisterPageStylesheetLink();

      base.OnPreRender (e);
    }

    public void SetTestOutput (string testOutput)
    {
      TestOutputLabel.Text = testOutput;
    }
  }
}