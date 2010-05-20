using System;
using System.Web;
using System.Web.UI.WebControls;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Provides a common base class for Standards Mode renderers or <see cref="BocReferenceValue"/> and <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public abstract class BocReferenceValueRendererBase<TControl> : BocRendererBase<TControl>
    where TControl : IBocReferenceValueBase
  {
    protected BocReferenceValueRendererBase (HttpContextBase context, TControl control, IResourceUrlFactory resourceUrlFactory)
        : base(context, control, resourceUrlFactory)
    {
    }

    protected Image GetIcon ()
    {
      var icon = new Image { EnableViewState = false, ID = Control.IconClientID, GenerateEmptyAlternateText = true, Visible = false };
      if (Control.EnableIcon && Control.Property != null)
      {
        IconInfo iconInfo = Control.GetIcon();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.CssClass = CssClassContent;

          if (Control.IsCommandEnabled (Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
              icon.AlternateText = HttpUtility.HtmlEncode (Control.GetLabelText());
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    protected string CssClassContent
    {
      get { return "body"; }
    }
  }
}