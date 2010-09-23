using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
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

    protected abstract void RenderEditModeValueWithSeparateOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext);
    protected abstract void RenderEditModeValueWithIntegratedOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext);

    public void Render (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (new RenderingContext<TControl> (renderingContext.HttpContext, renderingContext.Writer, renderingContext.Control));
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag ();
    }

    protected virtual void RenderContents (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      
      if (IsEmbedInOptionsMenu(renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext);
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext);
    }

    private void RenderContentsWithIntegratedOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate ((writer)=> RenderOptionsMenuTitle(renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    private void RenderContentsWithSeparateOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Image icon = GetIcon (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!renderingContext.Control.IsDesignMode)
        postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (renderingContext, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext, icon.Visible));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithSeparateOptionsMenu (renderingContext);

        renderingContext.Writer.RenderEndTag ();
      }

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);

        renderingContext.Writer.RenderEndTag ();
      }

      renderingContext.Writer.RenderEndTag ();
    }

    public void RenderOptionsMenuTitle (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Image icon = GetIcon (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (renderingContext, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext, icon.Visible));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithIntegratedOptionsMenu (renderingContext);

        renderingContext.Writer.RenderEndTag ();
      }
    }

    private void RenderSeparateIcon (BocReferenceValueBaseRenderingContext<TControl> renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (renderingContext.Control.Command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
      }
      else
      {
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (renderingContext.Writer);

      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
      else
        renderingContext.Writer.RenderEndTag ();
    }

    private void RenderReadOnlyValue (BocReferenceValueBaseRenderingContext<TControl> renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Label label = GetLabel (renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
      else
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (icon.Visible)
        icon.RenderControl (renderingContext.Writer);
      label.RenderControl (renderingContext.Writer);

      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
      else
        renderingContext.Writer.RenderEndTag ();
    }

    private Image GetIcon (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      var icon = new Image { EnableViewState = false, ID = renderingContext.Control.IconClientID, GenerateEmptyAlternateText = true, Visible = false };
      if (renderingContext.Control.EnableIcon && renderingContext.Control.Property != null)
      {
        IconInfo iconInfo = renderingContext.Control.GetIcon ();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;

          if (renderingContext.Control.IsCommandEnabled (renderingContext.Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
              icon.AlternateText = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    private Label GetLabel (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      var label = new Label { ID = renderingContext.Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
      return label;
    }

    private bool IsEmbedInOptionsMenu(BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
               || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly && renderingContext.Control.HasOptionsMenu;
    }

    private string GetCssClassInnerContent (BocReferenceValueBaseRenderingContext<TControl> renderingContext, bool hasIcon)
    {
      string cssClass = CssClassInnerContent;

      if (!renderingContext.Control.HasOptionsMenu)
        cssClass += " " + CssClassWithoutOptionsMenu;
      else if (IsEmbedInOptionsMenu(renderingContext))
        cssClass += " " + CssClassEmbeddedOptionsMenu;
      else
        cssClass += " " + CssClassSeparateOptionsMenu;

      if (hasIcon)
        cssClass += " " + CssClassHasIcon;

      return cssClass;
    }

    private string CssClassContent
    {
      get { return "body"; }
    }

    private string CssClassInnerContent
    {
      get { return "content"; }
    }

    private string CssClassSeparateOptionsMenu
    {
      get { return "separateOptionsMenu"; }
    }

    private string CssClassEmbeddedOptionsMenu
    {
      get { return "embeddedOptionsMenu"; }
    }

    private string CssClassWithoutOptionsMenu
    {
      get { return "withoutOptionsMenu"; }
    }

    private string CssClassOptionsMenu
    {
      get { return "optionsMenu"; }
    }

    private string CssClassCommand
    {
      get { return "command"; }
    }

    private string CssClassHasIcon
    {
      get { return "hasIcon"; }
    }
  }
}