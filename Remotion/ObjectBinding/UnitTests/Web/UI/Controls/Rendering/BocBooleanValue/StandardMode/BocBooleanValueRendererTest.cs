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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.StandardMode;
using System.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocBooleanValue.StandardMode
{
  [TestFixture]
  public class BocBooleanValueRendererTest : RendererTestBase
  {
    private const string c_defaultControlWidth = "100pt";
    private const string c_trueDescription = "Wahr";
    private const string c_falseDescription = "Falsch";
    private const string c_nullDescription = "Unbestimmt";
    private const string c_cssClass = "someCssClass";
    private const string c_postbackEventReference = "postbackEventReference";

    private string _startupScript;
    private string _clickScript;
    private string _keyDownScript;
    private const string _dummyScript = "return false;";
    private IBocBooleanValue _booleanValue;
    private IBocBooleanValueRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      var resourceSet = new BocBooleanValueResourceSet (
          "ResourceKey",
          "TrueIconUrl",
          "FalseIconUrl",
          "NullIconUrl",
          "DefaultTrueDescription",
          "DefaultFalseDescription",
          "DefaultNullDescription"
          );

      _booleanValue = MockRepository.GenerateMock<IBocBooleanValue>();
      _booleanValue.Stub (mock => mock.CreateResourceSet()).Return (resourceSet);

      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();

      _booleanValue.Stub (mock => mock.ClientID).Return ("MyBooleanValue");
      _booleanValue.Stub (mock => mock.GetHiddenFieldUniqueID()).Return ("_Boc_HiddenField");
      _booleanValue.Stub (mock => mock.GetHyperLinkUniqueID()).Return ("_Boc_HyperLink");
      _booleanValue.Stub (mock => mock.GetImageClientID()).Return ("_Boc_Image");
      _booleanValue.Stub (mock => mock.GetLabelClientID()).Return ("_Boc_Label");

      string startupScriptKey = typeof (ObjectBinding.Web.UI.Controls.BocBooleanValue).FullName + "_Startup_" + resourceSet.ResourceKey;
      _startupScript = string.Format (
          "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
          resourceSet.ResourceKey,
          "true",
          "false",
          "null",
          ScriptUtility.EscapeClientScript (resourceSet.DefaultTrueDescription),
          ScriptUtility.EscapeClientScript (resourceSet.DefaultFalseDescription),
          ScriptUtility.EscapeClientScript (resourceSet.DefaultNullDescription),
          resourceSet.TrueIconUrl,
          resourceSet.FalseIconUrl,
          resourceSet.NullIconUrl);
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_booleanValue, typeof (BocBooleanValueRenderer), startupScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<Type>.Is.NotNull, Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_booleanValue, string.Empty)).Return (c_postbackEventReference);

      _clickScript = string.Format (
          "BocBooleanValue_SelectNextCheckboxValue ('{0}', document.getElementById ('{1}'), " +
          "document.getElementById ('{2}'), document.getElementById ('{3}'), false, " +
          "'" + c_trueDescription + "', '" + c_falseDescription + "', '" + c_nullDescription + "');return false;",
          "ResourceKey",
          _booleanValue.GetImageClientID(),
          _booleanValue.GetLabelClientID(),
          _booleanValue.GetHiddenFieldUniqueID());

      _keyDownScript = "BocBooleanValue_OnKeyDown (this);";

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _booleanValue.Stub (mock => mock.Value).PropertyBehavior();
      _booleanValue.Stub (mock => mock.IsDesignMode).Return (false);
      _booleanValue.Stub (mock => mock.ShowDescription).Return (true);

      _booleanValue.Stub (mock => mock.Page).Return (pageStub);
      _booleanValue.Stub (mock => mock.TrueDescription).Return (c_trueDescription);
      _booleanValue.Stub (mock => mock.FalseDescription).Return (c_falseDescription);
      _booleanValue.Stub (mock => mock.NullDescription).Return (c_nullDescription);

      _booleanValue.Stub (mock => mock.CssClass).PropertyBehavior();

      StateBag stateBag = new StateBag();
      _booleanValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _booleanValue.Stub (mock => mock.Style).Return (_booleanValue.Attributes.CssStyle);
      _booleanValue.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _booleanValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
    }

    [Test]
    public void RenderTrue ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = false;
      CheckRendering (false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNull ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = null;
      CheckRendering ("null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = false;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering (false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullReadOnly ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = null;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      CheckRendering ("null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      _booleanValue.Value = true;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      _booleanValue.Value = false;
      CheckRendering (false.ToString(), "FalseIconUrl", _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullDisabled ()
    {
      _booleanValue.Value = null;
      CheckRendering ("null", "NullIconUrl", _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _booleanValue.Value = true;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClass ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperty ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperty ()
    {
      _booleanValue.Value = true;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClassInStandardProperty ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsReadOnly).Return (true);
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithAutoPostback ()
    {
      _booleanValue.Stub (mock => mock.Enabled).Return (true);
      _booleanValue.Value = true;
      _booleanValue.Stub (mock => mock.IsAutoPostBackEnabled).Return (true);
      _clickScript = _clickScript.Insert (_clickScript.IndexOf ("return false;"), c_postbackEventReference + ";");
      CheckRendering (true.ToString(), "TrueIconUrl", _booleanValue.TrueDescription);
    }

    private void CheckRendering (string value, string iconUrl, string description)
    {
      _renderer = new BocBooleanValueRenderer (MockRepository.GenerateMock<HttpContextBase>(), Html.Writer, _booleanValue);
      _renderer.Render (Html.Writer);
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document, "span", 0);
      CheckOuterSpanAttributes (outerSpan);

      int offset = 0;
      if (!_booleanValue.IsReadOnly)
      {
        CheckHiddenField (outerSpan, value);
        offset = 1;
      }
      Html.AssertChildElementCount (outerSpan, 2 + offset);

      var link = Html.GetAssertedChildElement (outerSpan, "a", offset);
      Html.AssertAttribute (link, "id", "_Boc_HyperLink");
      if (!_booleanValue.IsReadOnly)
        CheckLinkAttributes (link);

      var image = Html.GetAssertedChildElement (link, "img", 0);
      checkImageAttributes (image, iconUrl, description);

      var label = Html.GetAssertedChildElement (outerSpan, "span", offset + 1);
      Html.AssertAttribute (label, "id", "_Boc_Label");
      Html.AssertChildElementCount (label, 0);
      Html.AssertTextNode (label, description, 0);

      if (!_booleanValue.IsReadOnly)
        Html.AssertAttribute (label, "onclick", _booleanValue.Enabled ? _clickScript : _dummyScript);
    }

    private void CheckCssClass (XmlNode outerSpan)
    {
      string cssClass = _booleanValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _booleanValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _renderer.CssClassBase;
      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void CheckOuterSpanAttributes (XmlNode outerSpan)
    {
      CheckCssClass (outerSpan);

      Html.AssertAttribute (outerSpan, "id", "MyBooleanValue");

      if (!_booleanValue.Enabled)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_booleanValue.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      else
        Html.AssertStyleAttribute (outerSpan, "width", c_defaultControlWidth);

      Html.AssertStyleAttribute (outerSpan, "white-space", "nowrap");
    }

    private void checkImageAttributes (XmlNode image, string iconUrl, string description)
    {
      Html.AssertAttribute (image, "id", "_Boc_Image");
      Html.AssertAttribute (image, "src", iconUrl);
      Html.AssertAttribute (image, "alt", description);
      Html.AssertStyleAttribute (image, "border-width", "0px");
      Html.AssertStyleAttribute (image, "vertical-align", "middle");
    }

    private void CheckLinkAttributes (XmlNode link)
    {
      Html.AssertAttribute (link, "onclick", _booleanValue.Enabled ? _clickScript : _dummyScript);
      Html.AssertAttribute (link, "onkeydown", _keyDownScript);
      Html.AssertAttribute (link, "href", "#");
      Html.AssertStyleAttribute (link, "padding", "0px");
      Html.AssertStyleAttribute (link, "border", "none");
      Html.AssertStyleAttribute (link, "background-color", "transparent");
    }

    private void CheckHiddenField (XmlNode outerSpan, string value)
    {
      var hiddenField = Html.GetAssertedChildElement (outerSpan, "input", 0);
      Html.AssertAttribute (hiddenField, "type", "hidden");
      Html.AssertAttribute (hiddenField, "id", "_Boc_HiddenField");
      Html.AssertAttribute (hiddenField, "value", value);
    }
  }
}