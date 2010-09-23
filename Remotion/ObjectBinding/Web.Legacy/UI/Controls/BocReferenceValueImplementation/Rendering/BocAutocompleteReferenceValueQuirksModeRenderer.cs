// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocAutoCompleteReferenceValueQuirksModeRenderer : BocQuirksModeRendererBase<IBocAutoCompleteReferenceValue>
  {
    private const string c_defaultControlWidth = "150pt";

    public BocAutoCompleteReferenceValueQuirksModeRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control)
        : this (context, control, () => new TextBox())
    {
    }

    public BocAutoCompleteReferenceValueQuirksModeRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, Func<TextBox> textBoxFactory)
        : base (context, control)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      TextBoxFactory = textBoxFactory;
    }

    private Func<TextBox> TextBoxFactory { get; set; }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterJavaScriptFiles (htmlHeadAppender, control, context);
      RegisterStylesheets (htmlHeadAppender, control, context);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude ();

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlResolver.GetResourceUrl (
              control,
              context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.js"));

      string scriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlResolver.GetResourceUrl (
              control,
              context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.js"));
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlResolver.GetResourceUrl (
              control,
              context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink (
          jqueryAutocompleteStyleKey,
          ResourceUrlResolver.GetResourceUrl (
              control,
              context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.css"),
          HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Render (new BocAutoCompleteReferenceValueRenderingContext (Context, writer, Control));
    }

    public void Render (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterBindScript (renderingContext);

      AddAttributesToRender (renderingContext, false);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TextBox textBox = GetTextbox (renderingContext);
      textBox.Page = renderingContext.Control.Page.WrappedInstance;
      HiddenField hiddenField = GetHiddenField (renderingContext);
      Label label = GetLabel (renderingContext);
      Image icon = GetIcon (renderingContext);

      if (IsEmbedInOptionsMenu(renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext, textBox, label);
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext, textBox, hiddenField, label, icon);

      renderingContext.Writer.RenderEndTag ();
    }

    private void RegisterBindScript (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      string key = renderingContext.Control.UniqueID + "_BindScript";

      var dataSource = Maybe.ForValue (renderingContext.Control.DataSource);
      string businessObjectClass =
          dataSource.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (
              dataSource.Select (ds => ds.BusinessObject).Select (o => o.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (""));
      string businessObjectProperty = Maybe.ForValue (renderingContext.Control.Property).Select (p => p.Identifier).ValueOrDefault ("");
      string businessObjectID =
          dataSource.Select (ds => (IBusinessObjectWithIdentity) ds.BusinessObject).Select (o => o.UniqueIdentifier).ValueOrDefault ("");

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocAutoCompleteReferenceValue.Bind(");
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.TextBoxClientID);
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.HiddenFieldClientID);
      script.AppendFormat ("$('#{0}'),", renderingContext.Control.DropDownButtonClientID);

      script.AppendFormat ("'{0}', ", renderingContext.Control.ResolveClientUrl (StringUtility.NullToEmpty (renderingContext.Control.ServicePath)));
      script.AppendFormat ("'{0}', ", StringUtility.NullToEmpty (renderingContext.Control.ServiceMethod));

      script.AppendFormat ("{0}, ", renderingContext.Control.CompletionSetCount);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownDisplayDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownRefreshDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.SelectionUpdateDelay);

      script.AppendFormat ("'{0}', ", renderingContext.Control.NullValueString);
      script.AppendFormat ("'{0}', ", businessObjectClass);
      script.AppendFormat ("'{0}', ", businessObjectProperty);
      script.AppendFormat ("'{0}', ", businessObjectID);
      script.AppendFormat ("'{0}'", renderingContext.Control.Args);
      script.Append ("); } );");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (IBocAutoCompleteReferenceValue), key, script.ToString ());
    }

    private TextBox GetTextbox (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var textBox = TextBoxFactory();
      textBox.ID = renderingContext.Control.TextBoxUniqueID;
      textBox.EnableViewState = false;
      textBox.Text = renderingContext.Control.GetLabelText ();

      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.Height = Unit.Empty;
      textBox.Width = Unit.Empty;
      textBox.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.TextBoxStyle.ApplyStyle (textBox);

      return textBox;
    }

    private HiddenField GetHiddenField (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var hiddenField = new HiddenField();
      hiddenField.ID = renderingContext.Control.HiddenFieldUniqueID;
      hiddenField.Value = renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString;

      return hiddenField;
    }

    private Label GetLabel (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var label = new Label { EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
      return label;
    }

    private Image GetIcon (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var icon = new Image { EnableViewState = false, Visible = false };
      if (renderingContext.Control.EnableIcon && renderingContext.Control.Property != null)
      {
        IconInfo iconInfo = renderingContext.Control.GetIcon ();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";

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

    protected override void AddAdditionalAttributes (RenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      base.AddAdditionalAttributes(renderingContext);
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
    }

    public override string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
    /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
    public virtual string CssClassContent
    {
      get { return "bocAutoCompleteReferenceValueContent"; }
    }

    private void RenderContentsWithSeparateOptionsMenu (BocAutoCompleteReferenceValueRenderingContext renderingContext, TextBox textBox, HiddenField hiddenField, Label label, Image icon)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = textBox.Height.IsEmpty
                                  && string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isTextboxWidthEmpty = textBox.Width.IsEmpty
                                 && string.IsNullOrEmpty (textBox.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isTextboxWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!renderingContext.Control.IsDesignMode)
        postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (renderingContext, textBox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddStyleAttribute ("padding-left", "0.3em");
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //writer.AddAttribute ("align", "right");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag (); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!renderingContext.Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && renderingContext.HttpContext.Request.Browser.Browser == "Opera")
      {
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        renderingContext.Writer.Write ("&nbsp;");
        renderingContext.Writer.RenderEndTag (); // End td
      }

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu (BocAutoCompleteReferenceValueRenderingContext renderingContext, TextBox textBox, Label label)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isTextBoxWidthEmpty = string.IsNullOrEmpty (textBox.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        else
          renderingContext.Control.OptionsMenu.Style["width"] = "0%";
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          renderingContext.Control.OptionsMenu.Style["height"] = "100%";

        if (isTextBoxWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Control.OptionsMenu.Style["width"] = c_defaultControlWidth;
          else
            renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        }
      }

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate ((writer)=> RenderOptionsMenuTitle(renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var textbox = GetTextbox (renderingContext);
      var hiddenField = GetHiddenField (renderingContext);
      textbox.Page = renderingContext.Control.Page.WrappedInstance;
      hiddenField.Page = renderingContext.Control.Page.WrappedInstance;
      Image icon = GetIcon (renderingContext);
      Label label = GetLabel (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textbox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isTextboxWidthEmpty = string.IsNullOrEmpty (textbox.Style["width"]);

      bool isCommandEnabled = renderingContext.Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (renderingContext.Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          renderingContext.Writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        RenderEditModeValue (renderingContext, textbox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }
    }

    private void RenderSeparateIcon (BocAutoCompleteReferenceValueRenderingContext renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.AddStyleAttribute ("padding-right", "0.3em");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (renderingContext.Control.Command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
      }
      icon.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); //  End td
    }

    private void RenderReadOnlyValue (BocAutoCompleteReferenceValueRenderingContext renderingContext, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (renderingContext.Writer);
        renderingContext.Writer.Write ("&nbsp;");
      }
      label.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag(); //  End td
    }

    private void RenderEditModeValue (
        BocAutoCompleteReferenceValueRenderingContext renderingContext,
        TextBox textBox,
        HiddenField hiddenField,
        bool isControlHeightEmpty,
        bool isDropDownListHeightEmpty,
        bool isDropDownListWidthEmpty)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      
      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl (renderingContext.Writer);
      textBox.AutoPostBack = autoPostBack;

      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions (textBox, string.Empty);
        if (textBox.CausesValidation)
        {
          options.PerformValidation = true;
          options.ValidationGroup = textBox.ValidationGroup;
        }
        if (renderingContext.Control.Page.Form != null)
          options.AutoPostBack = true;
        var postBackEventReference = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (options, true);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
      hiddenField.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag(); //  End td

      RenderEditModeValueExtension (renderingContext);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      if (!renderingContext.Control.Enabled)
        return;

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderDropdownButton (renderingContext);
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderDropdownButton (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.DropDownButtonClientID);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (renderingContext.Writer, renderingContext.Control);
      renderingContext.Writer.RenderEndTag ();
    }

    protected string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private bool IsEmbedInOptionsMenu(BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
               || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly && renderingContext.Control.HasOptionsMenu;
    }
  }
}