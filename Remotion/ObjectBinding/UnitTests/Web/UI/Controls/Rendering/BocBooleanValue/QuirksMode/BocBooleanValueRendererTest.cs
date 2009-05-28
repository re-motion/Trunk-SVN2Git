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
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocBooleanValue.QuirksMode
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

    private ControlInvoker _controlInvoker;
    private string _startupScript;
    private string _clickScript;
    private string _keyDownScript;
    private string _dummyScript = "return false;";
    private MockBooleanValue _booleanValue;
    
    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _booleanValue = new MockBooleanValue();
      _controlInvoker = new ControlInvoker (_booleanValue);

      var clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      var resourceSet = _booleanValue.GetResourceSet();
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
      clientScriptManagerMock.Expect (mock => mock.RegisterStartupScriptBlock (_booleanValue, startupScriptKey, _startupScript));
      clientScriptManagerMock.Stub (mock => mock.IsStartupScriptRegistered (Arg<string>.Is.NotNull)).Return (false);
      clientScriptManagerMock.Stub (mock => mock.GetPostBackEventReference (_booleanValue, string.Empty)).Return (c_postbackEventReference);

      _clickScript = "BocBooleanValue_SelectNextCheckboxValue ('resourceGroup', document.getElementById ('_Boc_Image'), " +
                     "document.getElementById ('_Boc_Label'), document.getElementById ('_Boc_HiddenField'), false, " +
                     "'" + c_trueDescription + "', '" + c_falseDescription + "', '" + c_nullDescription + "');return false;";

      _keyDownScript = "BocBooleanValue_OnKeyDown (this);";

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptManagerMock);

      _booleanValue.SetPage (pageStub);
      _booleanValue.TrueDescription = c_trueDescription;
      _booleanValue.FalseDescription = c_falseDescription;
      _booleanValue.NullDescription = c_nullDescription;
    }

    [Test]
    public void RenderTrue ()
    {
      _booleanValue.Value = true;
      CheckRendering (true.ToString(), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalse ()
    {
      _booleanValue.Value = false;
      CheckRendering (false.ToString(), _booleanValue.FalseIconUrl, _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNull ()
    {
      _booleanValue.Value = null;
      CheckRendering ("null", _booleanValue.NullIconUrl, _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueReadOnly ()
    {
      _booleanValue.Value = true;
      _booleanValue.SetReadOnly (true);
      CheckRendering (true.ToString(), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseReadOnly ()
    {
      _booleanValue.Value = false;
      _booleanValue.SetReadOnly (true);
      CheckRendering (false.ToString(), _booleanValue.FalseIconUrl, _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullReadOnly ()
    {
      _booleanValue.Value = null;
      _booleanValue.SetReadOnly (true);
      CheckRendering ("null", _booleanValue.NullIconUrl, _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueDisabled ()
    {
      _booleanValue.Value = true;
      _booleanValue.Enabled = false;
      CheckRendering (true.ToString(), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderFalseDisabled ()
    {
      _booleanValue.Value = false;
      _booleanValue.Enabled = false;
      CheckRendering (false.ToString(), _booleanValue.FalseIconUrl, _booleanValue.FalseDescription);
    }

    [Test]
    public void RenderNullDisabled ()
    {
      _booleanValue.Value = null;
      _booleanValue.Enabled = false;
      CheckRendering ("null", _booleanValue.NullIconUrl, _booleanValue.NullDescription);
    }

    [Test]
    public void RenderTrueWithCssClass ()
    {
      _booleanValue.Value = true;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClass ()
    {
      _booleanValue.Value = true;
      _booleanValue.Enabled = false;
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClass ()
    {
      _booleanValue.Value = true;
      _booleanValue.SetReadOnly (true);
      _booleanValue.CssClass = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithCssClassInStandardProperty ()
    {
      _booleanValue.Value = true;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueDisabledWithCssClassInStandardProperty ()
    {
      _booleanValue.Value = true;
      _booleanValue.Enabled = false;
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueReadonlyWithCssClassInStandardProperty ()
    {
      _booleanValue.Value = true;
      _booleanValue.SetReadOnly (true);
      _booleanValue.Attributes["class"] = c_cssClass;
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }

    [Test]
    public void RenderTrueWithAutoPostback ()
    {
      _booleanValue.Value = true;
      _booleanValue.AutoPostBack = true;
      _clickScript = _clickScript.Insert (_clickScript.IndexOf ("return false;"), c_postbackEventReference + ";");
      CheckRendering (true.ToString (), _booleanValue.TrueIconUrl, _booleanValue.TrueDescription);
    }


    private void CheckRendering (string value, string iconUrl, string description)
    {
      _controlInvoker.PreRenderRecursive();
      _booleanValue.RenderControl (Html.Writer);
      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement (document.DocumentNode, "span", 0, false);
      checkOuterSpanAttributes (outerSpan);

      int offset = 0;
      if (!_booleanValue.IsReadOnly)
      {
        CheckHiddenField (outerSpan, value);
        offset = 1;
      }
      Html.AssertChildElementCount (outerSpan, 2 + offset);

      var link = Html.GetAssertedChildElement (outerSpan, "a", offset, false);
      Html.AssertAttribute (link, "id", "_Boc_HyperLink");
      if (!_booleanValue.IsReadOnly)
        CheckLinkAttributes (link);

      var image = Html.GetAssertedChildElement (link, "img", 0, false);
      checkImageAttributes (image, iconUrl, description);

      var label = Html.GetAssertedChildElement (outerSpan, "span", offset + 1, false);
      Html.AssertAttribute (label, "id", "_Boc_Label");
      Html.AssertChildElementCount (label, 0);
      Html.AssertTextNode (label, description, 0, false);

      if (!_booleanValue.IsReadOnly)
        Html.AssertAttribute (label, "onclick", _booleanValue.Enabled ? _clickScript : _dummyScript);
    }

    private void CheckCssClass (HtmlNode outerSpan)
    {
      string cssClass = _booleanValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _booleanValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _booleanValue.CssClassBasePublic;
      Html.AssertAttribute (outerSpan, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void checkOuterSpanAttributes (HtmlNode outerSpan)
    {
      CheckCssClass (outerSpan);

      if (!_booleanValue.Enabled)
        Html.AssertAttribute (outerSpan, "class", _booleanValue.CssClassDisabledPublic, HtmlHelper.AttributeValueCompareMode.Contains);
      if (_booleanValue.IsReadOnly)
        Html.AssertAttribute (outerSpan, "class", _booleanValue.CssClassReadOnlyPublic, HtmlHelper.AttributeValueCompareMode.Contains);
      else
        Html.AssertStyleAttribute (outerSpan, "width", c_defaultControlWidth);

      Html.AssertStyleAttribute (outerSpan, "white-space", "nowrap");
    }

    private void checkImageAttributes (HtmlNode image, string iconUrl, string description)
    {
      Html.AssertAttribute (image, "id", "_Boc_Image");
      Html.AssertAttribute (image, "src", iconUrl);
      Html.AssertAttribute (image, "alt", description);
      Html.AssertStyleAttribute (image, "border-width", "0px");
      Html.AssertStyleAttribute (image, "vertical-align", "middle");
    }

    private void CheckLinkAttributes (HtmlNode link)
    {
      Html.AssertAttribute (link, "onclick", _booleanValue.Enabled ? _clickScript : _dummyScript);
      Html.AssertAttribute (link, "onkeydown", _keyDownScript);
      Html.AssertAttribute (link, "href", "#");
      Html.AssertStyleAttribute (link, "padding", "0px");
      Html.AssertStyleAttribute (link, "border", "none");
      Html.AssertStyleAttribute (link, "background-color", "transparent");
    }

    private void CheckHiddenField (HtmlNode outerSpan, string value)
    {
      var hiddenField = Html.GetAssertedChildElement (outerSpan, "input", 0, false);
      Html.AssertAttribute (hiddenField, "type", "hidden");
      Html.AssertAttribute (hiddenField, "id", "_Boc_HiddenField");
      Html.AssertAttribute (hiddenField, "value", value);
    }
  }
}