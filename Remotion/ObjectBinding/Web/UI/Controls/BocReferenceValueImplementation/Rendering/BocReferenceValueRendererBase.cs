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

      if (!string.IsNullOrEmpty (renderingContext.Control.IconServicePath))
      {
        CheckScriptManager (
            renderingContext.Control,
            "{0} '{1}' requires that the page contains a ScriptManager because the IconServicePath is set.",
            renderingContext.Control.GetType().Name,
            renderingContext.Control.ID);
      }
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

      if (!renderingContext.Control.IsIconEnabled())
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
      var commandInfo = command.GetCommandInfo (postBackEvent, new[] { "-0-" }, "", null, new NameValueCollection(), true);

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

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (renderingContext.Control.IsReadOnly)
      {
        RenderReadOnlyValue (renderingContext, postBackEvent, string.Empty, objectID);
      }
      else
      {
        RenderSeparateIcon (renderingContext, postBackEvent, string.Empty, objectID);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));
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

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (renderingContext.Control.IsReadOnly)
      {
        RenderReadOnlyValue (renderingContext, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        RenderSeparateIcon (renderingContext, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));
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

    private void RenderSeparateIcon (BocRenderingContext<TControl> renderingContext, string postBackEvent, string onClick, string objectID)
    {
      IconInfo icon = null;
      var isIconEnabled = renderingContext.Control.IsIconEnabled();
      if (isIconEnabled)
        icon = GetIcon (renderingContext);

      var anchorClass = CssClassCommand;
      if (icon != null)
        anchorClass += " " + CssClassHasIcon;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, anchorClass);

      var isCommandEnabled = isIconEnabled && IsCommandEnabled (renderingContext);
      var command = GetCommand (renderingContext, isCommandEnabled);
      command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);

      if (isIconEnabled)
        icon = icon ?? IconInfo.Spacer;

      if (icon != null)
      {
        if (!string.IsNullOrEmpty (command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
        icon.Render (renderingContext.Writer, renderingContext.Control);
      }

      renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
    }

    private void RenderReadOnlyValue (BocRenderingContext<TControl> renderingContext, string postBackEvent, string onClick, string objectID)
    {
      Label label = GetLabel (renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);

      var isCommandEnabled = IsCommandEnabled (renderingContext);
      var command = GetCommand (renderingContext, isCommandEnabled);
      command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);

      IconInfo icon = null;
      var isIconEnabled = renderingContext.Control.IsIconEnabled();
      if (isIconEnabled)
        icon = GetIcon (renderingContext);

      if (icon != null)
        icon.Render (renderingContext.Writer, renderingContext.Control);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      label.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
    }

    private bool IsCommandEnabled (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.BusinessObjectUniqueIdentifier != null && renderingContext.Control.IsCommandEnabled();
    }

    private BocCommand GetCommand (BocRenderingContext<TControl> renderingContext, bool isCommandEnabled)
    {
      var command = isCommandEnabled
                        ? renderingContext.Control.Command
                        : new BocCommand (CommandType.None) { OwnerControl = renderingContext.Control };
      command.ItemID = "Command";
      return command;
    }

    private IconInfo GetIcon (BocRenderingContext<TControl> renderingContext)
    {
      var iconInfo = renderingContext.Control.GetIcon();

      if (iconInfo != null && renderingContext.Control.IsCommandEnabled())
      {
        if (string.IsNullOrEmpty (iconInfo.AlternateText))
          iconInfo.AlternateText = renderingContext.Control.GetLabelText();
      }

      return iconInfo;
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

    private string GetCssClassInnerContent (BocRenderingContext<TControl> renderingContext)
    {
      string cssClass = CssClassInnerContent;

      if (!renderingContext.Control.HasOptionsMenu)
        cssClass += " " + CssClassWithoutOptionsMenu;
      else if (IsEmbedInOptionsMenu(renderingContext))
        cssClass += " " + CssClassEmbeddedOptionsMenu;
      else
        cssClass += " " + CssClassSeparateOptionsMenu;

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