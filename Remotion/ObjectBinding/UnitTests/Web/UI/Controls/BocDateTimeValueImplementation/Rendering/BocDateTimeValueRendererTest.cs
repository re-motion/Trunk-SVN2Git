// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  [TestFixture]
  [SetCulture("")]
  public class BocDateTimeValueRendererTest : RendererTestBase
  {
    private const string c_timeString = "15:43";
    private const string c_dateString = "31.07.2009";
    private IBocDateTimeValue _control;
    private SingleRowTextBoxStyle _dateStyle;
    private SingleRowTextBoxStyle _timeStyle;
    private StubTextBox _dateTextBox;
    private StubTextBox _timeTextBox;
    private BocDateTimeValueRenderingContext _renderingContext;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _control = MockRepository.GenerateStub<IBocDateTimeValue>();
      _control.Stub (stub => stub.ClientID).Return ("MyDateTimeValue");
      _control.Stub (stub => stub.DateTextboxID).Return ("MyDateTimeValue$Boc_DateTextBox");
      _control.Stub (stub => stub.TimeTextboxID).Return ("MyDateTimeValue$Boc_TimeTextBox");

      _dateStyle = new SingleRowTextBoxStyle();
      _timeStyle = new SingleRowTextBoxStyle();
      _control.Stub (stub => stub.DateTextBoxStyle).Return (_dateStyle);
      _control.Stub (stub => stub.TimeTextBoxStyle).Return (_timeStyle);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new Page());
      pageStub.Stub (stub => stub.ClientScript).Return (MockRepository.GenerateStub<IClientScriptManager>());
      _control.Stub (stub => stub.Page).Return (pageStub);

      var datePickerButton = MockRepository.GenerateStub<IDatePickerButton> ();
      datePickerButton.Stub (stub => stub.EnableClientScript).Return (true);
      datePickerButton.Stub (stub => stub.RenderControl (Html.Writer)).WhenCalled (invocation => Html.Writer.WriteLine ("DatePicker"));
      _control.Stub (stub => stub.DatePickerButton).Return (datePickerButton);

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.Style).Return (_control.Attributes.CssStyle);
      _control.Stub (stub => stub.DateTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.TimeTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.DateTimeTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      _control.Stub (stub => stub.ProvideMaxLength).Return (true);

      _dateTextBox = new StubTextBox();
      _timeTextBox = new StubTextBox();

      _renderingContext = new BocDateTimeValueRenderingContext (HttpContext, Html.Writer, _control);
    }

    [Test]
    public void RenderDateValue ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.Date);
      _control.Stub (stub => stub.Enabled).Return (true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, true);

      AssertDate (container, renderer);

      container.AssertTextNode ("DatePicker", 1);
    }

    [Test]
    public void RenderDateTimeValue ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.TimeString).Return (c_timeString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _control.Stub (stub => stub.Enabled).Return (true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, false);

      AssertDate (container, renderer);
      AssertTime (container, renderer);

      container.AssertTextNode ("DatePicker", 1);
    }

    private void AssertTime (XmlNode container, BocDateTimeValueRenderer renderer)
    {
      var timeInputWrapper = container.GetAssertedChildElement ("span", 2);
      timeInputWrapper.AssertAttributeValueContains ("class", renderer.CssClassTimeInputWrapper);
      timeInputWrapper.AssertAttributeValueContains ("class", renderer.GetPositioningCssClass (_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Time));
      timeInputWrapper.AssertChildElementCount (0);

      timeInputWrapper.AssertTextNode ("TextBox", 0);

      Assert.That (_timeTextBox.ID, Is.EqualTo (_control.TimeTextboxID));
      Assert.That (_timeTextBox.CssClass, Is.EqualTo (renderer.CssClassTime));
      Assert.That (_timeTextBox.Text, Is.EqualTo (c_timeString));
      Assert.That (_timeTextBox.MaxLength, Is.EqualTo (5));
    }

    private void AssertDate (XmlNode container, BocDateTimeValueRenderer renderer)
    {
      var dateInputWrapper = container.GetAssertedChildElement ("span", 0);
      dateInputWrapper.AssertAttributeValueContains ("class", renderer.CssClassDateInputWrapper);
      dateInputWrapper.AssertAttributeValueContains ("class", renderer.GetPositioningCssClass (_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Date));
      dateInputWrapper.AssertChildElementCount (0);

      dateInputWrapper.AssertTextNode ("TextBox", 0);
      Assert.That (_dateTextBox.ID, Is.EqualTo (_control.DateTextboxID));
      Assert.That (_dateTextBox.CssClass, Is.EqualTo (renderer.CssClassDate));
      Assert.That (_dateTextBox.Text, Is.EqualTo (c_dateString));
      Assert.That (_dateTextBox.MaxLength, Is.EqualTo (10));
    }

    private XmlNode GetAssertedContainer (out BocDateTimeValueRenderer renderer, bool isDateOnly)
    {
      renderer = new TestableBocDateTimeValueRenderer (MockRepository.GenerateStub<IResourceUrlFactory>(), _dateTextBox, _timeTextBox);
      renderer.Render (new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control));

      var document = Html.GetResultDocument ();
      var container = document.GetAssertedChildElement ("span", 0);
      container.AssertAttributeValueEquals ("id", "MyDateTimeValue");
      container.AssertAttributeValueEquals ("class", isDateOnly ? renderer.CssClassDateOnly : renderer.CssClassDateTime);
      container.AssertChildElementCount (isDateOnly ? 1 : 2);
      return container;
    }
  }
}