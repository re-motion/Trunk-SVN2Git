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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  public class BocAutoCompleteReferenceValueRenderer : BocRendererBase<IBocAutoCompleteReferenceValue>
  {
    private const string c_defaultControlWidth = "150pt";

    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control)
        : this (context, control, () => new TextBox())
    {
    }

    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, Func<TextBox> textBoxFactory)
        : base (context, control)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      TextBoxFactory = textBoxFactory;
    }

    private Func<TextBox> TextBoxFactory { get; set; }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      htmlHeadAppender.RegisterJQueryBgiFramesJavaScriptInclude (Control);

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.js"));

      string scriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.js"));
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              ResourceTheme,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink (
          jqueryAutocompleteStyleKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              ResourceTheme,
              "BocAutoCompleteReferenceValue.jquery.css"),
          HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterBindScript();
      RegisterAdjustPositionScript ();
      RegisterAdjustLayoutScript ();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      TextBox textBox = GetTextBox();
      textBox.Page = Control.Page.WrappedInstance;
      Label label = GetLabel();
      Image icon = GetIcon();

      if (Control.EmbedInOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer, textBox, label);
      else
        RenderContentsWithSeparateOptionsMenu (writer, textBox, label, icon);

      writer.RenderEndTag();
    }

    private void RegisterBindScript ()
    {
      string key = Control.UniqueID + "_BindScript";
      const string scriptTemplate =
          @"$(document).ready( function() {{ BocAutoCompleteReferenceValue.Bind($('#{0}'), $('#{1}'), $('#{2}'), "
          + "'{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}'); }} );";

      var dataSource = Maybe.ForValue (Control.DataSource);
      string businessObjectClass = dataSource.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault ("");
      string businessObjectProperty = Maybe.ForValue (Control.Property).Select (p => p.Identifier).ValueOrDefault ("");
      string businessObjectID =
          dataSource.Select (ds => (IBusinessObjectWithIdentity) ds.BusinessObject).Select (o => o.UniqueIdentifier).ValueOrDefault ("");

      string script = string.Format (
          scriptTemplate,
          Control.TextBoxClientID,
          Control.HiddenFieldClientID,
          Control.DropDownButtonClientID,
          string.IsNullOrEmpty (Control.ServicePath)
              ? ""
              : UrlUtility.GetAbsoluteUrl (Context, Control.ServicePath, true),
          StringUtility.NullToEmpty (Control.ServiceMethod),
          Control.CompletionSetCount.HasValue ? Control.CompletionSetCount.Value : 10,
          Control.CompletionInterval,
          Control.SuggestionInterval,
          Control.NullValueString,
          businessObjectClass,
          businessObjectProperty,
          businessObjectID,
          Control.Args
          );
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocAutoCompleteReferenceValue), key, script);
    }

    private void RegisterAdjustPositionScript ()
    {
      string key = Control.ClientID + "_AdjustPositionScript";
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValueRenderer),
          key,
          string.Format ("BocAutoCompleteReferenceValue.AdjustPosition($('#{0}'), {1});",
                         Control.ClientID,
                         Control.EmbedInOptionsMenu ? "true" : "false"));
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValueRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout ($('#{0}'));", Control.ClientID));
    }

    private void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer, TextBox textBox, Label label, Image icon)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = textBox.Height.IsEmpty
                                       && string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = textBox.Width.IsEmpty
                                      && string.IsNullOrEmpty (textBox.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

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

        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValue (writer, textBox);

        writer.RenderEndTag();
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (writer);

        writer.RenderEndTag();
      }

      writer.RenderEndTag();
    }

    protected string CssClassOptionsMenu
    {
      get { return "bocAutoCompleteReferenceValueOptionsMenu"; }
    }

    private void RenderEditModeValue (HtmlTextWriter writer, TextBox textBox)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownList);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderEditableControl (writer, textBox);

      writer.RenderEndTag();
    }


    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
    }

    private void RenderReadOnlyValue (HtmlTextWriter writer, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
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
        writer.RenderEndTag();
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
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      icon.RenderControl (writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (writer);
      else
        writer.RenderEndTag();
    }

    private void RenderContentsWithIntegratedOptionsMenu (HtmlTextWriter writer, TextBox textBox, Label label)
    {
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isDropDownListWidthEmpty = string.IsNullOrEmpty (textBox.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          Control.OptionsMenu.Style["width"] = "100%";
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          Control.OptionsMenu.Style["height"] = "100%";

        if (isDropDownListWidthEmpty)
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

      TextBox textBox = GetTextBox();
      textBox.Page = Control.Page.WrappedInstance;

      Image icon = GetIcon();
      Label label = GetLabel();
      label.CssClass = CssClassReadOnly;
      bool isReadOnly = Control.IsReadOnly;

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (writer, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (writer, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInnerContent);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);

        textBox.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (writer, textBox);

        writer.RenderEndTag();
      }
    }


    private Label GetLabel ()
    {
      var label = new Label { ID = Control.TextBoxUniqueID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = Control.GetLabelText();
      return label;
    }

    private Image GetIcon ()
    {
      var icon = new Image { EnableViewState = false, ID = Control.IconUniqueID, GenerateEmptyAlternateText = true, Visible = false };
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
          icon.CssClass = CssClassContent;

          if (Control.IsCommandEnabled (Control.IsReadOnly))
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
            {
              if (Control.Value == null)
                icon.AlternateText = String.Empty;
              else
                icon.AlternateText = HttpUtility.HtmlEncode (Control.BusinessObjectDisplayName);
            }
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    private void RenderEditableControl (HtmlTextWriter writer, TextBox textBox)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      textBox.RenderControl (writer);
      writer.RenderEndTag();

      if (Control.Enabled)
        RenderDropdownButton (writer);

      RenderHiddenField (writer);
    }

    private void RenderHiddenField (HtmlTextWriter writer)
    {
      var hiddenField = new HiddenField
                        {
                            ID = Control.HiddenFieldUniqueID,
                            Page = Control.Page.WrappedInstance,
                            EnableViewState = true,
                            Value = Control.BusinessObjectUniqueIdentifier ?? Control.NullValueString
                        };
      hiddenField.RenderControl (writer);
    }

    private void RenderDropdownButton (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      string imgUrl = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocAutoCompleteReferenceValue), ResourceType.Image, ResourceTheme, "DropDownMenuArrow.gif");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url('{0}')", imgUrl));
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (writer);
      writer.RenderEndTag();
    }

    private TextBox GetTextBox ()
    {
      var textBox = TextBoxFactory();
      textBox.ID = Control.TextBoxUniqueID;
      textBox.CssClass = CssClassInput;
      textBox.Text = Control.BusinessObjectDisplayName;
      textBox.Enabled = Control.Enabled;
      textBox.Page = Control.Page.WrappedInstance;
      textBox.EnableViewState = false;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);
      return textBox;
    }

    public string CssClassDummy
    {
      get { return "bocAutoCompleteReferenceValueDummy"; }
    }

    public string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    public override string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    public string CssClassInput
    {
      get { return "bocAutoCompleteReferenceValueInput"; }
    }

    public string CssClassContent
    {
      get { return "bocAutoCompleteReferenceValueContent"; }
    }

    public string CssClassInnerContent
    {
      get { return "content"; }
    }

    public string CssClassCommand
    {
      get { return "bocAutoCompleteReferenceValueCommand"; }
    }

    public string CssClassDropDownList
    {
      get { return "bocAutoCompleteReferenceValueDropDownList"; }
    }
  }
}