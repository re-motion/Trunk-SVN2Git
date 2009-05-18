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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocDropDownMenuColumnRendererTest : ColumnRendererTestBase<BocDropDownMenuColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocDropDownMenuColumnDefinition();
      Column.ColumnTitle = "FirstColumn";
      Column.MenuTitleText = "Menu Title";
      Column.MenuTitleIcon = new IconInfo ("~/Images/MenuTitleIcon.gif", 16, 16);

      base.SetUp();

      ((BocListMock) List).IsDesignModeOverrideValue = false;
      ((BocListMock) List).RowMenuDisplay = RowMenuDisplay.Manual;

      List.OnLoad();
    }

    [Test]
    public void RenderCell ()
    {
      IBocColumnRenderer<BocDropDownMenuColumnDefinition> renderer = new BocDropDownMenuColumnRenderer (Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode outerDiv = Html.GetAssertedChildElement (td, "div", 0, false);

      HtmlNode rowMenuDiv = Html.GetAssertedChildElement (outerDiv, "div", 0, true);
      Html.AssertStyleAttribute (rowMenuDiv, "display", "inline-block");

      HtmlNode clickableDiv = Html.GetAssertedChildElement (rowMenuDiv, "div", 0, true);
      Html.AssertAttribute (clickableDiv, "onclick", "DropDownMenu_OnClick (this, 'ctl00_RowMenu_0', null, null);");
      Html.AssertStyleAttribute (clickableDiv, "position", "relative");

      HtmlNode headDiv = Html.GetAssertedChildElement (clickableDiv, "div", 0, true);
      Html.AssertAttribute (headDiv, "class", "dropDownMenuHead");
      Html.AssertAttribute (headDiv, "OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      Html.AssertAttribute (headDiv, "OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      Html.AssertStyleAttribute (headDiv, "position", "relative");

      HtmlNode table = Html.GetAssertedChildElement (headDiv, "table", 0, true);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertStyleAttribute (table, "display", "inline");

      HtmlNode row = Html.GetAssertedChildElement (table, "tr", 0, true);

      HtmlNode titleCell = Html.GetAssertedChildElement (row, "td", 0, true);
      Html.AssertAttribute (titleCell, "class", "dropDownMenuHeadTitle");
      Html.AssertStyleAttribute (titleCell, "width", "1%");

      HtmlNode iconLink = Html.GetAssertedChildElement (titleCell, "a", 0, true);
      HtmlNode iconImage = Html.GetAssertedChildElement (iconLink, "img", 0, true);
      Html.AssertStyleAttribute (iconImage, "vertical-align", "middle");
      Html.AssertStyleAttribute (iconImage, "border-style", "none");
      Html.AssertStyleAttribute (iconImage, "margin-right", "0.3em");

      Html.AssertTextNode (iconLink, "Menu Title", 0, true);

      HtmlNode spacerCell = Html.GetAssertedChildElement (row, "td", 1, true);
      Html.AssertStyleAttribute (spacerCell, "width", "0%");
      Html.AssertStyleAttribute (spacerCell, "padding-right", "0.3em");

      HtmlNode buttonCell = Html.GetAssertedChildElement (row, "td", 2, true);
      Html.AssertAttribute (buttonCell, "class", "dropDownMenuHeadButton");
      Html.AssertStyleAttribute (buttonCell, "width", "0%");
      Html.AssertStyleAttribute (buttonCell, "text-align", "center");

      HtmlNode buttonLink = Html.GetAssertedChildElement (buttonCell, "a", 0, true);
      Html.AssertAttribute (buttonLink, "href", "#");
      Html.AssertStyleAttribute (buttonLink, "width", "1em");

      HtmlNode buttonImage = Html.GetAssertedChildElement (buttonLink, "img", 0, true);
      Html.AssertStyleAttribute (buttonImage, "vertical-align", "middle");
      Html.AssertStyleAttribute (buttonImage, "border-style", "none");
    }
  }
}