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
  public class BocDateTimeValueRenderer : BocRendererBase<IBocDateTimeValue>, IBocDateTimeValueRenderer
  {
    /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";
    private const string c_defaultControlWidth = "150pt";

    private readonly DateTimeFormatter _formatter = new DateTimeFormatter();

    public BocDateTimeValueRenderer (IHttpContext context, HtmlTextWriter writer, IBocDateTimeValue control)
        : base (context, writer, control)
    {
    }

    /// <summary>
    /// Renders an inline table consisting of one row with up to three cells, depending on <see cref="IBocDateTimeValue.ActualValueType"/>.
    /// The first one for the date textbox, second for the <see cref="DatePickerButton"/> and third for the time textbox.
    /// The text boxes are rendered directly, the date picker is responsible for rendering itself.
    /// </summary>
    public void Render()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddAttributesToRender (true);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (Control.IsReadOnly)
        RenderReadOnlyValue();
      else
        RenderEditModeControls();

      Writer.RenderEndTag();
    }

    private void RenderEditModeControls ()
    {
      var dateTextBox = new TextBox { ID = Control.DateTextboxID };
      Initialize (dateTextBox, Control.DateTextBoxStyle, GetDateMaxLength());
      dateTextBox.Text = Control.Value.HasValue ? _formatter.FormatDateValue (Control.Value.Value) : Control.DateString;
      Control.Controls.Add (dateTextBox);
      
      var timeTextBox = new TextBox { ID = Control.TimeTextboxID };
      Initialize (timeTextBox, Control.TimeTextBoxStyle, GetTimeMaxLength ());
      timeTextBox.Text = Control.Value.HasValue ? _formatter.FormatTimeValue (Control.Value.Value, Control.ShowSeconds) : Control.TimeString;
      Control.Controls.Add (timeTextBox);

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

      Control.Controls.Remove (dateTextBox);
      Control.Controls.Remove (timeTextBox);
    }

    private bool DetermineClientScriptLevel (IDatePickerButton datePickerButton)
    {
      if (!datePickerButton.EnableClientScript)
        return false;

      if (Control.IsDesignMode)
        return true;

      bool isVersionGreaterOrEqual55 =
          Context.Request.Browser.MajorVersion >= 6
          || Context.Request.Browser.MajorVersion == 5
             && Context.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }

    private string GetDateTextBoxSize (int dateTextBoxWidthPercentage)
    {
      string dateTextBoxSize;
      if (!Control.DateTextBoxStyle.Width.IsEmpty)
        dateTextBoxSize = Control.DateTextBoxStyle.Width.ToString();
      else
        dateTextBoxSize = dateTextBoxWidthPercentage + "%";
      return dateTextBoxSize;
    }

    private string GetTimeTextBoxSize (int dateTextBoxWidthPercentage)
    {
      string timeTextBoxSize;
      if (!Control.TimeTextBoxStyle.Width.IsEmpty)
        timeTextBoxSize = Control.TimeTextBoxStyle.Width.ToString();
      else
        timeTextBoxSize = (100 - dateTextBoxWidthPercentage) + "%";
      return timeTextBoxSize;
    }

    private int GetDateTextBoxWidthPercentage (bool hasDateField, bool hasTimeField)
    {
      int dateTextBoxWidthPercentage = 0;
      if (hasDateField && hasTimeField && Control.ShowSeconds)
        dateTextBoxWidthPercentage = 55;
      else if (hasDateField && hasTimeField)
        dateTextBoxWidthPercentage = 60;
      else if (hasDateField)
        dateTextBoxWidthPercentage = 100;
      return dateTextBoxWidthPercentage;
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

    public override string CssClassBase
    {
      get { return "bocDateTimeValue"; }
    }

    private void Initialize (TextBox textBox, SingleRowTextBoxStyle textBoxStyle, int maxLength)
    {
      textBox.Enabled = Control.Enabled;
      textBox.ReadOnly = !Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.DateTimeTextBoxStyle.ApplyStyle (textBox);
      textBoxStyle.ApplyStyle (textBox);

      if (Control.ProvideMaxLength)
        textBox.MaxLength = maxLength;
    }

    /// <summary> Calculates the maximum length for required for entering the date component. </summary>
    /// <returns> The length. </returns>
    protected virtual int GetDateMaxLength ()
    {
      DateTime date = new DateTime (2000, 12, 31);
      string maxDate = date.ToString ("d");
      return maxDate.Length;
    }

    /// <summary> Calculates the maximum length for required for entering the time component. </summary>
    /// <returns> The length. </returns>
    protected virtual int GetTimeMaxLength ()
    {
      DateTime time = new DateTime (1, 1, 1, 23, 30, 30);
      string maxTime = Control.ShowSeconds ? time.ToString ("T") : time.ToString ("t");

      return maxTime.Length;
    }

    private void RenderReadOnlyValue ()
    {
      Label label = new Label();

      if (Control.IsDesignMode && string.IsNullOrEmpty (label.Text))
      {
        label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  Control.label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        if (Control.Value.HasValue)
        {
          DateTime dateTime = Control.Value.Value;

          if (Control.ActualValueType == BocDateTimeValueType.DateTime)
            label.Text = _formatter.FormatDateTimeValue (dateTime, Control.ShowSeconds);
          else if (Control.ActualValueType == BocDateTimeValueType.Date)
            label.Text = _formatter.FormatDateValue (dateTime);
          else
            label.Text = dateTime.ToString();
        }
        else
          label.Text = "&nbsp;";
      }

      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isLabelHeightEmpty = label.Height.IsEmpty && string.IsNullOrEmpty (label.Style["height"]);
      if (!isControlHeightEmpty && isLabelHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty && string.IsNullOrEmpty (label.Style["width"]);
      if (!isControlWidthEmpty && isLabelWidthEmpty)
      {
        if (!Control.Width.IsEmpty)
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
        else
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
      }

      label.RenderControl (Writer);
    }
  }
}