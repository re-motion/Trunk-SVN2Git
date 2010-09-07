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
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

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

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude ();

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.js"));

      string scriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.js"));
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink (
          jqueryAutocompleteStyleKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueQuirksModeRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.css"),
          HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      
      RegisterBindScript();

      AddAttributesToRender (new RenderingContext<IBocAutoCompleteReferenceValue>(Context, writer, Control), false);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TextBox textBox = GetTextbox();
      textBox.Page = Control.Page.WrappedInstance;
      HiddenField hiddenField = GetHiddenField();
      Label label = GetLabel();
      Image icon = GetIcon();

      if (EmbedInOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer, textBox, label);
      else
        RenderContentsWithSeparateOptionsMenu (writer, textBox, hiddenField, label, icon);

      writer.RenderEndTag();
    }

    private void RegisterBindScript ()
    {
      string key = Control.UniqueID + "_BindScript";

      var dataSource = Maybe.ForValue (Control.DataSource);
      string businessObjectClass =
          dataSource.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (
              dataSource.Select (ds => ds.BusinessObject).Select (o => o.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (""));
      string businessObjectProperty = Maybe.ForValue (Control.Property).Select (p => p.Identifier).ValueOrDefault ("");
      string businessObjectID =
          dataSource.Select (ds => (IBusinessObjectWithIdentity) ds.BusinessObject).Select (o => o.UniqueIdentifier).ValueOrDefault ("");

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocAutoCompleteReferenceValue.Bind(");
      script.AppendFormat ("$('#{0}'), ", Control.TextBoxClientID);
      script.AppendFormat ("$('#{0}'), ", Control.HiddenFieldClientID);
      script.AppendFormat ("$('#{0}'),", Control.DropDownButtonClientID);

      script.AppendFormat ("'{0}', ", Control.ResolveClientUrl (StringUtility.NullToEmpty (Control.ServicePath)));
      script.AppendFormat ("'{0}', ", StringUtility.NullToEmpty (Control.ServiceMethod));

      script.AppendFormat ("{0}, ", Control.CompletionSetCount);
      script.AppendFormat ("{0}, ", Control.DropDownDisplayDelay);
      script.AppendFormat ("{0}, ", Control.DropDownRefreshDelay);
      script.AppendFormat ("{0}, ", Control.SelectionUpdateDelay);

      script.AppendFormat ("'{0}', ", Control.NullValueString);
      script.AppendFormat ("'{0}', ", businessObjectClass);
      script.AppendFormat ("'{0}', ", businessObjectProperty);
      script.AppendFormat ("'{0}', ", businessObjectID);
      script.AppendFormat ("'{0}'", Control.Args);
      script.Append ("); } );");

      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocAutoCompleteReferenceValue), key, script.ToString());
    }

    private TextBox GetTextbox ()
    {
      var textBox = TextBoxFactory();
      textBox.ID = Control.TextBoxUniqueID;
      textBox.EnableViewState = false;
      textBox.Text = Control.GetLabelText();

      textBox.Enabled = Control.Enabled;
      textBox.Height = Unit.Empty;
      textBox.Width = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);

      return textBox;
    }

    private HiddenField GetHiddenField ()
    {
      var hiddenField = new HiddenField();
      hiddenField.ID = Control.HiddenFieldUniqueID;
      hiddenField.Value = Control.BusinessObjectUniqueIdentifier ?? Control.NullValueString;

      return hiddenField;
    }

    private Label GetLabel ()
    {
      var label = new Label { EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (Control.GetLabelText());
      return label;
    }

    private Image GetIcon ()
    {
      var icon = new Image { EnableViewState = false, Visible = false };
      if (Control.EnableIcon && Control.Property != null)
      {
        IconInfo iconInfo = Control.GetIcon();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";

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

    private void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer, TextBox textBox, HiddenField hiddenField, Label label, Image icon)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isTextboxHeightEmpty = textBox.Height.IsEmpty
                                  && string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isTextboxWidthEmpty = textBox.Width.IsEmpty
                                 && string.IsNullOrEmpty (textBox.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isTextboxWidthEmpty)
        {
          if (isControlWidthEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (writer, textBox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        writer.AddStyleAttribute ("padding-left", "0.3em");
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //writer.AddAttribute ("align", "right");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (writer);
        writer.RenderEndTag(); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && Context.Request.Browser.Browser == "Opera")
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        writer.Write ("&nbsp;");
        writer.RenderEndTag(); // End td
      }

      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private void RenderContentsWithIntegratedOptionsMenu (HtmlTextWriter writer, TextBox textBox, Label label)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isTextBoxWidthEmpty = string.IsNullOrEmpty (textBox.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          Control.OptionsMenu.Style["width"] = "100%";
        else
          Control.OptionsMenu.Style["width"] = "0%";
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          Control.OptionsMenu.Style["height"] = "100%";

        if (isTextBoxWidthEmpty)
        {
          if (isControlWidthEmpty)
            Control.OptionsMenu.Style["width"] = c_defaultControlWidth;
          else
            Control.OptionsMenu.Style["width"] = "100%";
        }
      }

      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (RenderOptionsMenuTitle);
      Control.OptionsMenu.RenderControl (writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      var textbox = GetTextbox();
      var hiddenField = GetHiddenField();
      textbox.Page = Control.Page.WrappedInstance;
      hiddenField.Page = Control.Page.WrappedInstance;
      Image icon = GetIcon();
      Label label = GetLabel();
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textbox.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isTextboxWidthEmpty = string.IsNullOrEmpty (textbox.Style["width"]);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
      {
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        RenderEditModeValue (writer, textbox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }
    }

    private void RenderSeparateIcon (HtmlTextWriter writer, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("padding-right", "0.3em");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      icon.RenderControl (writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);

      writer.RenderEndTag(); //  End td
    }

    private void RenderReadOnlyValue (HtmlTextWriter writer, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        Control.Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (writer);
        writer.Write ("&nbsp;");
      }
      label.RenderControl (writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);

      writer.RenderEndTag(); //  End td
    }

    private void RenderEditModeValue (
        HtmlTextWriter writer,
        TextBox textBox,
        HiddenField hiddenField,
        bool isControlHeightEmpty,
        bool isDropDownListHeightEmpty,
        bool isDropDownListWidthEmpty)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      
      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl (writer);
      textBox.AutoPostBack = autoPostBack;

      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions (textBox, string.Empty);
        if (textBox.CausesValidation)
        {
          options.PerformValidation = true;
          options.ValidationGroup = textBox.ValidationGroup;
        }
        if (Control.Page.Form != null)
          options.AutoPostBack = true;
        var postBackEventReference = Control.Page.ClientScript.GetPostBackEventReference (options, true);
        writer.AddAttribute (HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
      hiddenField.RenderControl (writer);

      writer.RenderEndTag(); //  End td

      RenderEditModeValueExtension (writer);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
      if( !Control.Enabled )
        return;

      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderDropdownButton (writer);
      writer.RenderEndTag ();
    }

    private void RenderDropdownButton (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (writer, Control);
      writer.RenderEndTag ();
    }

    protected string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private bool EmbedInOptionsMenu
    {
      get
      {
        return Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
               || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu;
      }
    }
  }
}