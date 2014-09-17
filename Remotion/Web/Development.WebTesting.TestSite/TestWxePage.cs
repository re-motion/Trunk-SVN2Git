using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class TestWxePage : WxePage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      HtmlHeadAppender.Current.RegisterPageStylesheetLink();
    }
  }
}