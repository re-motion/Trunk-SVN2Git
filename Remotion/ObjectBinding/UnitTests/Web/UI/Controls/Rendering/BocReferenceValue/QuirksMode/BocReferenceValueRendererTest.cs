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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.QuirksMode;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue.QuirksMode
{
  [TestFixture]
  public class BocReferenceValueRendererTest : RendererTestBase
  {
    private static readonly Unit s_width = Unit.Pixel (250);
    private static readonly Unit s_height = Unit.Point (12);

    private IBocReferenceValue _control;
    private TypeWithReference _businessObject;
    private IBusinessObjectProvider _provider;
    private BusinessObjectReferenceDataSource _dataSource;

    private StubDropDownMenu _optionsMenu;
    private IClientScriptManager _clientScriptManagerMock;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _optionsMenu = new StubDropDownMenu();

      _control = MockRepository.GenerateStub<IBocReferenceValue>();
      _control.Stub (stub => stub.ClientID).Return ("MyReferenceValue");
      _control.Stub (stub => stub.Command).Return (new BocCommand());
      _control.Command.Type = CommandType.Event;
      _control.Command.Show = CommandShow.Always;

      _control.Stub (stub => stub.OptionsMenu).Return (_optionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      _control.Stub (stub => stub.Page).Return (pageStub);

      _clientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      pageStub.Stub (stub => stub.ClientScript).Return (_clientScriptManagerMock);

      _businessObject = TypeWithReference.Create ("MyBusinessObject");
      _businessObject.ReferenceList = new[]
                                      {
                                          TypeWithReference.Create ("ReferencedObject 0"),
                                          TypeWithReference.Create ("ReferencedObject 1"),
                                          TypeWithReference.Create ("ReferencedObject 2")
                                      };
      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;

      _provider = ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService>(new ReflectionBusinessObjectWebUIService());

      StateBag stateBag = new StateBag ();
      _control.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (mock => mock.Style).Return (_control.Attributes.CssStyle);
      _control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      _control.Stub (mock => mock.DropDownListStyle).Return (new DropDownListStyle ());
      _control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      _control.Stub (stub => stub.LabelClientID).Return (_control.ClientID + "_Boc_Label");
      _control.Stub (stub => stub.DropDownListClientID).Return (_control.ClientID + "_Boc_DropDownList");
      _control.Stub (stub => stub.IconClientID).Return (_control.ClientID + "_Boc_Icon");

      _control.Stub (stub => stub.GetLabelText()).Return ("MyText");
    }

    [TearDown]
    public void TearDown ()
    {
      _clientScriptManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      SetUpClientScriptExpectations ();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations ();
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (_optionsMenu.Style["width"], Is.Null);
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      _control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      _control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (_optionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (_optionsMenu.Style["width"], Is.Null);
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations();
      
      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      SetUpClientScriptExpectations ();
      SetValue();
      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    private void SetValue ()
    {
      _businessObject.ReferenceValue = _businessObject.ReferenceList[0];
      _control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) _businessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations ();
      SetValue();
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (_optionsMenu.Style["width"], Is.Null);
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      SetValue();
      _control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      SetValue();
      _control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (_optionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      SetValue();
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      SetValue();
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      SetValue();
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (_optionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      SetValue();
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      SetValue();
      _control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (_optionsMenu.Style["width"], Is.Null);
      Assert.That (_optionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      SetValue();
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Html.Writer, _control, () => new StubDropDownList());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle();
      Html.Writer.RenderEndTag();


      var document = Html.GetResultDocument();
      AssertRow (document, false, false, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      _control.Stub (stub => stub.EnableIcon).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueRenderer (HttpContext, Html.Writer, _control, () => new StubDropDownList());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle();
      Html.Writer.RenderEndTag();


      var document = Html.GetResultDocument();
      AssertRow (document, true, false, false);
    }

    [Test]
    public void RenderOptionsReadOnlyWithStyle ()
    {
      AddStyle();
      _control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueRenderer (HttpContext, Html.Writer, _control);
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle();
      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      AssertRow (document, true, false, true);
    }

    private XmlNode GetAssertedDiv (int expectedChildElements, bool withStyle)
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Html.Writer, _control, () => new StubDropDownList());
      renderer.Render();

      var document = Html.GetResultDocument();
      var div = document.GetAssertedChildElement ("div", 0);
      div.AssertAttributeValueContains ("class", "bocReferenceValue");
      if (_control.IsReadOnly)
        div.AssertAttributeValueContains ("class", "readOnly");

      div.AssertStyleAttribute ("display", "inline");
      if (withStyle)
      {
        div.AssertStyleAttribute ("height", s_height.ToString());
        div.AssertStyleAttribute ("width", s_width.ToString());
      }

      div.AssertChildElementCount (expectedChildElements);
      return div;
    }

    private XmlNode GetAssertedTable (XmlNode div, bool withStyle)
    {
      var table = div.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("cellspacing", "0");
      table.AssertAttributeValueEquals ("cellpadding", "0");
      table.AssertAttributeValueEquals ("border", "0");

      table.AssertStyleAttribute ("display", "inline");

      if (withStyle)
      {
        table.AssertStyleAttribute ("width", "100%");
        if (!_control.IsReadOnly)
          table.AssertStyleAttribute ("height", "100%");
      }
      else if (!_control.IsReadOnly)
        table.AssertStyleAttribute ("width", "150pt");

      table.AssertChildElementCount (1);
      return table;
    }

    private void AssertRow (XmlNode table, bool hasLabel, bool hasIcon, bool hasDummyCell)
    {
      var row = table.GetAssertedChildElement ("tr", 0);

      int cellCount = 1;
      if (_control.HasOptionsMenu)
        cellCount++;
      if (hasIcon)
        cellCount++;
      if (hasDummyCell)
        cellCount++;

      row.AssertChildElementCount (cellCount);

      if (hasIcon)
        AssertIconCell (row);

      AssertValueCell (row, hasLabel, hasIcon ? 1 : 0);

      if (_control.HasOptionsMenu)
        AssertMenuCell (row);

      if (hasDummyCell)
      {
        var cell = row.GetAssertedChildElement ("td", cellCount - 1);
        cell.AssertStyleAttribute ("width", "1%");
        cell.AssertChildElementCount (0);
      }
    }

    private void AssertIconCell (XmlNode row)
    {
      var iconCell = row.GetAssertedChildElement ("td", 0);
      iconCell.AssertAttributeValueEquals ("class", "bocReferenceValueContent");
      iconCell.AssertStyleAttribute ("width", "0%");
      iconCell.AssertStyleAttribute ("padding-right", "0.3em");
      iconCell.AssertChildElementCount (1);

      if (_control.IsCommandEnabled (_control.IsReadOnly))
      {
        var link = iconCell.GetAssertedChildElement ("a", 0);
        link.AssertAttributeValueEquals ("href", "#");
        link.AssertAttributeValueEquals ("onclick", "");
        link.AssertChildElementCount (1);

        var icon = link.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/Remotion.ObjectBinding.UnitTests.Web.Domain.TypeWithReference.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
      else
      {
        var icon = iconCell.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/NullIcon.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
    }

    private void AssertValueCell (XmlNode row, bool hasLabel, int index)
    {
      var valueCell = row.GetAssertedChildElement ("td", index);
      valueCell.AssertAttributeValueEquals ("class", "bocReferenceValueContent");
      if (_control.IsReadOnly)
        valueCell.AssertStyleAttribute ("width", "auto");
      else
        valueCell.AssertStyleAttribute ("width", "100%");

      valueCell.AssertChildElementCount (hasLabel ? 1 : 0);
      if (hasLabel)
      {
        if (_control.IsCommandEnabled (_control.IsReadOnly))
        {
          var link = valueCell.GetAssertedChildElement ("a", 0);
          link.AssertAttributeValueEquals ("href", "#");
          link.AssertAttributeValueEquals ("onclick", "");
          link.AssertChildElementCount (1);

          var label = link.GetAssertedChildElement ("span", 0);
          label.AssertAttributeValueEquals ("id", _control.ClientID + "_Boc_Label");
          label.AssertTextNode (_control.Value.DisplayName, 0);
        }
        else
        {
          var label = valueCell.GetAssertedChildElement ("span", 0);
          label.AssertAttributeValueEquals ("id", _control.ClientID + "_Boc_Label");
          label.AssertTextNode ("MyText", 0);
        }
      }
      else
        valueCell.AssertTextNode ("DropDownList", 0);
    }

    private void AssertMenuCell (XmlNode row)
    {
      var menuCell = row.GetAssertedChildElement ("td", 1);
      menuCell.AssertStyleAttribute ("width", "100%");
      menuCell.AssertStyleAttribute ("padding-left", "0.3em");
      menuCell.AssertStyleAttribute ("width", "0%");
      menuCell.AssertChildElementCount (0);
      menuCell.AssertTextNode ("DropDownMenu", 0);
    }

    private void AddStyle ()
    {
      _control.Height = s_height;
      _control.Width = s_width;
      _control.Style["height"] = _control.Height.ToString();
      _control.Style["width"] = _control.Width.ToString ();
    }

    private void SetUpClientScriptExpectations ()
    {
      _clientScriptManagerMock.Expect (mock => mock.GetPostBackEventReference (_control, string.Empty)).Return ("PostBackEventReference");
    }

    private void SetUpGetIconExpectations ()
    {
      _control.Expect (mock => mock.GetIcon (null, null)).IgnoreArguments ().Return (new IconInfo ("~/Images/NullIcon.gif"));
    }
  }
}