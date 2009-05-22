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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocListMenuBlockRendererTest : RendererTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      Initialize ();
    }

    [Test]
    public void RenderWithAvailableViews ()
    {
      DropDownList dropDownList = MockRepository.GenerateMock<DropDownList>();
      List.Stub (mock => mock.AvailableViewsList).Return (dropDownList);
      List.Stub (mock => mock.HasAvailableViewsList).Return (true);
      List.Stub (mock => mock.AvailableViewsListTitle).Return ("Views List Title");
      List.Stub (mock => mock.CssClassAvailableViewsListLabel).Return ("CssClass");

      dropDownList.Stub (mock => mock.RenderControl (Html.Writer)).Do (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write ("mocked dropdown list"));

      var renderer = new BocListMenuBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "margin-bottom", "5pt");

      HtmlNode span = Html.GetAssertedChildElement (div, "span", 0, true);
      Html.AssertAttribute (span, "class", "CssClass");
      Html.AssertTextNode (span, "Views List Title", 0, false);

      Html.AssertTextNode (div, HtmlHelper.WhiteSpace + "mocked dropdown list", 1, true);
    }

    [Test]
    public void RenderWithOptions ()
    {
      DropDownMenu optionsMenu = MockRepository.GenerateStub<DropDownMenu> (List);
      List.Stub (mock => mock.OptionsMenu).Return (optionsMenu);
      List.Stub (mock => mock.HasOptionsMenu).Return (true);
      List.Stub (mock => mock.OptionsTitle).Return ("Options Menu Title");
      // can't assert this because CssStyleCollection is sealed and has only internal constructors
      // List.Stub (mock => mock.MenuBlockItemOffset).Return (new Unit (7, UnitType.Pixel));

      optionsMenu.Stub (menuMock => menuMock.RenderControl (Html.Writer)).Do (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write ("mocked dropdown menu"));

      var renderer = new BocListMenuBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();
      Html.AssertTextNode (document.DocumentNode, "mocked dropdown menu", 0, false);

      Assert.IsFalse (optionsMenu.IsReadOnly);
      Assert.IsTrue (optionsMenu.Enabled);
    }

    [Test]
    public void RenderWithListMenu ()
    {
      var menuItemCollection = new WebMenuItemCollection (List);
      List.Stub (mock => mock.ListMenuItems).Return (menuItemCollection);
      List.Stub (mock => mock.HasListMenu).Return (true);
      
      Unit menuBlockOffset = new Unit (3, UnitType.Pixel);
      List.Stub (mock => mock.MenuBlockItemOffset).Return (menuBlockOffset);

      WebMenuItem item = new WebMenuItem (
          "itemId",
          "category",
          "text",
          new IconInfo ("~/Images/NullIcon.gif"),
          new IconInfo ("~/Images/NullIcon.gif"),
          WebMenuItemStyle.IconAndText,
          RequiredSelection.Any,
          false,
          new Command());

      menuItemCollection.Add (item);

      var renderer = new BocListMenuBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "margin-bottom", menuBlockOffset.ToString());

      HtmlNode table = Html.GetAssertedChildElement (div, "table", 0, true);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertAttribute (table, "border", "0");

      HtmlNode tr = Html.GetAssertedChildElement (table, "tr", 0, true);

      HtmlNode td = Html.GetAssertedChildElement (tr, "td", 0, true);
      Html.AssertAttribute (td, "class", "contentMenuRow");
      Html.AssertStyleAttribute (td, "width", "100%");

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);

      HtmlNode a = Html.GetAssertedChildElement (span, "a", 0, false);

      HtmlNode img = Html.GetAssertedChildElement (a, "img", 0, false);
      Html.AssertAttribute (img, "src", "/Images/NullIcon.gif", HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (img, "vertical-align", "middle");
      Html.AssertStyleAttribute (img, "border-style", "none");
      
      Html.AssertTextNode (a, HtmlHelper.WhiteSpace+"text", 1, false);
    }
  }
}