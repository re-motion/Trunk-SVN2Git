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
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.StandardMode;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocTextValue.StandardMode
{
  [TestFixture]
  public class BocTextValueRendererTest : BocTextValueRendererTestBase<IBocTextValue>
  {
    [SetUp]
    public void SetUp ()
    {
      Initialize();
      TextValue = MockRepository.GenerateMock<IBocTextValue>();
      Renderer = new BocTextValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, TextValue);

      TextValue.Stub (stub => stub.ClientID).Return ("MyTextValue");
      TextValue.Stub (stub => stub.TextBoxID).Return ("MyTextValue_Boc_Textbox");
      TextValue.Stub (mock => mock.CssClass).PropertyBehavior();

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());

      TextValue.Stub (stub => stub.Page).Return (pageStub);
    }

    [Test]
    public void RenderSingleLineEditabaleAutoPostback ()
    {
      RenderSingleLineEditable (false, false, false, true);
    }

    [Test]
    public void RenderSingleLineEditable ()
    {
      RenderSingleLineEditable (false, false, false, false);
    }

    [Test]
    public void RenderSingleLineDisabled ()
    {
      RenderSingleLineDisabled (false, false, false);
    }

    [Test]
    public void RenderSingleLineReadonly ()
    {
      RenderSingleLineReadonly (false, false, false);
    }

    [Test]
    public void RenderMultiLineReadonly ()
    {
      RenderMultiLineReadonly (false, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyle ()
    {
      RenderSingleLineEditable (true, false, false, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyle ()
    {
      RenderSingleLineDisabled (true, false, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyle ()
    {
      RenderSingleLineReadonly (true, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyle ()
    {
      RenderMultiLineReadonly (true, false, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClass ()
    {
      RenderSingleLineEditable (true, true, false, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClass ()
    {
      RenderSingleLineDisabled (true, true, false);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClass ()
    {
      RenderSingleLineReadonly (true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClass ()
    {
      RenderMultiLineReadonly (true, true, false);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleInStandardProperties ()
    {
      RenderSingleLineEditable (true, false, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleInStandardProperties ()
    {
      RenderSingleLineDisabled (true, false, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleInStandardProperties ()
    {
      RenderSingleLineReadonly (true, false, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleInStandardProperties ()
    {
      RenderMultiLineReadonly (true, false, true);
    }

    [Test]
    public void RenderSingleLineEditableWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineEditable (true, true, true, false);
    }

    [Test]
    public void RenderSingleLineDisabledWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineDisabled (true, true, true);
    }

    [Test]
    public void RenderSingleLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderSingleLineReadonly (true, true, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineReadonly (true, true, true);
    }

    private void RenderSingleLineEditable (bool withStyle, bool withCssClass, bool inStandardProperties, bool autoPostBack)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, autoPostBack);

      Renderer.Render();

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var input = Html.GetAssertedChildElement (span, "input", 0);
      Html.AssertAttribute (input, "type", "text");
      Html.AssertAttribute (input, "value", BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText);
      if (TextValue.TextBoxStyle.AutoPostBack == true)
        Html.AssertAttribute (input, "onchange", string.Format ("javascript:__doPostBack('{0}','')", TextValue.TextBoxID));

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineDisabled (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Stub (mock => mock.Enabled).Return (false);
      Renderer.Render();

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", Renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var input = Html.GetAssertedChildElement (span, "input", 0);
      Html.AssertAttribute (input, "disabled", "disabled");
      Html.AssertAttribute (input, "readonly", "readonly");
      Html.AssertAttribute (input, "value", BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText);

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);
      Renderer.Render();

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);
      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", Renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var labelSpan = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (labelSpan, BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, 0);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void RenderMultiLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (
          BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText + Environment.NewLine
          + BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText);
      TextValue.Stub (mock => mock.IsReadOnly).Return (true);

      SetStyle (withStyle, withCssClass, inStandardProperties, false);
      TextValue.TextBoxStyle.TextMode = TextBoxMode.MultiLine;

      Renderer.Render();

      var document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentElement, 1);

      var span = Html.GetAssertedChildElement (document, "span", 0);

      Html.AssertAttribute (span, "id", "MyTextValue");
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", Renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      var labelSpan = Html.GetAssertedChildElement (span, "span", 0);

      Html.AssertTextNode (labelSpan, BocTextValueRendererTestBase<IBocTextValue>.c_firstLineText, 0);
      Html.GetAssertedChildElement (labelSpan, "br", 1);
      Html.AssertTextNode (labelSpan, BocTextValueRendererTestBase<IBocTextValue>.c_secondLineText, 2);
      Html.AssertChildElementCount (labelSpan, 1);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void CheckStyle (bool withStyle, XmlNode span, XmlNode valueElement)
    {
      string height = withStyle ? Height.ToString() : null;
      string width = withStyle ? Width.ToString() : /* default constant in BocTextValueRendererBase */ "150pt";
      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "height", height);

        if (height != null)
          Html.AssertStyleAttribute (valueElement, "height", "100%");
        Html.AssertStyleAttribute (valueElement, "width", width);
      }
    }
  }
}
