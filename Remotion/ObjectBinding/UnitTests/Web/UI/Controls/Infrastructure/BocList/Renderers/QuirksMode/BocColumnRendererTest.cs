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
using System.Globalization;
using System.Threading;
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Renderers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Renderers.QuirksMode
{
  [TestFixture]
  public class BocColumnRendererTest
  {
    private HtmlHelper Html { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }
    private BocColumnDefinition Column { get; set; }

    [SetUp]
    public void SetUp ()
    {
      List = new ObjectBinding.Web.UI.Controls.BocList();
      Column = new BocSimpleColumnDefinition();
      Column.ColumnTitle = "TestColumn1";
      BocListView listView = new BocListView (List, new[] { Column });
      List.AvailableViews.Add (listView);

      Html = new HtmlHelper();
      Html.InitializeStream();
    }

    [Test]
    public void RenderTitleCellAscendingZero ()
    {
      RenderTitleCell (SortingDirection.Ascending, 0, "bocListTitleCell", "bocListSortingOrder", "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingZero ()
    {
      RenderTitleCell (SortingDirection.Descending, 0, "bocListTitleCell", "bocListSortingOrder", "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellAscendingThree ()
    {
      RenderTitleCell (SortingDirection.Ascending, 3, "bocListTitleCell", "bocListSortingOrder", "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingFour ()
    {
      RenderTitleCell (SortingDirection.Descending, 4, "bocListTitleCell", "bocListSortingOrder", "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellAscendingZeroGerman ()
    {
      RenderTitleCellForCulture (
          new CultureInfo ("de-AT"),
          SortingDirection.Ascending,
          0,
          "bocListTitleCell",
          "bocListSortingOrder",
          "SortAscending.gif",
          "Aufsteigend sortiert.");
    }

    [Test]
    public void RenderTitleCellDescendingThreeGerman ()
    {
      RenderTitleCellForCulture (
          new CultureInfo ("de-AT"),
          SortingDirection.Descending,
          3,
          "bocListTitleCell",
          "bocListSortingOrder",
          "SortDescending.gif",
          "Absteigend sortiert.");
    }

    private void RenderTitleCellForCulture (
        CultureInfo culture,
        SortingDirection sortDirection,
        int sortIndex,
        string titleCellCssClass,
        string sortOrderSpanCssClass,
        string iconFilename,
        string iconAltText)
    {
      CultureInfo backupCulture = CultureInfo.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = culture;

      try
      {
        RenderTitleCell (sortDirection, sortIndex, titleCellCssClass, sortOrderSpanCssClass, iconFilename, iconAltText);
      }
      finally
      {
        Thread.CurrentThread.CurrentUICulture = backupCulture;
      }
    }

    private void RenderTitleCell (
        SortingDirection sortDirection,
        int sortIndex,
        string titleCellCssClass,
        string sortOrderSpanCssClass,
        string iconFilename,
        string iconAltText)
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (List, Html.Writer, (BocSimpleColumnDefinition) Column);
      renderer.RenderTitleCell (sortDirection, sortIndex);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode th = Html.GetAssertedChildElement (document.DocumentNode, "th", 0, false);
      Html.AssertAttribute (th, "class", titleCellCssClass);

      Assert.Less (0, th.ChildNodes.Count);
      HtmlNode outerSpan = Html.GetAssertedChildElement (th, "span", 0, false);
      Html.AssertTextNode (outerSpan, Column.ColumnTitleDisplayValue + HtmlHelper.WhiteSpace, 0, false);

      HtmlNode sortOrderSpan = Html.GetAssertedChildElement (outerSpan, "span", 1, false);
      Html.AssertAttribute (sortOrderSpan, "class", sortOrderSpanCssClass);

      HtmlNode sortIcon = Html.GetAssertedChildElement (sortOrderSpan, "img", 0, false);
      Html.AssertAttribute (sortIcon, "src", iconFilename, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (sortIcon, "alt", iconAltText);
      Html.AssertStyleAttribute (sortIcon, "vertical-align", "middle");
      Html.AssertStyleAttribute (sortIcon, "border-style", "none");

      Html.AssertTextNode (sortOrderSpan, HtmlHelper.WhiteSpace + (sortIndex + 1), 1, false);
    }
  }
}