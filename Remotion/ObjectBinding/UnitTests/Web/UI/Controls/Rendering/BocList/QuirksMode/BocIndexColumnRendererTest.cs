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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  [TestFixture]
  public class BocIndexColumnRendererTest : BocListRendererTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.IsIndexEnabled).Return (true);
    }

    [Test]
    public void RenderIndexTitleCell ()
    {
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);

      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer (HttpContext, Html.Writer, List);
      renderer.RenderTitleCell();

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0, false);
      Html.AssertAttribute (th, "class", List.CssClassTitleCell, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", List.CssClassTitleCellIndex, HtmlHelper.AttributeValueCompareMode.Contains);

      var span = Html.GetAssertedChildElement (th, "span", 0, false);
      Html.AssertTextNode (span, "No.", 0, false);
    }

    [Test]
    public void RenderIndexDataCellInitialOrder ()
    {
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);

      RenderIndexDataCell (0);
    }

    [Test]
    public void RenderIndexDataCellSortedOrderAndIndexOffset ()
    {
      List.Stub (mock => mock.Index).Return (RowIndex.SortedOrder);
      List.Stub (mock => mock.IndexOffset).Return (2);

      RenderIndexDataCell (2);
    }

    private void RenderIndexDataCell (int indexOffset)
    {
      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer (HttpContext, Html.Writer, List);
      const string cssClassTableCell = "bocListTableCell";
      renderer.RenderDataCell (0, "selectorID", 0, cssClassTableCell);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0, false);
      Html.AssertAttribute (td, "class", cssClassTableCell, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (td, "class", List.CssClassDataCellIndex, HtmlHelper.AttributeValueCompareMode.Contains);

      var label = Html.GetAssertedChildElement (td, "label", 0, false);
      Html.AssertAttribute (label, "class", List.CssClassContent);
      Html.AssertAttribute (label, "for", "selectorID");

      Html.AssertTextNode (label, (1 + indexOffset).ToString(), 0, false);
    }
  }
}