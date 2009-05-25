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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocColumnRendererTest : RendererTestBase
  {
    private const string c_columnCssClass = "cssClassColumn";

    private BocColumnDefinition Column { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.ColumnTitle = "TestColumn1";
      Column.CssClass = c_columnCssClass;

      Initialize ();

      List.Stub (mock => mock.GetColumns()).Return (new[] { Column });

      var editModeController = MockRepository.GenerateMock<IEditModeController>();
      editModeController.Stub (mock => mock.RenderTitleCellMarkers (Html.Writer, Column, 0)).Do (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write (string.Empty));

      List.Stub (mock => mock.EditModeController).Return (editModeController);

      List.Stub (mock => mock.IsClientSideSortingEnabled).Return (true);
      List.Stub (mock => mock.IsShowSortingOrderEnabled).Return (true);
    }

    [Test]
    public void RenderTitleCellAscendingZero ()
    {
      RenderTitleCell (SortingDirection.Ascending, 0, "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingZero ()
    {
      RenderTitleCell (SortingDirection.Descending, 0, "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellAscendingThree ()
    {
      RenderTitleCell (SortingDirection.Ascending, 3, "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingFour ()
    {
      RenderTitleCell (SortingDirection.Descending, 4, "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellNoSorting ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, (BocSimpleColumnDefinition) Column);
      renderer.RenderTitleCell (SortingDirection.None, -1);

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode th = Html.GetAssertedChildElement (document.DocumentNode, "th", 0, false);
      Html.AssertAttribute (th, "class", List.CssClassTitleCell, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", c_columnCssClass, HtmlHelper.AttributeValueCompareMode.Contains);

      Assert.Less (0, th.ChildNodes.Count);
      HtmlNode sortCommandLink = Html.GetAssertedChildElement (th, "a", 0, false);
      Html.AssertTextNode (sortCommandLink, Column.ColumnTitleDisplayValue, 0, false);

      Html.AssertChildElementCount (sortCommandLink, 0);
    }

    private void RenderTitleCell (
        SortingDirection sortDirection,
        int sortIndex,
        string iconFilename,
        string iconAltText)
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, (BocSimpleColumnDefinition) Column);
      renderer.RenderTitleCell (sortDirection, sortIndex);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode th = Html.GetAssertedChildElement (document.DocumentNode, "th", 0, false);
      Html.AssertAttribute (th, "class", List.CssClassTitleCell, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", c_columnCssClass, HtmlHelper.AttributeValueCompareMode.Contains);

      Assert.Less (0, th.ChildNodes.Count);
      HtmlNode sortCommandLink = Html.GetAssertedChildElement (th, "a", 0, false);
      Html.AssertTextNode (sortCommandLink, Column.ColumnTitleDisplayValue + HtmlHelper.WhiteSpace, 0, false);

      HtmlNode sortOrderSpan = Html.GetAssertedChildElement (sortCommandLink, "span", 1, false);
      Html.AssertAttribute (sortOrderSpan, "class", List.CssClassSortingOrder, HtmlHelper.AttributeValueCompareMode.Contains);

      HtmlNode sortIcon = Html.GetAssertedChildElement (sortOrderSpan, "img", 0, false);
      Html.AssertAttribute (sortIcon, "src", iconFilename, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (sortIcon, "alt", iconAltText);
      Html.AssertStyleAttribute (sortIcon, "vertical-align", "middle");
      Html.AssertStyleAttribute (sortIcon, "border-style", "none");

      Html.AssertTextNode (sortOrderSpan, HtmlHelper.WhiteSpace + (sortIndex + 1), 1, false);
    }
  }
}