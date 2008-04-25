using System;
using System.Web.UI;
using Remotion.Web.UI;

namespace Remotion.Web.Test
{
  public class TestBasePage : Page
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Remotion.Web.ResourceType.Html, "Style.css"));
      Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "fontsize080",
          Remotion.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Remotion.Web.ResourceType.Html, "FontSize080.css"));
    }
  }
}