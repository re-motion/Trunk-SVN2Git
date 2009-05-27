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
  public class BocMultilineTextValueRendererTest : BocTextValueRendererTestBase<IBocMultilineTextValue>
  {
    private const string c_cssClass = "SomeClass";

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      TextValue = MockRepository.GenerateMock<IBocMultilineTextValue>();
      TextValue.Stub (mock => mock.Text).Return (c_firstLineText + Environment.NewLine + c_secondLineText);
      TextValue.Stub (mock => mock.Value).Return (new[] { c_firstLineText, c_secondLineText });

      TextValue.Stub (mock => mock.CssClass).PropertyBehavior ();
      TextValue.Stub (mock => mock.CssClassBase).Return ("cssClassBase");
      TextValue.Stub (mock => mock.CssClassDisabled).Return ("cssClassDisabled");
      TextValue.Stub (mock => mock.CssClassReadOnly).Return ("cssClassReadonly");

      Renderer = new BocMultilineTextValueRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, TextValue);
    }

    [Test]
    public void RenderMultiLineEditable ()
    {
      RenderMultiLineEditable (false, false, false, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyle ()
    {
      RenderMultiLineEditable(false, true, false, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleInStandardProperties ()
    {
      RenderMultiLineEditable (false, true, false, true);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleAndCssClass ()
    {
      RenderMultiLineEditable (false, true, true, false);
    }

    [Test]
    public void RenderMultiLineEditableWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineEditable (false, true, true, true);
    }

    [Test]
    public void RenderMultiLineReadonly ()
    {
      RenderMultiLineReadOnly (false, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyle ()
    {
      RenderMultiLineReadOnly(true, false, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleInStandardProperties ()
    {
      RenderMultiLineReadOnly (true, false, true);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClass ()
    {
      RenderMultiLineReadOnly (true, true, false);
    }

    [Test]
    public void RenderMultiLineReadonlyWithStyleAndCssClassInStandardProperties ()
    {
      RenderMultiLineReadOnly (true, true, true);
    }

    [Test]
    public void RenderMultiLineDisabled ()
    {
      RenderMultiLineEditable (true, false, false, false);
    }

    [Test]
    public void RenderMultiLineDisabledWithStyle ()
    {
      RenderMultiLineEditable (true, true, false, false);
    }

    [Test]
    public void RenderMultiLineDisabledWithStyleInStandardProperties ()
    {
      RenderMultiLineEditable (true, true, false, true);
    }

    private void RenderMultiLineEditable (bool isDisabled, bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      SetStyle(withStyle, withCssClass, inStandardProperties);

      TextValue.Stub (mock => mock.Enabled).Return (!isDisabled);

      Renderer.Render();
      var document = Html.GetResultDocument ();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      CheckCssClass (span, withCssClass, inStandardProperties);
      if( isDisabled )
        Html.AssertAttribute (span, "class", TextValue.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);

      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      HtmlNode textarea = Html.GetAssertedChildElement (span, "textarea", 0, false);
      CheckTextAreaStyle (textarea, false, withStyle);
      Html.AssertTextNode (textarea, TextValue.Text, 0, false);
      Html.AssertChildElementCount (textarea, 0);
    }

    private void RenderMultiLineReadOnly (bool withStyle, bool withCssClass, bool inStandardProperties)
    {
      SetStyle (withStyle, withCssClass, inStandardProperties);

      TextValue.Stub (mock => mock.IsReadOnly).Return (true);

      Renderer.Render ();

      var document = Html.GetResultDocument ();
      Html.AssertChildElementCount (document.DocumentNode, 1);

      HtmlNode span = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      CheckCssClass (span, withCssClass, inStandardProperties);
      Html.AssertAttribute (span, "class", TextValue.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (span, "width", "auto");
      Html.AssertChildElementCount (span, 1);

      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "height", Height.ToString ());

        if(!inStandardProperties)
          Html.AssertStyleAttribute (span, "display", "inline-block");
      }

      HtmlNode label = Html.GetAssertedChildElement (span, "span", 0, false);
      Html.AssertTextNode (label, c_firstLineText, 0, false);
      Html.GetAssertedChildElement (label, "br", 1, false);
      Html.AssertTextNode (label, c_secondLineText, 2, false);
      Html.AssertChildElementCount (label, 1);
    }

    private void CheckTextAreaStyle (HtmlNode textarea, bool isDisabled, bool withStyle)
    {
      string width = withStyle ? Width.ToString () : "150pt";

      int rowCount = TextValue.Text.Split (new[] { Environment.NewLine }, StringSplitOptions.None).Length;
      Html.AssertAttribute (textarea, "rows", rowCount.ToString());
      Html.AssertAttribute (textarea, "cols", "60");
      Html.AssertStyleAttribute (textarea, "width", width);

      if (isDisabled)
      {
        Html.AssertAttribute (textarea, "readonly", "readonly");
        Html.AssertAttribute (textarea, "disabled", "disabled");
      }
    }

    protected override void SetStyle (bool withStyle, bool withCssClass, bool inStyleProperty)
    {
      base.SetStyle (withStyle, withCssClass, inStyleProperty);
      TextValue.TextBoxStyle.TextMode = TextBoxMode.MultiLine;
    }
  }
}