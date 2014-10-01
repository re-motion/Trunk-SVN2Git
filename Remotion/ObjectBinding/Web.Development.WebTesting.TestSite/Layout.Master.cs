using System;
using System.Web;
using System.Web.UI;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class Layout : MasterPage
  {
    protected override void OnInit (EventArgs e)
    {
      var requestUrl = Request.Url;

      var query = HttpUtility.ParseQueryString(requestUrl.Query);
      query["GuaranteeRefresh"] = Guid.NewGuid().ToString();

      RefreshButton.NavigateUrl = requestUrl.GetLeftPart(UriPartial.Path) + "?" + query;

      base.OnInit (e);
    }

    protected override void OnPreRender (EventArgs e)
    {
      HtmlHeadAppender.Current.RegisterPageStylesheetLink ();

      base.OnPreRender (e);
    }

    public TUserControl GetTestOutputControl<TUserControl>()
      where TUserControl : UserControl
    {
      return (TUserControl) testOutput.Controls[1].Controls[0];
    }
  }
}