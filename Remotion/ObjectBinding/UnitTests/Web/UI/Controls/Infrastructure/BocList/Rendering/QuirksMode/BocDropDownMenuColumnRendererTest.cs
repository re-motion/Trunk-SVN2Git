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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocDropDownMenuColumnRendererTest : ColumnRendererTestBase<BocDropDownMenuColumnDefinition>
  {
    private DropDownMenu Menu { get; set; }

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocDropDownMenuColumnDefinition();
      Column.ColumnTitle = "FirstColumn";
      Column.MenuTitleText = "Menu Title";
      Column.MenuTitleIcon = new IconInfo ("~/Images/MenuTitleIcon.gif", 16, 16);

      base.SetUp();

      List.Stub (mock => mock.HasMenuBlock).Return (true);
      List.Stub (mock => mock.RowMenuDisplay).Return (RowMenuDisplay.Manual);


      Menu = MockRepository.GenerateMock<DropDownMenu> (List);
      Menu.Stub (menuMock => menuMock.RenderControl (Html.Writer)).Do (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write ("mocked dropdown menu"));
    }

    [Test]
    public void RenderCellWithPopulatedMenu ()
    {
      InitializeRowMenus ();
      Menu.MenuItems.Add (
          new WebMenuItem (
              "itemId",
              "category",
              "text",
              new IconInfo ("~/Images/NullImage.gif"),
              new IconInfo ("~/Images/NullImage.gif"),
              WebMenuItemStyle.Text,
              RequiredSelection.Any,
              false,
              new Command()));

      IBocColumnRenderer<BocDropDownMenuColumnDefinition> renderer = new BocDropDownMenuColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode div = Html.GetAssertedChildElement (td, "div", 0, false);
      Html.AssertAttribute (div, "onclick", "BocList_OnCommandClick();");

      Html.AssertTextNode (div, "mocked dropdown menu", 0, false);
    }

    [Test]
    public void RenderCellWithEmptyMenu ()
    {
      InitializeRowMenus ();

      IBocColumnRenderer<BocDropDownMenuColumnDefinition> renderer = new BocDropDownMenuColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      Html.AssertChildElementCount (td, 0);
    }

    [Test]
    public void RenderCellWithNullMenu ()
    {
      IBocColumnRenderer<BocDropDownMenuColumnDefinition> renderer = new BocDropDownMenuColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      Html.AssertChildElementCount (td, 0);
    }

    private void InitializeRowMenus ()
    {
      BocListRowMenuTuple[] rowMenus = new[]
                                       {
                                           new BocListRowMenuTuple (BusinessObject, 0, Menu),
                                           new BocListRowMenuTuple (BusinessObject, 1, Menu)
                                       };
      List.Stub (mock => mock.RowMenus).Return (rowMenus);
    }
  }
}