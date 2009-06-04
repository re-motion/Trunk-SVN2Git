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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocDateTimeValue
{
  [TestFixture]
  public class BocDateTimeValueRendererTest : RendererTestBase
  {
    private const string c_defaultControlWidth = "150pt";

    private IBocDateTimeValue _dateTimeValue;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _dateTimeValue = MockRepository.GenerateStub<IBocDateTimeValue>();
      _dateTimeValue.ID = "controlId";
      _dateTimeValue.Stub (mock => mock.ClientID).Return ("controlId");
      _dateTimeValue.Stub (mock => mock.DatePickerButton).Return (new StubDatePickerButton());
      _dateTimeValue.DatePickerButton.AlternateText = "DatePickerButton";

      _dateTimeValue.Stub (mock => mock.ProvideMaxLength).Return (true);

      _dateTimeValue.Stub (mock => mock.CssClassBase).Return ("cssClassBase");
      _dateTimeValue.Stub (mock => mock.CssClassDisabled).Return ("cssClassDisabled");
      _dateTimeValue.Stub (mock => mock.CssClassReadOnly).Return ("cssClassReadonly");

      _dateTimeValue.Stub (mock => mock.GetDateTextboxId()).Return ("DateTextboxId");
      _dateTimeValue.Stub (mock => mock.GetTimeTextboxId ()).Return ("TimeTextboxId");
      
      StateBag stateBag = new StateBag();
      _dateTimeValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _dateTimeValue.Stub (mock => mock.Style).Return (_dateTimeValue.Attributes.CssStyle);
      _dateTimeValue.Stub (mock => mock.DateTextBoxStyle).Return (new TextBoxStyle ());
      _dateTimeValue.Stub (mock => mock.TimeTextBoxStyle).Return (new TextBoxStyle ());
      _dateTimeValue.Stub (mock => mock.DateTimeTextBoxStyle).Return (new TextBoxStyle ());
      _dateTimeValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

    }

    [Test]
    public void RenderUndefined ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTimeWithSeconds ()
    {
      _dateTimeValue.Stub (mock => mock.ShowSeconds).Return (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTime ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDate ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderUndefinedDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateTimeDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderUndefinedReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateTimeReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderUndefinedWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderUndefinedWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderEmptyDateTime ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderEmptyDateTimeReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _dateTimeValue);
      renderer.Render ();

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
        _dateTimeValue.ControlStyle.Width = _dateTimeValue.Width;
        _dateTimeValue.ControlStyle.Height = _dateTimeValue.Height;
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
      Html.AssertChildElementCount (buttonCell, 0);
      
      if (_dateTimeValue.ActualValueType != BocDateTimeValueType.Date)
      {
        XmlNode timeBoxCell = GetAssertedTimeBoxCell (tr);
        AssertTimeTextBox (timeBoxCell, isDisabled, withStyle);
      }
    }

    private void AssertSpan (XmlNode div)
    {
      var span = Html.GetAssertedChildElement (div, "span", 0);
      string formatString = "dd.MM.yyyy";
      if (_dateTimeValue.ActualValueType == BocDateTimeValueType.DateTime)
        formatString += " HH:mm";
      else if (_dateTimeValue.ActualValueType == BocDateTimeValueType.Undefined)
        formatString += " HH:mm:ss";

      Html.AssertTextNode (
          span,
          _dateTimeValue.Value.HasValue ? _dateTimeValue.Value.Value.ToString (formatString) : HtmlHelper.WhiteSpace,
          0);
    }

    private XmlNode GetAssertedTimeBoxCell (XmlNode tr)
    {
      var timeBoxCell = Html.GetAssertedChildElement (tr, "td", 2);
      Html.AssertStyleAttribute (timeBoxCell, "width", _dateTimeValue.ShowSeconds ? "45%" : "40%");
      Html.AssertStyleAttribute (timeBoxCell, "padding-left", "0.3em");
      return timeBoxCell;
    }

    private XmlNode GetAssertedDateBoxCell (XmlNode tr)
    {
      var width = "100%";
      if (_dateTimeValue.ActualValueType != BocDateTimeValueType.Date)
      {
        if (_dateTimeValue.ShowSeconds)
          width = "55%";
        else
          width = "60%";
      }

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

    private void AssertTimeTextBox (XmlNode timeBoxCell, bool isDisabled, bool withStyle)
    {
      var timeBox = Html.GetAssertedChildElement (timeBoxCell, "input", 0);
      int maxLength = 5;
      string timeFormat = "HH:mm";

      if (_dateTimeValue.ShowSeconds)
      {
        maxLength = 8;
        timeFormat = "HH:mm:ss";
      }
      AssertTextBox (timeBox, _dateTimeValue.GetTimeTextboxId(), maxLength, isDisabled, withStyle);
      if (_dateTimeValue.Value.HasValue)
        Html.AssertAttribute (timeBox, "value", _dateTimeValue.Value.Value.ToString (timeFormat));
      else
        Html.AssertNoAttribute (timeBox, "value");
    }

    private void AssertDateTextBox (XmlNode dateBoxCell, bool isDisabled, bool withStyle)
    {
      var dateBox = Html.GetAssertedChildElement (dateBoxCell, "input", 0);
      AssertTextBox (dateBox, _dateTimeValue.GetDateTextboxId(), 10, isDisabled, withStyle);
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

      string width = c_defaultControlWidth;
      if (withStyle)
        width = _dateTimeValue.Width.IsEmpty ? _dateTimeValue.Style["width"] : _dateTimeValue.Width.ToString();

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