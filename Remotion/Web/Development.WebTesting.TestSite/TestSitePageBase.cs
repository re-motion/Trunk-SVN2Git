using System;
using Remotion.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class TestSitePageBase : SmartPage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      HtmlHeadAppender.Current.RegisterPageStylesheetLink();
    }
  }
}