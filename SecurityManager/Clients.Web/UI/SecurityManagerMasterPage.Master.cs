using System;
using System.Web.UI;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class SecurityManagerMasterPage : MasterPage
  {
    private const string c_contentViewStyleFileUrl = "ContentViewStyle.css";
    private const string c_contentViewStyleFileKey = "SecurityManagerContentViewStyle";

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!HtmlHeadAppender.Current.IsRegistered (c_contentViewStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            this, typeof (SecurityManagerMasterPage), ResourceType.Html, c_contentViewStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_contentViewStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}
