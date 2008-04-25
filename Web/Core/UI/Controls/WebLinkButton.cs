using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary> A <c>LinkButton</c> using <c>&amp;</c> as access key prefix in <see cref="LinkButton.Text"/>. </summary>
/// <include file='doc\include\UI\Controls\WebLinkButton.xml' path='WebLinkButton/Class/*' />
[ToolboxData("<{0}:WebLinkButton runat=server></{0}:WebLinkButton>")]
[ToolboxItem (false)]
public class WebLinkButton : LinkButton
{
  private string _text = string.Empty;

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string accessKey;
    _text = StringUtility.NullToEmpty (Text);
    _text = SmartLabel.FormatLabelText (_text, false, out accessKey);

    if (StringUtility.IsNullOrEmpty (AccessKey))
      writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);

    base.AddAttributesToRender (writer);
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      WcagHelper.Instance.HandleError (1, this);

    if (HasControls())
      base.RenderContents (writer);
    else
      writer.Write (_text);
  }
}

}
