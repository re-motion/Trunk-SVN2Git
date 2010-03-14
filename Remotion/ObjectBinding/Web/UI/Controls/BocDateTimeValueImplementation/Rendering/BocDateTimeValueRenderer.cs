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
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  public class BocDateTimeValueRenderer : BocRendererBase<IBocDateTimeValue>
  {
    public enum DateTimeValuePart
    {
      Date,
      Time,
      Picker
    }

    /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    private readonly DateTimeFormatter _formatter = new DateTimeFormatter();
    private readonly TextBox _dateTextBox;
    private readonly TextBox _timeTextBox;

    public BocDateTimeValueRenderer (HttpContextBase context, IBocDateTimeValue control, IResourceUrlFactory resourceUrlFactory)
      : this (context, control, resourceUrlFactory, new TextBox (), new TextBox ())
    {
    }

    public BocDateTimeValueRenderer (HttpContextBase context, IBocDateTimeValue control, IResourceUrlFactory resourceUrlFactory, TextBox dateTextBox, TextBox timeTextBox)
      : base (context, control, resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("dateTextBox", dateTextBox);
      ArgumentUtility.CheckNotNull ("timeTextBox", timeTextBox);

      _dateTextBox = dateTextBox;
      _timeTextBox = timeTextBox;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);

      string styleKey = typeof (BocDateTimeValueRenderer).FullName + "_Style";
      var styleFile = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocDateTimeValueRenderer), ResourceType.Html, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterAdjustLayoutScript();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (Control.IsReadOnly)
        RenderReadOnlyValue (writer);
      else
        RenderEditModeControls (writer);

      writer.RenderEndTag();
    }

    private void RenderEditModeControls (HtmlTextWriter writer)
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

      if (hasDateField)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDateInputWrapper + " " + GetPositioningCssClass (DateTimeValuePart.Date));
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
        dateTextBox.RenderControl (writer);
        writer.RenderEndTag();

        datePickerButton.CssClass = GetPositioningCssClass (DateTimeValuePart.Picker);
        datePickerButton.RenderControl (writer);
      }

      if (hasTimeField)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTimeInputWrapper + " " + GetPositioningCssClass (DateTimeValuePart.Time));
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
        timeTextBox.RenderControl (writer);
        writer.RenderEndTag();
      }
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocDateTimeValueRenderer),
          Guid.NewGuid().ToString(),
          string.Format ("BocBrowserCompatibility.AdjustDateTimeValueLayout ($('#{0}'));", Control.ClientID));
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

    public string GetPositioningCssClass (DateTimeValuePart part)
    {
      return string.Format ("boc{0}{1}Hours{2}", part, Formatter.Is12HourTimeFormat() ? 12 : 24, Control.ShowSeconds ? "WithSeconds" : string.Empty);
    }

    private DateTimeFormatter Formatter
    {
      get { return _formatter; }
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
    private int GetDateMaxLength ()
    {
      DateTime date = new DateTime (2000, 12, 31);
      string maxDate = date.ToString ("d");
      return maxDate.Length;
    }

    /// <summary> Calculates the maximum length for required for entering the time component. </summary>
    /// <returns> The length. </returns>
    private int GetTimeMaxLength ()
    {
      DateTime time = new DateTime (1, 1, 1, 23, 30, 30);
      string maxTime = Control.ShowSeconds ? time.ToString ("T") : time.ToString ("t");

      return maxTime.Length;
    }

    private void RenderReadOnlyValue (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

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
            label.Text = Formatter.FormatDateTimeValue (dateTime, Control.ShowSeconds);
          else if (Control.ActualValueType == BocDateTimeValueType.Date)
            label.Text = Formatter.FormatDateValue (dateTime);
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
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty && string.IsNullOrEmpty (label.Style["width"]);
      if (!isControlWidthEmpty && isLabelWidthEmpty)
      {
        if (!Control.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
      }

      label.RenderControl (writer);
    }
  }
}