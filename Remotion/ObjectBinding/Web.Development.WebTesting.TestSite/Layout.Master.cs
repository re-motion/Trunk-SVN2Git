using System;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class Layout : System.Web.UI.MasterPage
  {
    protected override void OnInit (EventArgs e)
    {
      var requestUrl = Context.Request.Url;
      var requestUrlWithoutQueryString = requestUrl.GetLeftPart (UriPartial.Path);
      RefreshButton.NavigateUrl = string.Format ("{0}?Garbage={1}", requestUrlWithoutQueryString, Guid.NewGuid());

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