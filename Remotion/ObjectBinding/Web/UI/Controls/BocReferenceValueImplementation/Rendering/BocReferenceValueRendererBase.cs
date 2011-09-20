using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Provides a common base class for Standards Mode renderers or <see cref="BocReferenceValue"/> and <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public abstract class BocReferenceValueRendererBase<TControl> : BocRendererBase<TControl>
    where TControl : IBocReferenceValueBase
  {
    protected BocReferenceValueRendererBase (IResourceUrlFactory resourceUrlFactory)
        : base(resourceUrlFactory)
    {
    }

    protected abstract void RenderEditModeValueWithSeparateOptionsMenu (BocRenderingContext<TControl> renderingContext);
    protected abstract void RenderEditModeValueWithIntegratedOptionsMenu (BocRenderingContext<TControl> renderingContext);

    protected virtual void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string scriptKey = typeof (BocReferenceValueRendererBase<>).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueRendererBase<>), ResourceType.Html, "BocReferenceValueBase.js"));
    }

    protected void Render (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag ();

      RegisterInitializationScript (renderingContext);
    }

    private void RegisterInitializationScript (BocRenderingContext<TControl> renderingContext)
    {
      string key = typeof (BocReferenceValueRendererBase<>).FullName + "_InitializeGlobals";

      if (renderingContext.Control.Page.ClientScript.IsClientScriptBlockRegistered (typeof (BocReferenceValueRendererBase<>), key))
        return;

      var nullIconUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (IControl), ResourceType.Image, "Spacer.gif");

      var script = new StringBuilder (1000);
      script.Append ("BocReferenceValueBase.InitializeGlobals(");
      script.AppendFormat ("'{0}'", nullIconUrl.GetUrl());
      script.Append (");");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control, typeof (BocReferenceValueRendererBase<>), key, script.ToString ());
    }

    protected string GetIconServicePath (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.EnableIcon)
        return null;

      var iconServicePath = renderingContext.Control.IconServicePath;

      if (string.IsNullOrEmpty (iconServicePath))
        return null;
      return renderingContext.Control.ResolveClientUrl (iconServicePath);
    }

    protected string GetIconContextAsJson (BusinessObjectIconWebServiceContext iconServiceContext)
    {
      if (iconServiceContext == null)
        return null;

      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("businessObjectClass : ");
      AppendStringValueOrNullToScript (jsonBuilder, iconServiceContext.BusinessObjectClass);
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString ();
    }

    protected string GetCommandInfoAsJson (BocRenderingContext<TControl> renderingContext)
    {
      var command = renderingContext.Control.Command;
      if (command == null)
        return null;

      if (command.Show == CommandShow.ReadOnly)
        return null;

      var postBackEvent = GetPostBackEvent (renderingContext);
      var commandInfo = command.GetCommandInfo (postBackEvent, new[] { "-0-" }, "", null, new NameValueCollection(), false);

      if (commandInfo == null)
        return null;

      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");

      jsonBuilder.Append ("href : ");
      string href = commandInfo.Href.Replace ("-0-", "{0}");
      AppendStringValueOrNullToScript (jsonBuilder, href);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("target : ");
      string target = commandInfo.Target;
      AppendStringValueOrNullToScript (jsonBuilder, target);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("onClick : ");
      string onClick = commandInfo.OnClick;
      AppendStringValueOrNullToScript (jsonBuilder, onClick);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("title : ");
      string title = commandInfo.Title;
      AppendStringValueOrNullToScript (jsonBuilder, title);

      jsonBuilder.Append (" }");

      return jsonBuilder.ToString ();
    }

    protected virtual void RenderContents (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      
      if (IsEmbedInOptionsMenu(renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext);
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext);
    }

    private void RenderContentsWithIntegratedOptionsMenu (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (writer => RenderOptionsMenuTitle(renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    private void RenderContentsWithSeparateOptionsMenu (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Image icon = GetIcon (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string postBackEvent = GetPostBackEvent (renderingContext);
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

    public void RenderOptionsMenuTitle (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Image icon = GetIcon (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string postBackEvent = GetPostBackEvent (renderingContext);
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

    private string GetPostBackEvent (BocRenderingContext<TControl> renderingContext)
    {
      if (renderingContext.Control.IsDesignMode)
        return "";

      string argument = string.Empty;
      return renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
    }

    private void RenderSeparateIcon (BocRenderingContext<TControl> renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
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

    private void RenderReadOnlyValue (BocRenderingContext<TControl> renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
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

    private Image GetIcon (BocRenderingContext<TControl> renderingContext)
    {
      var icon = new Image { EnableViewState = false, ID = renderingContext.Control.IconClientID, GenerateEmptyAlternateText = true, Visible = false };
      if (renderingContext.Control.EnableIcon)
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
              icon.AlternateText = renderingContext.Control.GetLabelText ();
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    private Label GetLabel (BocRenderingContext<TControl> renderingContext)
    {
      var label = new Label { ID = renderingContext.Control.LabelClientID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
      return label;
    }

    private bool IsEmbedInOptionsMenu (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
               || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly && renderingContext.Control.HasOptionsMenu;
    }

    private string GetCssClassInnerContent (BocRenderingContext<TControl> renderingContext, bool hasIcon)
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

    protected string CssClassContent
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

    protected string CssClassCommand
    {
      get { return "command"; }
    }

    private string CssClassHasIcon
    {
      get { return "hasIcon"; }
    }
  }
}