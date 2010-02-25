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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.StandardMode
{
  public class BocDateTimeValueRenderer : BocDateTimeValueRendererBase
  {
    private readonly TextBox _dateTextBox;
    private readonly TextBox _timeTextBox;

    public BocDateTimeValueRenderer (HttpContextBase context, IBocDateTimeValue control)
        : this (context, control, null, null)
    {
    }

    public BocDateTimeValueRenderer (HttpContextBase context, IBocDateTimeValue control, TextBox dateTextBox, TextBox timeTextBox)
        : base (context, control)
    {
      _dateTextBox = dateTextBox ?? new TextBox();
      _timeTextBox = timeTextBox ?? new TextBox();
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      
      string styleKey = typeof (BocDateTimeValueRenderer).FullName + "_Style";
      string styleFile = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (BocDateTimeValueRenderer), ResourceType.Html, ResourceTheme, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterAdjustLayoutScript ();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (Control.IsReadOnly)
        RenderReadOnlyValue (writer);
      else
        RenderEditModeControls (writer);

      writer.RenderEndTag ();
    }

    protected override void RenderEditModeControls (HtmlTextWriter writer)
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
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDateInputWrapper + " " + GetPositioningCssClass(DateTimeValuePart.Date));
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
        dateTextBox.RenderControl (writer);
        writer.RenderEndTag();
      }

      if (hasDatePicker)
      {
        datePickerButton.CssClass = GetPositioningCssClass (DateTimeValuePart.Picker);
        datePickerButton.RenderControl (writer);
      }

      if (hasTimeField)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTimeInputWrapper + " " + GetPositioningCssClass (DateTimeValuePart.Time));
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
        timeTextBox.RenderControl (writer);
        writer.RenderEndTag ();
      }
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocDateTimeValueRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustDateTimeValueLayout ($('#{0}'));", Control.ClientID));
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
