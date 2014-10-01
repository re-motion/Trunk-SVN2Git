using System;
using System.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class Layout : System.Web.UI.MasterPage
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

    public void SetBOUINormal (string uniqueIdentifier)
    {
      BOUINormalLabel.Text = uniqueIdentifier;
    }

    public void SetBOUINoAutoPostBack (string uniqueIdentifier)
    {
      BOUINoAutoPostBackLabel.Text = uniqueIdentifier;
    }

    public void SetActionPerformed (string action, string parameter, string sender)
    {
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
      ActionPerformedSenderLabel.Text = sender;
    }
  }
}