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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocIndexColumnQuirksModeRendererTest : BocListRendererTestBase
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

      IBocIndexColumnRenderer renderer = new BocIndexColumnQuirksModeRenderer (HttpContext, List, BocListQuirksModeCssClassDefinition.Instance);
      renderer.RenderTitleCell (Html.Writer);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", BocListQuirksModeCssClassDefinition.Instance.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", BocListQuirksModeCssClassDefinition.Instance.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      var span = Html.GetAssertedChildElement (th, "span", 0);
      Html.AssertTextNode (span, "No.", 0);
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
      IBocIndexColumnRenderer renderer = new BocIndexColumnQuirksModeRenderer (HttpContext, List, BocListQuirksModeCssClassDefinition.Instance);
      const string cssClassTableCell = "bocListTableCell";
      renderer.RenderDataCell (Html.Writer, 0, "selectorID", 0, cssClassTableCell);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", cssClassTableCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (td, "class", BocListQuirksModeCssClassDefinition.Instance.DataCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      var label = Html.GetAssertedChildElement (td, "label", 0);
      Html.AssertAttribute (label, "class", BocListQuirksModeCssClassDefinition.Instance.Content);
      Html.AssertAttribute (label, "for", "selectorID");

      Html.AssertTextNode (label, (1 + indexOffset).ToString(), 0);
    }
  }
}