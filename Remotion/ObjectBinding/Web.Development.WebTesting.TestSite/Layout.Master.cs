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

      var query = HttpUtility.ParseQueryString (requestUrl.Query);
      query["GuaranteeRefresh"] = Guid.NewGuid().ToString();

      RefreshButton.NavigateUrl = requestUrl.GetLeftPart (UriPartial.Path) + "?" + query;

      base.OnInit (e);
    }

    protected override void OnPreRender (EventArgs e)
    {
      HtmlHeadAppender.Current.RegisterPageStylesheetLink();

      base.OnPreRender (e);
    }

    public UserControl GetTestOutputControl ()
    {
      return (UserControl) testOutput.Controls[1].Controls[0];
    }
  }
}