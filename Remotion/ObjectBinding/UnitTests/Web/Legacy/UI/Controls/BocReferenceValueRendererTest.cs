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
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls
{
  [TestFixture]
  public class BocReferenceValueRendererTest : BocReferenceValueRendererTestBase
  {
    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      DropDownList.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();

      Control.Stub (stub => stub.DropDownListStyle).Return (new DropDownListStyle ());
      Control.DropDownListStyle.AutoPostBack = true;

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
      Assert.IsTrue (DropDownList.AutoPostBack);
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      SetUpClientScriptExpectations();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (OptionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
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
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      SetUpClientScriptExpectations();
      SetValue();
      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations();
      SetValue();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      SetValue();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      SetValue();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (OptionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      SetValue();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      SetValue();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      SetValue();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
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
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      SetValue();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new BocReferenceValueQuirksModeRenderer (HttpContext, Control, () => new StubDropDownList());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag();


      var document = Html.GetResultDocument();
      AssertRow (document, false, false, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueQuirksModeRenderer (HttpContext, Control, () => new StubDropDownList());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag();


      var document = Html.GetResultDocument();
      AssertRow (document, true, false, false);
    }

    [Test]
    public void RenderOptionsReadOnlyWithStyle ()
    {
      AddStyle();
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueQuirksModeRenderer (HttpContext, Control);
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      AssertRow (document, true, false, true);
    }

    private XmlNode GetAssertedDiv (int expectedChildElements, bool withStyle)
    {
      var renderer = new BocReferenceValueQuirksModeRenderer (HttpContext, Control, () => DropDownList);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();
      var div = document.GetAssertedChildElement ("div", 0);
      div.AssertAttributeValueEquals ("id", "MyReferenceValue");
      div.AssertAttributeValueContains ("class", "bocReferenceValue");
      if (Control.IsReadOnly)
        div.AssertAttributeValueContains ("class", "readOnly");

      div.AssertStyleAttribute ("display", "inline");
      if (withStyle)
      {
        div.AssertStyleAttribute ("height", Height.ToString());
        div.AssertStyleAttribute ("width", Width.ToString());
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
        if (!Control.IsReadOnly)
          table.AssertStyleAttribute ("height", "100%");
      }
      else if (!Control.IsReadOnly)
        table.AssertStyleAttribute ("width", "150pt");

      table.AssertChildElementCount (1);
      return table;
    }

    private void AssertRow (XmlNode table, bool hasLabel, bool hasIcon, bool hasDummyCell)
    {
      var row = table.GetAssertedChildElement ("tr", 0);

      int cellCount = 1;
      if (Control.HasOptionsMenu)
        cellCount++;
      if (hasIcon)
        cellCount++;
      if (hasDummyCell)
        cellCount++;

      row.AssertChildElementCount (cellCount);

      if (hasIcon)
        AssertIconCell (row);

      AssertValueCell (row, hasLabel, hasIcon ? 1 : 0);

      if (Control.HasOptionsMenu)
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

      AssertIcon(iconCell, false);
    }

    private void AssertValueCell (XmlNode row, bool hasLabel, int index)
    {
      var valueCell = row.GetAssertedChildElement ("td", index);
      valueCell.AssertAttributeValueEquals ("class", "bocReferenceValueContent");
      if (Control.IsReadOnly)
        valueCell.AssertStyleAttribute ("width", "auto");
      else
        valueCell.AssertStyleAttribute ("width", "100%");

      valueCell.AssertChildElementCount (hasLabel ? 1 : 0);
      if (hasLabel)
      {
        if (Control.IsCommandEnabled (Control.IsReadOnly))
        {
          var link = valueCell.GetAssertedChildElement ("a", 0);
          link.AssertAttributeValueEquals ("href", "#");
          link.AssertAttributeValueEquals ("onclick", "");
          link.AssertChildElementCount (1);

          var label = link.GetAssertedChildElement ("span", 0);
          label.AssertAttributeValueEquals ("id", Control.ClientID + "_Boc_Label");
          label.AssertTextNode (Control.Value.DisplayName, 0);
        }
        else
        {
          var label = valueCell.GetAssertedChildElement ("span", 0);
          label.AssertAttributeValueEquals ("id", Control.ClientID + "_Boc_Label");
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
  }
}