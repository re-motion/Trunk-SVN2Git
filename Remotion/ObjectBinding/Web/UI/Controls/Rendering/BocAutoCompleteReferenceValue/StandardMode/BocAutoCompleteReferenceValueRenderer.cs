// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode
{
  public class BocAutoCompleteReferenceValueRenderer
      : BocRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueRenderer
  {
    private const string c_defaultControlWidth = "150pt";

    public BocAutoCompleteReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control)
        : this (context, writer, control, () => new TextBox())
    {
    }

    public BocAutoCompleteReferenceValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocAutoCompleteReferenceValue control, Func<TextBox> textBoxGetter)
        : base(context, writer, control)
    {
      TextBoxGetter = textBoxGetter;
    }

    private Func<TextBox> TextBoxGetter
    {
      get; set;
    }

    public void Render ()
    {
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TextBox textBox = GetTextBox ();
      textBox.Page = Control.Page.WrappedInstance;
      Label label = GetLabel ();
      Image icon = GetIcon ();

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      if (Control.EmbedInOptionsMenu)
      {
        RenderContentsWithIntegratedOptionsMenu (textBox, label);
      }
      else
      {
        RenderContentsWithSeparateOptionsMenu (textBox, label, icon);
      }

      Writer.RenderEndTag ();
    }

    private void RenderContentsWithSeparateOptionsMenu (TextBox textBox, Label label, Image icon)
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
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isDropDownListHeightEmpty)
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isDropDownListWidthEmpty)
        {
          if (isControlWidthEmpty)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = "";
      if (!Control.IsDesignMode)
        postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = string.Empty;
      if (Control.BusinessObjectUniqueIdentifier != null)
        objectID = Control.BusinessObjectUniqueIdentifier;

      if (isReadOnly)
      {
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, string.Empty, objectID);

        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValue (textBox);

        Writer.RenderEndTag ();
      }

      bool hasOptionsMenu = Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        Writer.RenderBeginTag (HtmlTextWriterTag.Div);

        Control.OptionsMenu.Width = Control.OptionsMenuWidth;
        Control.OptionsMenu.RenderControl (Writer);

        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag ();
    }

    protected string CssClassOptionsMenu
    {
      get { return "bocAutoCompleteReferenceValueOptionsMenu"; }
    }

    private void RenderEditModeValue (TextBox textBox)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownList);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderEditableControl (textBox);

      Writer.RenderEndTag();
    }

    

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension ()
    {
    }

    private void RenderReadOnlyValue (Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
      else
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (icon.Visible)
      {
        icon.RenderControl (Writer);
      }
      label.RenderControl (Writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);
      else
        Writer.RenderEndTag ();
    }

    private void RenderSeparateIcon (Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        Control.Command.RenderBegin (Writer, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (Control.Command.ToolTip))
          icon.ToolTip = Control.Command.ToolTip;
      }
      else
      {
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (Writer);

      if (isCommandEnabled)
        Control.Command.RenderEnd (Writer);
      else
        Writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu (TextBox textBox, Label label)
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
      Control.OptionsMenu.RenderControl (Writer);
      Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle ()
    {
      TextBox textBox = GetTextBox ();
      textBox.Page = Control.Page.WrappedInstance;

      Image icon = GetIcon ();
      Label label = GetLabel ();
      label.CssClass = CssClassReadOnly;
      bool isReadOnly = Control.IsReadOnly;

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isDropDownListHeightEmpty = string.IsNullOrEmpty (textBox.Style["height"]);

      bool isCommandEnabled = Control.IsCommandEnabled (isReadOnly);

      string argument = string.Empty;
      string postBackEvent = Control.Page.ClientScript.GetPostBackEventReference (Control, argument) + ";";
      string objectID = string.Empty;
      if (Control.BusinessObjectUniqueIdentifier != null)
        objectID = Control.BusinessObjectUniqueIdentifier;


      if (isReadOnly)
      {
        RenderReadOnlyValue (icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      }
      else
      {
        if (icon.Visible)
        {
          RenderSeparateIcon (icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        }
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        textBox.Attributes.Add ("onClick", DropDownMenu.OnHeadTitleClickScript);
        RenderEditModeValue (textBox);

        Writer.RenderEndTag ();
      }
    }


    private Label GetLabel ()
    {
      var label = new Label { ID = Control.TextBoxUniqueID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = Control.BusinessObjectDisplayName;
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

    private void RenderReadOnlyControl ()
    {
      var label = new Label { ID = Control.TextBoxUniqueID, Text = Control.BusinessObjectDisplayName };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);

      Writer.Write (HttpUtility.HtmlEncode (Control.BusinessObjectDisplayName));
    }

    private void RenderEditableControl (TextBox textBox)
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      textBox.RenderControl (Writer);
      Writer.RenderEndTag();

      if (Control.Enabled)
        RenderDropdownButton();

      RenderHiddenField();
      // RenderDummy();
    }

    private void RenderDummy ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDummy);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.WriteLine ("&nbsp;");
      Writer.RenderEndTag();
    }

    private void RenderHiddenField ()
    {
      var hiddenField = new HiddenField
                        {
                            ID = Control.HiddenFieldUniqueID,
                            Page = Control.Page.WrappedInstance,
                            EnableViewState = true,
                            Value = Control.BusinessObjectUniqueIdentifier
                        };
      hiddenField.RenderControl (Writer);
    }

    private void RenderDropdownButton ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      string imgUrl = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (IBocAutoCompleteReferenceValue), ResourceType.Image, "DropDownMenuArrow.gif");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url('{0}')", imgUrl));
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (Writer);
      Writer.RenderEndTag();
    }

    private TextBox GetTextBox ()
    {
      var textBox = TextBoxGetter();
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

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
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