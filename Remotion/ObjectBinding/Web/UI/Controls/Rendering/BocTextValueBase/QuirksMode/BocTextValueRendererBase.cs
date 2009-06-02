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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode
{
  public abstract class BocTextValueRendererBase<T> : RendererBase<T>
    where T: IBocTextValueBase
  {
    /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    protected const string c_designModeEmptyLabelContents = "##";

    protected const string c_defaultTextBoxWidth = "150pt";
    protected const int c_defaultColumns = 60;

    protected BocTextValueRendererBase (IHttpContext context, HtmlTextWriter writer, T control)
        : base(context, writer, control)
    {

    }

    protected void AddStandardAttributesToRender (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");

      string cssClass = string.Empty;
      if (!string.IsNullOrEmpty (Control.CssClass))
        cssClass = Control.CssClass + " ";

      if (!string.IsNullOrEmpty (Control.ControlStyle.CssClass))
        cssClass += Control.ControlStyle.CssClass;

      if( !string.IsNullOrEmpty(cssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      CssStyleCollection styles = Control.ControlStyle.GetStyleAttributes(Control);
      foreach (string style in styles.Keys)
      {
        writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in Control.Attributes.Keys)
      {
        string value = Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          writer.AddAttribute (attribute, value);
      }
    }

    protected TextBox GetTextBox ()
    {
      TextBox textBox = new TextBox { Text = Control.Text };
      textBox.ID = Control.GetTextBoxUniqueID();
      textBox.EnableViewState = false;
      textBox.Enabled = Control.Enabled;
      textBox.ReadOnly = !Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);
      if (textBox.TextMode == TextBoxMode.MultiLine && textBox.Columns < 1)
        textBox.Columns = c_defaultColumns;

      textBox.AutoPostBack = Control.AutoPostBack;
      return textBox;
    }

    protected void AddAttributesToRender (HtmlTextWriter writer)
    {
      string backUpStyleWidth = Control.Style["width"];
      bool hasStyleWidth = !StringUtility.IsNullOrEmpty (backUpStyleWidth);
      if (hasStyleWidth)
        Control.Style["width"] = null;
      
      Unit backUpWidth = Control.Width; // base.Width and base.ControlStyle.Width
      bool hasControlWidth = !backUpWidth.IsEmpty;
      if (hasControlWidth)
        Control.Width = Unit.Empty;

      bool isReadOnly = Control.IsReadOnly;
      bool isDisabled = !Control.Enabled;

      string backUpCssClass = Control.CssClass; // base.CssClass and base.ControlStyle.CssClass
      bool hasCssClass = !StringUtility.IsNullOrEmpty (backUpCssClass);
      if (hasCssClass)
        Control.CssClass += GetAdditionalCssClass (isReadOnly, isDisabled);

      string backUpAttributeCssClass = Control.Attributes["class"];
      bool hasClassAttribute = !StringUtility.IsNullOrEmpty (backUpAttributeCssClass);
      if (hasClassAttribute)
        Control.Attributes["class"] += GetAdditionalCssClass (isReadOnly, isDisabled);

      AddStandardAttributesToRender (Writer);

      if (!hasCssClass && !hasClassAttribute)
      {
        string cssClass = Control.CssClassBase + GetAdditionalCssClass(isReadOnly, isDisabled);
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      }
      
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");

      // restore original values
      Control.CssClass = backUpCssClass;
      Control.Attributes["class"] = backUpAttributeCssClass;
      Control.Style["width"] = backUpStyleWidth;
      Control.Width = backUpWidth;
    }

    private string GetAdditionalCssClass (bool isReadOnly, bool isDisabled)
    {
      string additionalCssClass = string.Empty;
      if (isReadOnly)
        additionalCssClass = " " + Control.CssClassReadOnly;
      else if (isDisabled)
        additionalCssClass = " " + Control.CssClassDisabled;
      return additionalCssClass;
    }

    public void Render ()
    {
      AddAttributesToRender (Writer);
      Writer.RenderBeginTag ("span");

      bool isControlHeightEmpty = Control.Height.IsEmpty && StringUtility.IsNullOrEmpty (Control.Style["height"]);

      string controlWidth = Control.Width.IsEmpty ? Control.Style["width"] : Control.Width.ToString();
      bool isControlWidthEmpty = string.IsNullOrEmpty (controlWidth);


      WebControl innerControl = Control.IsReadOnly ? (WebControl) GetLabel() : GetTextBox();

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && StringUtility.IsNullOrEmpty (innerControl.Style["height"]);
      bool isInnerControlWidthEmpty = innerControl.Width.IsEmpty && StringUtility.IsNullOrEmpty (innerControl.Style["width"]);

      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");


      if (isInnerControlWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          bool needsColumnCount = Control.TextBoxStyle.TextMode != TextBoxMode.MultiLine || Control.TextBoxStyle.Columns == null;
          if (!Control.IsReadOnly && needsColumnCount)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultTextBoxWidth);
        }
        else
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, controlWidth);
      }
      innerControl.RenderControl (Writer);

      Writer.RenderEndTag();
    }

    protected abstract Label GetLabel ();

    
  }
}