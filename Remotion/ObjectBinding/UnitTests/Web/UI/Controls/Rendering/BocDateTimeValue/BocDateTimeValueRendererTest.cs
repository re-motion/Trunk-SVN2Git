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
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocDateTimeValue
{
  [TestFixture]
  public class BocDateTimeValueRendererTest : RendererTestBase
  {
    private MockDateTimeValue _dateTimeValue;
    private ControlInvoker _invoker;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _dateTimeValue = new MockDateTimeValue();
      _dateTimeValue.DatePickerButton.ImageUrl = "~/Images/DatePickerButton.gif";
      _dateTimeValue.DatePickerButton.Text = "DatePickerButton";
      _dateTimeValue.ToolTip = "Tooltip";

      _invoker = new ControlInvoker (_dateTimeValue);
    }

    [Test]
    public void RenderUndefined ()
    {
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTime ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDate ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderUndefinedDisabled ()
    {
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateTimeDisabled ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateDisabled ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderUndefinedReadOnly ()
    {
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateTimeReadOnly ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateReadOnly ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderUndefinedWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderUndefinedWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.Enabled = false;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.ValueType = BocDateTimeValueType.Date;
      _dateTimeValue.Value = DateTime.Today;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderEmptyDateTime ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderEmptyDateTimeReadOnly ()
    {
      _dateTimeValue.ValueType = BocDateTimeValueType.DateTime;
      _dateTimeValue.SetReadOnly (true);

      _invoker.PreRenderRecursive();
      _dateTimeValue.RenderControl (Html.Writer);
      AssertDocument (true, false, false);
    }

    private void SetStyle (bool inAttributes)
    {
      if (inAttributes)
      {
        _dateTimeValue.Style["width"] = "213pt";
        _dateTimeValue.Style["height"] = "23pt";
        _dateTimeValue.Attributes["class"] = "CssClass";
      }
      else
      {
        _dateTimeValue.Width = Unit.Point (213);
        _dateTimeValue.Height = Unit.Point (23);
        _dateTimeValue.CssClass = "CssClass";
      }
    }

    private void AssertDocument (bool isReadOnly, bool isDisabled, bool withStyle)
    {
      var document = Html.GetResultDocument();
      var div = GetAssertedDiv (document, isReadOnly, isDisabled, withStyle);

      if (isReadOnly)
        AssertSpan (div);
      else
        AssertTable (div, isDisabled, withStyle);
    }

    private void AssertTable (XmlNode div, bool isDisabled, bool withStyle)
    {
      var table = GetAssertedTable (div, withStyle);
      var tr = Html.GetAssertedChildElement (table, "tr", 0);

      XmlNode dateBoxCell = GetAssertedDateBoxCell (tr);
      AssertDateTextBox (dateBoxCell, false, withStyle);

      XmlNode buttonCell = GetAssertedButtonCell (tr);
      AssertDateTimePickerButton (buttonCell, isDisabled);

      if (_dateTimeValue.ValueType != BocDateTimeValueType.Date)
      {
        XmlNode timeBoxCell = GetAssertedTimeBoxCell (tr);
        AssertTimeTextBox (timeBoxCell, isDisabled, withStyle);
      }
    }

    private void AssertSpan (XmlNode div)
    {
      var span = Html.GetAssertedChildElement (div, "span", 0);
      string formatString = "dd.MM.yyyy";
      if (_dateTimeValue.ValueType == BocDateTimeValueType.DateTime)
        formatString += " HH:mm";
      else if (_dateTimeValue.ValueType == BocDateTimeValueType.Undefined)
        formatString += " HH:mm:ss";

      Html.AssertTextNode (
          span,
          _dateTimeValue.Value.HasValue ? _dateTimeValue.Value.Value.ToString (formatString) : HtmlHelper.WhiteSpace,
          0);
    }

    private XmlNode GetAssertedTimeBoxCell (XmlNode tr)
    {
      var timeBoxCell = Html.GetAssertedChildElement (tr, "td", 2);
      Html.AssertStyleAttribute (timeBoxCell, "width", "40%");
      Html.AssertStyleAttribute (timeBoxCell, "padding-left", "0.3em");
      return timeBoxCell;
    }

    private XmlNode GetAssertedDateBoxCell (XmlNode tr)
    {
      var width = (_dateTimeValue.ValueType == BocDateTimeValueType.Date) ? "100%" : "60%";

      var dateBoxCell = Html.GetAssertedChildElement (tr, "td", 0);
      Html.AssertStyleAttribute (dateBoxCell, "width", width);
      return dateBoxCell;
    }

    private XmlNode GetAssertedButtonCell (XmlNode tr)
    {
      var buttonCell = Html.GetAssertedChildElement (tr, "td", 1);
      Html.AssertStyleAttribute (buttonCell, "width", "0%");
      Html.AssertStyleAttribute (buttonCell, "padding-left", "0.3em");
      return buttonCell;
    }

    private void AssertDateTimePickerButton (XmlNode buttonCell, bool isDisabled)
    {
      var button = Html.GetAssertedChildElement (buttonCell, "a", 0);
      Html.AssertAttribute (button, "id", "_Boc_DatePickerButton");
      Html.AssertAttribute (button, "href", "#");
      string script = string.Format (
          "DatePicker_ShowDatePicker(this, document.getElementById ('{0}'), " +
          "document.getElementById ('{1}'), '{2}', '150pt', '150pt');return false;",
          null,
          _dateTimeValue.DateTextBox.ClientID,
          "/res/Remotion.Web/UI/DatePickerForm.aspx"
          );

      if (isDisabled)
      {
        script = "return false;";
        Html.AssertAttribute (button, "disabled", "disabled");
      }
      Html.AssertAttribute (button, "onclick", script);
      Html.AssertStyleAttribute (button, "padding", "0px");
      Html.AssertStyleAttribute (button, "border", "none");
      Html.AssertStyleAttribute (button, "background-color", "transparent");
    }

    private void AssertTimeTextBox (XmlNode timeBoxCell, bool isDisabled, bool withStyle)
    {
      var timeBox = Html.GetAssertedChildElement (timeBoxCell, "input", 0);
      AssertTextBox (timeBox, "_Boc_TimeTextBox", 5, isDisabled, withStyle);
      if (_dateTimeValue.Value.HasValue)
        Html.AssertAttribute (timeBox, "value", _dateTimeValue.Value.Value.ToString ("HH:mm"));
      else
        Html.AssertNoAttribute (timeBox, "value");
    }

    private void AssertDateTextBox (XmlNode dateBoxCell, bool isDisabled, bool withStyle)
    {
      var dateBox = Html.GetAssertedChildElement (dateBoxCell, "input", 0);
      AssertTextBox (dateBox, "_Boc_DateTextBox", 10, isDisabled, withStyle);
      if (_dateTimeValue.Value.HasValue)
        Html.AssertAttribute (dateBox, "value", _dateTimeValue.Value.Value.ToString ("dd.MM.yyyy"));
      else
        Html.AssertNoAttribute (dateBox, "value");
    }

    private void AssertTextBox (XmlNode textBox, string id, int maxLength, bool isDisabled, bool withStyle)
    {
      Html.AssertAttribute (textBox, "type", "text");
      Html.AssertAttribute (textBox, "id", id);
      Html.AssertAttribute (textBox, "name", id);
      Html.AssertAttribute (textBox, "maxlength", maxLength.ToString());
      Html.AssertStyleAttribute (textBox, "width", "100%");

      if (isDisabled)
      {
        Html.AssertAttribute (textBox, "disabled", "disabled");
        Html.AssertAttribute (textBox, "readonly", "readonly");
      }

      if (withStyle)
        Html.AssertStyleAttribute (textBox, "height", "100%");
    }

    private XmlNode GetAssertedTable (XmlNode div, bool withStyle)
    {
      var table = Html.GetAssertedChildElement (div, "table", 0);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertAttribute (table, "border", "0");

      string width = _dateTimeValue.DefaultWidth;
      if (withStyle)
        width = _dateTimeValue.Width.IsEmpty ? _dateTimeValue.Style["width"] : _dateTimeValue.Width.ToString ();
      
      Html.AssertStyleAttribute (table, "width", width);
      Html.AssertStyleAttribute (table, "display", "inline");
      return table;
    }

    private XmlNode GetAssertedDiv (XmlDocument document, bool isReadOnly, bool isDisabled, bool withStyle)
    {
      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (
          div,
          "class",
          withStyle ? _dateTimeValue.CssClass : _dateTimeValue.CssClassBase,
          HtmlHelper.AttributeValueCompareMode.Contains);
      
      if (isDisabled)
        Html.AssertAttribute (div, "class", _dateTimeValue.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
      
      if (isReadOnly)
        Html.AssertAttribute (div, "class", _dateTimeValue.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);

      Html.AssertStyleAttribute (div, "width", "auto");
      Html.AssertStyleAttribute (div, "display", "inline");

      if (withStyle)
        Html.AssertStyleAttribute (div, "height", _dateTimeValue.Height.IsEmpty ? _dateTimeValue.Style["height"] : _dateTimeValue.Height.ToString());

      return div;
    }
  }
}