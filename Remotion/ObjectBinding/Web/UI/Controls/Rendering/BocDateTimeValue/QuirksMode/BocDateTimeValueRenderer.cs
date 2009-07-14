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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocDateTimeValue;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocDateTimeValue"/> controls, but not for the included <see cref="IDatePickerButton"/>.
  /// For that, see <see cref="IDatePickerButtonRenderer"/>.
  /// <seealso cref="IBocDateTimeValue"/>
  /// </summary>
  /// <include file='doc\include\UI\Controls\Rendering\QuirksMode\BocDateTimeValueRenderer.xml' path='BocDateTimeValueRenderer/Class'/>
  public class BocDateTimeValueRenderer : BocDateTimeValueRendererBase
  {
    private const string c_defaultControlWidth = "150pt";

    public BocDateTimeValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control)
        : base (context, writer, control)
    {
    }

    protected override void RenderEditModeControls ()
    {
      var dateTextBox = new TextBox { ID = Control.DateTextboxID };
      Initialize (dateTextBox, Control.DateTextBoxStyle, GetDateMaxLength());
      dateTextBox.Text = Control.Value.HasValue ? Formatter.FormatDateValue (Control.Value.Value) : Control.DateString;
      dateTextBox.Page = Control.Page.WrappedInstance;
      
      var timeTextBox = new TextBox { ID = Control.TimeTextboxID };
      Initialize (timeTextBox, Control.TimeTextBoxStyle, GetTimeMaxLength ());
      timeTextBox.Text = Control.Value.HasValue ? Formatter.FormatTimeValue (Control.Value.Value, Control.ShowSeconds) : Control.TimeString;
      timeTextBox.Page = Control.Page.WrappedInstance;

      var datePickerButton = Control.DatePickerButton;
      datePickerButton.AlternateText = Control.GetDatePickerText();
      datePickerButton.IsDesignMode = Control.IsDesignMode;

      RenderTableBeginTag (dateTextBox, timeTextBox); // Begin table
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin tr

      bool hasDateField = false;
      bool hasTimeField = false;
      switch (Control.ActualValueType)
      {
        case BocDateTimeValueType.Date:
          hasDateField = true;
          break;
        case BocDateTimeValueType.DateTime:
        case BocDateTimeValueType.Undefined:
          hasDateField = true;
          hasTimeField = true;
          break;
      }
      bool canScript = DetermineClientScriptLevel(datePickerButton);
      bool hasDatePicker = hasDateField && canScript;

      int dateTextBoxWidthPercentage = GetDateTextBoxWidthPercentage (hasDateField, hasTimeField);
      string dateTextBoxSize = GetDateTextBoxSize (dateTextBoxWidthPercentage);
      string timeTextBoxSize = GetTimeTextBoxSize (dateTextBoxWidthPercentage);

      RenderDateCell (hasDateField, dateTextBox, dateTextBoxSize);
      RenderDatePickerCell (hasDatePicker, datePickerButton);

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      InsertDummyCellForOpera (hasDatePicker);

      RenderTimeCell (hasDateField, hasTimeField, timeTextBox, timeTextBoxSize);

      Writer.RenderEndTag(); // End tr
      Writer.RenderEndTag(); // End table
    }

    private void RenderTimeCell (bool hasDateField, bool hasTimeField, TextBox timeTextBox, string timeTextBoxSize)
    {
      if (!hasTimeField)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, timeTextBoxSize);

      if (hasDateField)
        Writer.AddStyleAttribute ("padding-left", "0.3em");

      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (timeTextBox))
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      timeTextBox.RenderControl (Writer);

      Writer.RenderEndTag(); // End td
    }

    private void RenderDatePickerCell (bool hasDatePicker, IDatePickerButton datePickerButton)
    {
      if (!hasDatePicker)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      Writer.AddStyleAttribute ("padding-left", "0.3em");
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      datePickerButton.RenderControl (Writer);
      Writer.RenderEndTag(); // End td
    }

    private void InsertDummyCellForOpera (bool hasDatePicker)
    {
      if (hasDatePicker || Context.Request.Browser.Browser != "Opera")
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      Writer.Write ("&nbsp;");
      Writer.RenderEndTag(); // End td
    }

    private void RenderDateCell (bool hasDateField, TextBox dateTextBox, string dateTextBoxSize)
    {
      if (!hasDateField)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, dateTextBoxSize);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (dateTextBox))
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dateTextBox.RenderControl (Writer);

      Writer.RenderEndTag(); // End td
    }

    private void RenderTableBeginTag (TextBox dateTextBox, TextBox timeTextBox)
    {
      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (dateTextBox) && IsControlHeightEmpty (timeTextBox))
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      if (IsControlWidthEmpty (dateTextBox) && IsControlWidthEmpty (timeTextBox))
      {
        if (IsControlWidthEmpty (Control))
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
        {
          if (!Control.Width.IsEmpty)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
          else
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
        }
      }

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      Writer.AddStyleAttribute ("display", "inline");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
    }

    private bool IsControlWidthEmpty (WebControl control)
    {
      return control.Width.IsEmpty && string.IsNullOrEmpty (control.Style["width"]);
    }

    private bool IsControlWidthEmpty (IBocDateTimeValue control)
    {
      return control.Width.IsEmpty && string.IsNullOrEmpty (control.Style["width"]);
    }

    private bool IsControlHeightEmpty (WebControl control)
    {
      return control.Height.IsEmpty && string.IsNullOrEmpty (control.Style["height"]);
    }

    private bool IsControlHeightEmpty (IBocDateTimeValue control)
    {
      return control.Height.IsEmpty && string.IsNullOrEmpty (control.Style["height"]);
    }

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddStyleAttribute ("display", "inline");
    }
  }
}