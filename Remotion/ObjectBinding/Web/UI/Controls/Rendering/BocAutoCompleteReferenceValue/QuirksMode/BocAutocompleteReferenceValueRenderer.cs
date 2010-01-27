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
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocAutoCompleteReferenceValueRenderer : BocRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueRenderer
  {
    private const string c_nullIdentifier = "==null==";
    private const string c_defaultControlWidth = "150pt";

    public BocAutoCompleteReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control)
        : this (context, writer, control, null)
    {
    }

    public BocAutoCompleteReferenceValueRenderer (
        IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control, Func<TextBox> textBoxGetter)
        : base (context, writer, control)
    {
      if (textBoxGetter != null)
        TextBoxGetter = textBoxGetter;
      else
        TextBoxGetter = () => new TextBox();
    }

    private Func<TextBox> TextBoxGetter { get; set; }

    public void Render ()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TextBox textBox = GetTextbox();
      textBox.Page = Control.Page.WrappedInstance;
      HiddenField hiddenField = GetHiddenField();
      Label label = GetLabel();
      Image icon = GetIcon();

      if (Control.HasValueEmbeddedInsideOptionsMenu == true && Control.HasOptionsMenu
          || Control.HasValueEmbeddedInsideOptionsMenu == null && Control.IsReadOnly && Control.HasOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (textBox, hiddenField, label);
      else
        RenderContentsWithSeparateOptionsMenu (textBox, hiddenField, label, icon);

      Writer.RenderEndTag();
    }

    private TextBox GetTextbox ()
    {
      var textbox = TextBoxGetter();
      textbox.ID = Control.TextBoxUniqueID;
      textbox.EnableViewState = false;
      textbox.Text = Control.BusinessObjectDisplayName;

      textbox.Enabled = Control.Enabled;
      textbox.Height = Unit.Empty;
      textbox.Width = Unit.Empty;
      textbox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textbox);

      return textbox;
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
      label.Text = Control.GetLabelText();
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
            {
              if (Control.Value == null)
                icon.AlternateText = String.Empty;
              else
                icon.AlternateText = HttpUtility.HtmlEncode (Control.Value.DisplayNameSafe);
            }
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddStyleAttribute ("display", "inline");
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

    private void RenderContentsWithSeparateOptionsMenu (TextBox textBox, HiddenField hiddenField, Label label, Image icon)
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
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isTextboxWidthEmpty)
        {
          if (isControlWidthEmpty)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      Writer.AddStyleAttribute ("display", "inline");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = StringUtility.NullToEmpty (Control.BusinessObjectUniqueIdentifier);

      if (isReadOnly)
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (textBox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        Writer.AddStyleAttribute ("padding-left", "0.3em");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //Writer.AddAttribute ("align", "right");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (Writer);
        Writer.RenderEndTag(); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && Context.Request.Browser.Browser == "Opera")
      {
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        Writer.Write ("&nbsp;");
        Writer.RenderEndTag(); // End td
      }

      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    private void RenderContentsWithIntegratedOptionsMenu (TextBox textBox, HiddenField hiddenField, Label label)
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
      Control.OptionsMenu.RenderControl (Writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle ()
    {
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
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          Writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        RenderEditModeValue (textbox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }
    }

    private void RenderSeparateIcon (Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      Writer.AddStyleAttribute ("padding-right", "0.3em");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      icon.RenderControl (Writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);

      Writer.RenderEndTag(); //  End td
    }

    private void RenderReadOnlyValue (Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (isCommandEnabled)
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
      if (icon.Visible)
      {
        icon.RenderControl (Writer);
        Writer.Write ("&nbsp;");
      }
      label.RenderControl (Writer);
      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);

      Writer.RenderEndTag(); //  End td
    }

    private void RenderEditModeValue (
        TextBox textBox,
        HiddenField hiddenField,
        bool isControlHeightEmpty,
        bool isDropDownListHeightEmpty,
        bool isDropDownListWidthEmpty)
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      textBox.RenderControl (Writer);

      hiddenField.RenderControl (Writer);

      Writer.RenderEndTag(); //  End td

      RenderEditModeValueExtension();
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension ()
    {
      if( !Control.Enabled )
        return;

      Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderDropdownButton();
      Writer.RenderEndTag ();
    }

    private void RenderDropdownButton ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      string imgUrl = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocAutoCompleteReferenceValue), ResourceType.Image, ResourceTheme, "DropDownMenuArrow.gif");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url('{0}')", imgUrl));
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (Writer);
      Writer.RenderEndTag ();
    }

    protected string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }
  }
}
