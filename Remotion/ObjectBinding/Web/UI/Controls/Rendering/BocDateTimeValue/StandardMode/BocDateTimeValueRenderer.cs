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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.StandardMode
{
  public class BocDateTimeValueRenderer : BocDateTimeValueRendererBase
  {
    private readonly TextBox _dateTextBox;
    private readonly TextBox _timeTextBox;

    public BocDateTimeValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control)
        : this (context, writer, control, null, null)
    {
    }

    public BocDateTimeValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control, TextBox dateTextBox, TextBox timeTextBox)
        : base (context, writer, control)
    {
      _dateTextBox = dateTextBox ?? new TextBox();
      _timeTextBox = timeTextBox ?? new TextBox();
    }

    /// <summary>
    /// Renders an inline table consisting of one row with up to three cells, depending on <see cref="IBocDateTimeValue.ActualValueType"/>.
    /// The first one for the date textbox, second for the <see cref="DatePickerButton"/> and third for the time textbox.
    /// The text boxes are rendered directly, the date picker is responsible for rendering itself.
    /// </summary>
    public override void Render ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddAttributesToRender (false);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (Control.IsReadOnly)
        RenderReadOnlyValue ();
      else
        RenderEditModeControls ();

      Writer.RenderEndTag ();
    }

    protected override void AddAdditionalAttributes ()
    {
    }

    protected override void RenderEditModeControls ()
    {
      var dateTextBox = _dateTextBox;
      dateTextBox.ID = Control.DateTextboxID;
      dateTextBox.CssClass = CssClassDate;
      Initialize (dateTextBox, Control.DateTextBoxStyle, GetDateMaxLength());
      dateTextBox.Text = Control.DateString;
      dateTextBox.Page = Control.Page.WrappedInstance;

      var timeTextBox = _timeTextBox;
      timeTextBox.ID = Control.TimeTextboxID;
      timeTextBox.CssClass = CssClassTime;
      Initialize (timeTextBox, Control.TimeTextBoxStyle, GetTimeMaxLength());
      timeTextBox.Text = Control.TimeString;
      timeTextBox.Page = Control.Page.WrappedInstance;

      var datePickerButton = Control.DatePickerButton;
      datePickerButton.AlternateText = Control.GetDatePickerText();
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

      if (hasDateField)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDateInputWrapper + " " + GetPositioningCssClass(DateTimeValuePart.Date));
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        dateTextBox.RenderControl (Writer);
        Writer.RenderEndTag();
      }

      if (hasDatePicker)
      {
        datePickerButton.CssClass = GetPositioningCssClass (DateTimeValuePart.Picker);
        datePickerButton.RenderControl (Writer);
      }

      if (hasTimeField)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTimeInputWrapper + " " + GetPositioningCssClass (DateTimeValuePart.Time));
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        timeTextBox.RenderControl (Writer);
        Writer.RenderEndTag ();
      }
    }

    protected override bool DetermineClientScriptLevel (IDatePickerButton datePickerButton)
    {
      return true;
    }

    public override string CssClassBase
    {
      get
      {
        switch (Control.ActualValueType)
        {
          case BocDateTimeValueType.DateTime:
            return CssClassDateTime;
          case BocDateTimeValueType.Date:
            return CssClassDateOnly;
          default:
            return CssClassDateTime;
        }
      }
    }

    public string CssClassDateTime
    {
      get { return "bocDateTimeValue"; }
    }

    public string CssClassDateOnly
    {
      get { return "bocDateValue"; }
    }

    public string CssClassDateInputWrapper
    {
      get { return "bocDateInputWrapper"; }
    }

    public string CssClassTimeInputWrapper
    {
      get { return "bocTimeInputWrapper"; }
    }

    public string CssClassDate
    {
      get { return "bocDateTimeDate"; }
    }

    public string CssClassTime
    {
      get { return "bocDateTimeTime"; }
    }

    public string GetPositioningCssClass(DateTimeValuePart part)
    {
      return string.Format ("boc{0}{1}Hours{2}", part, Formatter.Is12HourTimeFormat() ? 12 : 24, Control.ShowSeconds ? "WithSeconds" : string.Empty);
    }

    public enum DateTimeValuePart
    {
      Date, Time, Picker
    }
  }
}