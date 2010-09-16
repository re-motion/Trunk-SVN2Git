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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using System.Web;
using Remotion.Reflection;
using Remotion.Web;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocEnumValueImplementation.Rendering
{
  [TestFixture]
  public class BocEnumValueRendererTest : RendererTestBase
  {
    private IBocEnumValue _enumValue;
    private readonly Unit _width = Unit.Point (173);
    private readonly Unit _height = Unit.Point (17);
    private IEnumerationValueInfo[] _enumerationInfos;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _enumValue = MockRepository.GenerateStub<IBocEnumValue>();
      var businessObjectProvider = BindableObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute));
      var propertyInfo = new PropertyInfoAdapter (typeof (TypeWithEnum).GetProperty ("EnumValue"));
      IBusinessObjectEnumerationProperty property =
          new EnumerationProperty (
              new PropertyBase.Parameters (
                  (BindableObjectProvider) businessObjectProvider, propertyInfo, typeof (TestEnum), typeof (TestEnum), null, true, false)
              );
      _enumValue.Property = property;
      _enumValue.Stub (stub => stub.ClientID).Return ("MyEnumValue");
      _enumValue.Stub (mock => mock.IsDesignMode).Return (false);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      _enumValue.Stub (stub => stub.Page).Return (pageStub);

      var values = new List<EnumerationValueInfo>(3);
      foreach (TestEnum value in Enum.GetValues (typeof (TestEnum)))
        values.Add (new EnumerationValueInfo (value, value.ToString(), value.ToString(), true));
      _enumerationInfos = values.ToArray();
      _enumValue.Stub (mock => mock.GetEnabledValues()).Return (_enumerationInfos);

      _enumValue.Stub (mock => mock.GetNullItemText()).Return ("null");
      _enumValue.Stub (mock => mock.LabelID).Return ("LabelClientID");
      _enumValue.Stub (mock => mock.ListControlID).Return ("ListControlClientID");

      StateBag stateBag = new StateBag ();
      _enumValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _enumValue.Stub (mock => mock.Style).Return (_enumValue.Attributes.CssStyle);
      _enumValue.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _enumValue.Stub (mock => mock.ListControlStyle).Return (new ListControlStyle ());
      _enumValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
    }

    [Test]
    public void RenderNullValue ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);

      AssertOptionList (true, null, false, false, false);
    }

    [Test]
    public void RenderFirstValue ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueAutoPostback ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.IsRequired).Return (true);
      _enumValue.ListControlStyle.AutoPostBack = true;
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, true);
    }

    [Test]
    public void RenderFirstValueWithNullOption ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub(mock=>mock.IsRequired).Return(false);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (true, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderNullValueDisabled ()
    {
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);

      AssertOptionList (true, null, true, false, false);
    }

    [Test]
    public void RenderFirstValueDisabled ()
    {
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueWithNullOptionDisabled ()
    {
      _enumValue.Stub(mock=>mock.IsRequired).Return(false);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (true, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderNullValueReadOnly ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);

      AssertLabel (null, false);
    }

    [Test]
    public void RenderFirstValueReadOnly ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithCssClass ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithCssClass ()
    {
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithCssClass ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.CssClass = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithCssClassInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, false, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithCssClassInAttributes ()
    {
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, false, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithCssClassInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Attributes["class"] = "CssClass";
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    [Test]
    public void RenderFirstValueWithStyle ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, true, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithStyle ()
    {
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, true, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithStyle ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Height = _height;
      _enumValue.Width = _width;
      _enumValue.ControlStyle.Height = _height;
      _enumValue.ControlStyle.Width = _width;
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, true);
    }

    [Test]
    public void RenderFirstValueWithStyleInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, false, true, false);
    }

    [Test]
    public void RenderFirstValueDisabledWithStyleInAttributes ()
    {
      _enumValue.Style["height"] = _height.ToString();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Value = TestEnum.First;

      AssertOptionList (false, TestEnum.First, true, true, false);
    }

    [Test]
    public void RenderFirstValueReadOnlyWithStyleInAttributes ()
    {
      _enumValue.Stub (mock => mock.Enabled).Return (true);
      _enumValue.Style["height"] = _height.ToString ();
      _enumValue.Style["width"] = _width.ToString();
      _enumValue.Stub(mock=>mock.IsRequired).Return(true);
      _enumValue.Stub(mock=>mock.IsReadOnly).Return(true);
      _enumValue.Value = TestEnum.First;
      _enumValue.Stub (mock => mock.EnumerationValueInfo).Return (_enumerationInfos[0]);

      AssertLabel (TestEnum.First, false);
    }

    private void AssertLabel (TestEnum? value, bool withStyle)
    {
      var renderer = new BocEnumValueRenderer (
          MockRepository.GenerateMock<HttpContextBase>(), _enumValue, MockRepository.GenerateStub<IResourceUrlFactory>());
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();
      XmlNode div = GetAssertedSpan (document, true, false, false, renderer);


      var span = Html.GetAssertedChildElement (div, "span", 0);
      Html.AssertAttribute (span, "id", _enumValue.LabelID);

      if (withStyle)
      {
        Html.AssertStyleAttribute (span, "width", _width.ToString());
        Html.AssertStyleAttribute (span, "height", "100%");
      }

      if (value.HasValue)
        Html.AssertTextNode (span, value.Value.ToString(), 0);
      else
        Html.AssertChildElementCount (span, 0);
    }

    private XmlNode GetAssertedSpan (XmlDocument document, bool isReadOnly, bool isDisabled, bool withStyle, BocEnumValueRenderer renderer)
    {
      var div = Html.GetAssertedChildElement (document, "span", 0);
      string cssClass = _enumValue.CssClass;
      if (string.IsNullOrEmpty (cssClass))
        cssClass = _enumValue.Attributes["class"];
      if (string.IsNullOrEmpty (cssClass))
        cssClass = renderer.CssClassBase;

      Html.AssertAttribute (div, "id", "MyEnumValue");
      Html.AssertAttribute (div, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
      if (isReadOnly)
        Html.AssertAttribute (div, "class", renderer.CssClassReadOnly, HtmlHelper.AttributeValueCompareMode.Contains);
      if (isDisabled)
        Html.AssertAttribute (div, "class", renderer.CssClassDisabled, HtmlHelper.AttributeValueCompareMode.Contains);

      if (withStyle)
      {
        Html.AssertStyleAttribute (div, "height", _height.ToString());
        Html.AssertStyleAttribute (div, "width", _width.ToString());
      }

      return div;
    }

    private void AssertOptionList (bool withNullValue, TestEnum? selectedValue, bool isDisabled, bool withStyle, bool autoPostBack)
    {
      var renderer = new BocEnumValueRenderer (
          MockRepository.GenerateMock<HttpContextBase>(), _enumValue, MockRepository.GenerateStub<IResourceUrlFactory>());
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();
      var div = GetAssertedSpan (document, false, false, false, renderer);

      var select = Html.GetAssertedChildElement (div, "select", 0);
      Html.AssertAttribute (select, "id", _enumValue.GetListControlClientID());

      if (withStyle)
        Html.AssertStyleAttribute (select, "height", "100%");

      if (isDisabled)
        Html.AssertAttribute (select, "disabled", "disabled");

      if (withNullValue)
        AssertNullOption (select, !selectedValue.HasValue);

      if (autoPostBack)
        Html.AssertAttribute (select, "onchange", string.Format ("javascript:__doPostBack('{0}','')", _enumValue.ListControlID));

      int index = withNullValue ? 1 : 0;
      foreach (TestEnum value in Enum.GetValues (typeof (TestEnum)))
      {
        AssertOption (select, value.ToString(), value.ToString(), index, selectedValue == value);
        ++index;
      }
    }

    private void AssertOption (XmlNode select, string value, string text, int index, bool isSelected)
    {
      var option = Html.GetAssertedChildElement (select, "option", index);
      Html.AssertAttribute (option, "value", value);

      if (!string.IsNullOrEmpty (text))
        Html.AssertTextNode (option, text, 0);

      if (isSelected)
        Html.AssertAttribute (option, "selected", "selected");
    }

    private void AssertNullOption (XmlNode select, bool isSelected)
    {
      AssertOption (select, _enumValue.GetNullItemText(), "", 0, isSelected);
    }
  }
}