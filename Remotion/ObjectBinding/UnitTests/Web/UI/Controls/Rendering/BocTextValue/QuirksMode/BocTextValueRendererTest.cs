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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocTextValue.QuirksMode
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

      TextValue.Stub (mock => mock.CssClass).PropertyBehavior();
      TextValue.Stub (mock => mock.CssClassBase).Return ("cssClassBase");
      TextValue.Stub (mock => mock.CssClassDisabled).Return ("cssClassDisabled");
      TextValue.Stub (mock => mock.CssClassReadOnly).Return ("cssClassReadonly");
    }

    [Test]
    public void RenderSingleLineEditable ()
    {
      RenderSingleLineEditable (false, false, false);
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
      RenderSingleLineEditable (true, false, false);
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
      RenderSingleLineEditable (true, true, false);
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
      RenderSingleLineEditable (true, false, true);
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
      RenderSingleLineEditable (true, true, true);
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

    private void RenderSingleLineEditable (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties);

      Renderer.Render();

      HtmlDocument document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      HtmlNode input = Html.GetAssertedChildElement (span, "input", 0, false);
      Html.AssertAttribute (input, "type", "text");
      Html.AssertAttribute (input, "value", c_firstLineText);

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineDisabled (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties);

      TextValue.Stub (mock => mock.Enabled).Return (false);
      Renderer.Render();

      HtmlDocument document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", TextValue.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      HtmlNode input = Html.GetAssertedChildElement (span, "input", 0, false);
      Html.AssertAttribute (input, "disabled", "disabled");
      Html.AssertAttribute (input, "readonly", "readonly");
      Html.AssertAttribute (input, "value", c_firstLineText);

      CheckStyle (withStyle, span, input);
    }

    private void RenderSingleLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (c_firstLineText);

      SetStyle (withStyle, withCssClass, inStandardProperties);

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);
      Renderer.Render();

      HtmlDocument document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", TextValue.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      HtmlNode labelSpan = Html.GetAssertedChildElement (span, "span", 0, false);
      Html.AssertTextNode (labelSpan, c_firstLineText, 0, false);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void RenderMultiLineReadonly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      TextValue.Stub (mock => mock.Text).Return (c_firstLineText + Environment.NewLine + c_secondLineText);
      TextValue.Stub (mock => mock.IsReadOnly).Return (true);

      SetStyle (withStyle, withCssClass, inStandardProperties);
      TextValue.TextBoxStyle.TextMode = TextBoxMode.MultiLine;

      Renderer.Render();

      HtmlDocument document = Html.GetResultDocument();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);

      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", TextValue.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      HtmlNode labelSpan = Html.GetAssertedChildElement (span, "span", 0, false);

      Html.AssertTextNode (labelSpan, c_firstLineText, 0, false);
      Html.GetAssertedChildElement (labelSpan, "br", 1, false);
      Html.AssertTextNode (labelSpan, c_secondLineText, 2, false);
      Html.AssertChildElementCount (labelSpan, 1);

      CheckStyle (withStyle, span, labelSpan);
    }

    private void CheckStyle (bool withStyle, HtmlNode span, HtmlNode valueElement)
    {
      string height = withStyle ? Height.ToString() : null;
      string width = withStyle ? Width.ToString() : /* default constant in BocTextValueRendererBase */ "150pt";
      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "display", "inline-block");

        Html.AssertStyleAttribute (span, "height", height);

        if (height != null)
          Html.AssertStyleAttribute (valueElement, "height", "100%");
        Html.AssertStyleAttribute (valueElement, "width", width);
      }
    }
  }
}