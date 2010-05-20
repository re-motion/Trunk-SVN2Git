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

    protected abstract void RenderEditModeValueWithSeparateOptionsMenu (HtmlTextWriter writer);
    protected abstract void RenderEditModeValueWithIntegratedOptionsMenu (HtmlTextWriter writer);

    protected void RenderContentsWithIntegratedOptionsMenu (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (RenderOptionsMenuTitle);
      Control.OptionsMenu.RenderControl (writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    protected void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Image icon = GetIcon ();
      bool isReadOnly = Control.IsReadOnly;

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);

        writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent ());
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithSeparateOptionsMenu (writer);

        writer.RenderEndTag ();
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (writer);

        writer.RenderEndTag ();
      }

      writer.RenderEndTag ();
    }

    public void RenderOptionsMenuTitle (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Image icon = GetIcon ();
      bool isReadOnly = Control.IsReadOnly;

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent ());
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithIntegratedOptionsMenu (writer);

        writer.RenderEndTag ();
      }
    }

    private void RenderSeparateIcon (HtmlTextWriter writer, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      else
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);
      else
        writer.RenderEndTag ();
    }

    private void RenderReadOnlyValue (HtmlTextWriter writer, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Label label = GetLabel ();

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
      else
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (icon.Visible)
        icon.RenderControl (writer);
      label.RenderControl (writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);
      else
        writer.RenderEndTag ();
    }

    private Image GetIcon ()
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

    private Label GetLabel ()
    {
      var label = new Label { ID = Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (Control.GetLabelText ());
      return label;
    }

    protected bool EmbedInOptionsMenu
    {
      get
      {
        return Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
               || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu;
      }
    }

    private string GetCssClassInnerContent ()
    {
      if (!Control.HasOptionsMenu)
        return CssClassInnerContent + " " + CssClassWithoutOptionsMenu;
      if (EmbedInOptionsMenu)
        return CssClassInnerContent + " " + CssClassEmbeddedOptionsMenu;
      return CssClassInnerContent + " " + CssClassSeparateOptionsMenu;
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
  }
}