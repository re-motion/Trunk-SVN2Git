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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocBooleanValue.QuirksMode
{
  [TestFixture]
  public class BocCheckboxRendererTest : RendererTestBase
  {
    private const string c_postbackEventReference = "postbackEventReference";
    private const string c_trueDescription = "Wahr";
    private const string c_falseDescription = "Falsch";
    private const string c_defaultControlWidth = "100pt";
    private const string c_cssClass = "someCssClass";
    private readonly string _startUpScriptKey = typeof (BocCheckBox).FullName + "_Startup";

    private MockCheckbox _checkbox;
    private ControlInvoker _controlInvoker;

    private string _startupScript;

    [SetUp]
    public void SetUp ()
    {
      Initialize ();
      _checkbox = new MockCheckbox();
      _controlInvoker = new ControlInvoker (_checkbox);

      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      _startupScript = string.Format (
              "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
              _checkbox.DefaultTrueDescription, _checkbox.DefaultFalseDescription);
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_checkbox, _startUpScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_checkbox, string.Empty)).Return (c_postbackEventReference);

      var pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _checkbox.SetPage (pageStub);
      _checkbox.TrueDescription = c_trueDescription;
      _checkbox.FalseDescription = c_falseDescription;
      _checkbox.ShowDescription = true;
    }

    [Test]
    public void RenderTrue ()
    {
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _checkbox.SetReadOnly (true);
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _checkbox.SetReadOnly (true);
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      _checkbox.Enabled = false;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      _checkbox.Enabled = false;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClass ()
    {
      _checkbox.SetReadOnly (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClass ()
    {
      _checkbox.SetReadOnly (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _checkbox.Enabled = false;
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClass ()
    {
      _checkbox.Enabled = false;
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.SetReadOnly (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.SetReadOnly (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Enabled = false;
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Enabled = false;
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    private void CheckRender (bool value, string spanText)
    {
      _checkbox.Value = value;

      _controlInvoker.PreRenderRecursive ();
      _checkbox.RenderControl (Html.Writer);

      var document = Html.GetResultDocument();

      var outerSpan = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      checkCssClass(outerSpan);

      Html.AssertStyleAttribute (outerSpan, "white-space", "nowrap");
      if (!_checkbox.IsReadOnly)
        Html.AssertStyleAttribute (outerSpan, "width", c_defaultControlWidth);

      if (_checkbox.IsReadOnly)
        CheckImage(value, outerSpan, spanText);
      else
        CheckInput(value, outerSpan);

      var label = Html.GetAssertedChildElement (outerSpan, "span", 1, false);
      Html.AssertAttribute (label, "id", "_Boc_Label");

      Html.AssertTextNode (label, spanText, 0, false);
    }

    private void CheckInput (bool value, HtmlNode outerSpan)
    {
      var checkbox = Html.GetAssertedChildElement (outerSpan, "input", 0, false);
      Html.AssertAttribute (checkbox, "type", "checkbox");
      Html.AssertAttribute (checkbox, "id", "_Boc_CheckBox");
      Html.AssertAttribute (checkbox, "name", "_Boc_CheckBox");
      if (value)
        Html.AssertAttribute (checkbox, "checked", "checked");
      else
        Html.AssertNoAttribute (checkbox, "checked");

      if (_checkbox.Enabled)
        Html.AssertNoAttribute (checkbox, "disabled");
      else
        Html.AssertAttribute (checkbox, "disabled", "disabled");
    }

    private void CheckImage (bool value, HtmlNode outerSpan, string altText)
    {
      var image = Html.GetAssertedChildElement (outerSpan, "img", 0, false);
      Html.AssertAttribute (image, "id", "_Boc_Image");
      Html.AssertAttribute (image, "src", string.Format ("/CheckBox{0}.gif", value), HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (image, "alt", altText);
      Html.AssertStyleAttribute (image, "border-width", "0px");
      Html.AssertStyleAttribute (image, "vertical-align", "middle");
    }

    private void checkCssClass (HtmlNode outerSpan)
    {
      string cssClass = _checkbox.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.CssClassBasePublic;

      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
      if( _checkbox.IsReadOnly )
        Html.AssertAttribute (outerSpan, "class", _checkbox.CssClassReadOnlyPublic, HtmlHelper.AttributeValueCompareMode.Contains);
      if( !_checkbox.Enabled)
        Html.AssertAttribute (outerSpan, "class", _checkbox.CssClassDisabledPublic, HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}