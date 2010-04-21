using System;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Represents a <c>script</c> element for javascript includes.
  /// </summary>
  public class JavaScriptInclude : HtmlHeadElement
  {
    private readonly IResourceUrl _resourceUrl;

    public JavaScriptInclude (IResourceUrl resourceUrl)
    {
      ArgumentUtility.CheckNotNull ("resourceUrl", resourceUrl);

      _resourceUrl = resourceUrl;
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Type, "text/javascript");
      writer.AddAttribute (HtmlTextWriterAttribute.Src, _resourceUrl.GetUrl());
      writer.RenderBeginTag (HtmlTextWriterTag.Script);
      writer.RenderEndTag ();
    }
  }
}