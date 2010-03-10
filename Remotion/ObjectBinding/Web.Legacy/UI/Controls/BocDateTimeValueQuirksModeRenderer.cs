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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Responsible for rendering <see cref="BocDateTimeValue"/> controls, but not for the included <see cref="IDatePickerButton"/>.
  /// For that, see <see cref="DatePickerButtonRenderer"/>.
  /// <seealso cref="IBocDateTimeValue"/>
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValueRenderer.xml' path='BocDateTimeValueRenderer/Class'/>
  public class BocDateTimeValueQuirksModeRenderer : BocDateTimeValueRendererBase
  {
    private const string c_defaultControlWidth = "150pt";

    public BocDateTimeValueQuirksModeRenderer (HttpContextBase context, IBocDateTimeValue control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      
      string styleKey = typeof (BocDateTimeValueQuirksModeRenderer).FullName + "_Style";
      string styleFile = ResourceUrlResolver.GetResourceUrl (
          Control, Context, typeof (BocDateTimeValueQuirksModeRenderer), ResourceType.Html, ResourceTheme.Legacy, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders an inline table consisting of one row with up to three cells, depending on <see cref="IBocDateTimeValue.ActualValueType"/>.
    /// The first one for the date textbox, second for the <see cref="DatePickerButton"/> and third for the time textbox.
    /// The text boxes are rendered directly, the date picker is responsible for rendering itself.
    /// </summary>
    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddAttributesToRender (writer, true);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (Control.IsReadOnly)
        RenderReadOnlyValue (writer);
      else
        RenderEditModeControls (writer);

      writer.RenderEndTag ();
    }

    protected override void RenderEditModeControls (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

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

      RenderTableBeginTag (writer, dateTextBox, timeTextBox); // Begin table
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin tr

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
      string timeTextBoxSize = GetTimeTextBoxSize (100 - dateTextBoxWidthPercentage);

      RenderDateCell (writer, hasDateField, dateTextBox, dateTextBoxSize);
      RenderDatePickerCell (writer, hasDatePicker, datePickerButton);

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      InsertDummyCellForOpera (writer, hasDatePicker);

      RenderTimeCell (writer, hasDateField, hasTimeField, timeTextBox, timeTextBoxSize);

      writer.RenderEndTag(); // End tr
      writer.RenderEndTag(); // End table
    }

    private void RenderTimeCell (HtmlTextWriter writer, bool hasDateField, bool hasTimeField, TextBox timeTextBox, string timeTextBoxSize)
    {
      if (!hasTimeField)
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, timeTextBoxSize);

      if (hasDateField)
        writer.AddStyleAttribute ("padding-left", "0.3em");

      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (timeTextBox))
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      timeTextBox.RenderControl (writer);

      writer.RenderEndTag(); // End td
    }

    private void RenderDatePickerCell (HtmlTextWriter writer, bool hasDatePicker, IDatePickerButton datePickerButton)
    {
      if (!hasDatePicker)
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("padding-left", "0.3em");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      datePickerButton.RenderControl (writer);
      writer.RenderEndTag(); // End td
    }

    private void InsertDummyCellForOpera (HtmlTextWriter writer, bool hasDatePicker)
    {
      if (hasDatePicker || Context.Request.Browser.Browser != "Opera")
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      writer.Write ("&nbsp;");
      writer.RenderEndTag(); // End td
    }

    private void RenderDateCell (HtmlTextWriter writer ,bool hasDateField, TextBox dateTextBox, string dateTextBoxSize)
    {
      if (!hasDateField)
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, dateTextBoxSize);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (dateTextBox))
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dateTextBox.RenderControl (writer);

      writer.RenderEndTag(); // End td
    }

    private void RenderTableBeginTag (HtmlTextWriter writer, TextBox dateTextBox, TextBox timeTextBox)
    {
      if (!IsControlHeightEmpty (Control) && IsControlHeightEmpty (dateTextBox) && IsControlHeightEmpty (timeTextBox))
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      if (IsControlWidthEmpty (dateTextBox) && IsControlWidthEmpty (timeTextBox))
      {
        if (IsControlWidthEmpty (Control))
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
        {
          if (!Control.Width.IsEmpty)
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
          else
            writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
        }
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
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

    protected override void AddAdditionalAttributes (HtmlTextWriter writer)
    {
      base.AddAdditionalAttributes (writer);
      writer.AddStyleAttribute ("display", "inline");
    }

    protected override bool DetermineClientScriptLevel (IDatePickerButton datePickerButton)
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
  }
}