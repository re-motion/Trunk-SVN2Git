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
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.StandardMode
{
  public class BocDateTimeValueRenderer : BocDateTimeValueRendererBase
  {
    public BocDateTimeValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control)
        : base(context, writer, control)
    {
    }

    protected override void AddAdditionalAttributes ()
    {
      
    }

    protected override void RenderEditModeControls ()
    {
      var dateTextBox = new TextBox { ID = Control.DateTextboxID, CssClass="Date" };
      Initialize (dateTextBox, Control.DateTextBoxStyle, GetDateMaxLength ());
      dateTextBox.Text = Control.Value.HasValue ? Formatter.FormatDateValue (Control.Value.Value) : Control.DateString;
      dateTextBox.Page = Control.Page.WrappedInstance;

      var timeTextBox = new TextBox { ID = Control.TimeTextboxID, CssClass = "Time" };
      Initialize (timeTextBox, Control.TimeTextBoxStyle, GetTimeMaxLength ());
      timeTextBox.Text = Control.Value.HasValue ? Formatter.FormatTimeValue (Control.Value.Value, Control.ShowSeconds) : Control.TimeString;
      timeTextBox.Page = Control.Page.WrappedInstance;

      var datePickerButton = Control.DatePickerButton;
      datePickerButton.AlternateText = Control.GetDatePickerText ();
      datePickerButton.IsDesignMode = Control.IsDesignMode;

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
      bool canScript = DetermineClientScriptLevel (datePickerButton);
      bool hasDatePicker = hasDateField && canScript;

      int dateTextBoxWidthPercentage = GetDateTextBoxWidthPercentage (hasDateField, hasTimeField);
      string dateTextBoxSize = GetDateTextBoxSize (dateTextBoxWidthPercentage - 5);
      string timeTextBoxSize = GetTimeTextBoxSize (95 - dateTextBoxWidthPercentage);
      dateTextBox.Style["width"] = dateTextBoxSize;
      timeTextBox.Style["width"] = timeTextBoxSize;

      if (hasDateField)
      {
        dateTextBox.RenderControl (Writer);
      }

      if (hasDatePicker)
      {
        datePickerButton.RenderControl (Writer);
      }

      if (hasTimeField)
      {
        timeTextBox.RenderControl (Writer);
      }
    }
  }
}