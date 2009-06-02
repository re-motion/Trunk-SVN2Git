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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase;
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

    private IBocCheckBox _checkbox;
    private string _startupScript;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _checkbox = MockRepository.GenerateMock<IBocCheckBox>();

      _checkbox.Stub (mock => mock.GetCheckboxKey()).Return ("_Boc_CheckBox");
      _checkbox.Stub (mock => mock.GetImageKey()).Return ("_Boc_Image");
      _checkbox.Stub (mock => mock.GetLabelKey()).Return ("_Boc_Label");

      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      _startupScript = string.Format (
          "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
          _checkbox.DefaultTrueDescription,
          _checkbox.DefaultFalseDescription);
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_checkbox, _startUpScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_checkbox, string.Empty)).Return (c_postbackEventReference);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _checkbox.Stub (mock => mock.Value).PropertyBehavior();
      _checkbox.Stub (mock => mock.HasClientScript).Return (true);
      _checkbox.Stub (mock => mock.IsDescriptionEnabled).Return (true);

      _checkbox.Stub (mock => mock.Page).Return (pageStub);
      _checkbox.Stub (mock => mock.TrueDescription).Return (c_trueDescription);
      _checkbox.Stub (mock => mock.FalseDescription).Return (c_falseDescription);

      _checkbox.Stub (mock => mock.CssClass).PropertyBehavior();
      _checkbox.Stub (mock => mock.CssClassBase).Return ("cssClassBase");
      _checkbox.Stub (mock => mock.CssClassDisabled).Return ("cssClassDisabled");
      _checkbox.Stub (mock => mock.CssClassReadOnly).Return ("cssClassReadonly");

      StateBag stateBag = new StateBag();
      _checkbox.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _checkbox.Stub (mock => mock.Style).Return (_checkbox.Attributes.CssStyle);
      _checkbox.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _checkbox.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
    }

    [Test]
    public void RenderTrue ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClass ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClass ()
    {
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClass ()
    {
      _checkbox.CssClass = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnlyWithCssClassInStandardProperties ()
    {
      _checkbox.Stub (mock => mock.Enabled).Return (true);
      _checkbox.Stub (mock => mock.IsRequired).Return (true);
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (true, _checkbox.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabledWithCssClassInStandardProperties ()
    {
      _checkbox.Attributes["class"] = c_cssClass;
      CheckRender (false, _checkbox.FalseDescription);
    }

    private void CheckRender (bool value, string spanText)
    {
      _checkbox.Value = value;

      var renderer = new BocCheckboxRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _checkbox);
      renderer.Render();

      var document = Html.GetResultDocument();

      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      checkCssClass (outerSpan);

      Html.AssertStyleAttribute (outerSpan, "white-space", "nowrap");
      if (!_checkbox.IsReadOnly)
        Html.AssertStyleAttribute (outerSpan, "width", c_defaultControlWidth);

      if (_checkbox.IsReadOnly)
        CheckImage (value, outerSpan, spanText);
      else
        CheckInput (value, outerSpan);

      var label = Html.GetAssertedChildElement (outerSpan, "span", 1);
      Html.AssertAttribute (label, "id", "_Boc_Label");

      Html.AssertTextNode (label, spanText, 0);
    }

    private void CheckInput (bool value, XmlNode outerSpan)
    {
      var checkbox = Html.GetAssertedChildElement (outerSpan, "input", 0);
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

    private void CheckImage (bool value, XmlNode outerSpan, string altText)
    {
      var image = Html.GetAssertedChildElement (outerSpan, "img", 0);
      Html.AssertAttribute (image, "id", "_Boc_Image");
      Html.AssertAttribute (image, "src", string.Format ("/CheckBox{0}.gif", value), HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (image, "alt", altText);
      Html.AssertStyleAttribute (image, "border-width", "0px");
      Html.AssertStyleAttribute (image, "vertical-align", "middle");
    }

    private void checkCssClass (XmlNode outerSpan)
    {
      string cssClass = _checkbox.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _checkbox.CssClassBase;

      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_checkbox.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _checkbox.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      if (!_checkbox.Enabled)
        Html.AssertAttribute (outerSpan, "class", _checkbox.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}