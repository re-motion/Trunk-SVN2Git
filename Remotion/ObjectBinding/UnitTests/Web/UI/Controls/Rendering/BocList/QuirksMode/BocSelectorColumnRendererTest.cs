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
  public class BocSelectorColumnRendererTest : BocListRendererTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.IsSelectionEnabled).Return (true);
    }

    [Test]
    public void RenderTitleCellForMultiSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (HttpContext, Html.Writer, List);
      renderer.RenderTitleCell();

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0, false);
      Html.AssertAttribute (th, "class", List.CssClassTitleCell);

      var input = Html.GetAssertedChildElement (th, "input", 0, false);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "name", "_Boc_SelectorControl_SelectAll");
      Html.AssertAttribute (input, "value", "-1");
      Html.AssertAttribute (input, "alt", "Select all rows.");
    }

    [Test]
    public void RenderDataCellForMultiSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (HttpContext, Html.Writer, List);
      renderer.RenderDataCell (0, "checkboxControl", false, "bocListTableCell");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0, false);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "name", "checkboxControl");
      Html.AssertAttribute (input, "value", "0");
      Html.AssertAttribute (input, "alt", "Select this row.");
    }

    [Test]
    public void RenderTitleCellForSingleSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (HttpContext, Html.Writer, List);
      renderer.RenderTitleCell();

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0, false);
      Html.AssertAttribute (th, "class", List.CssClassTitleCell);

      Html.AssertTextNode (th, HtmlHelper.WhiteSpace, 0, false);
    }

    [Test]
    public void RenderDataCellForSingleSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (HttpContext, Html.Writer, List);
      renderer.RenderDataCell (0, "radioControl", false, "bocListTableCell");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0, false);
      Html.AssertAttribute (input, "type", "radio");
      Html.AssertAttribute (input, "name", "radioControl");
      Html.AssertAttribute (input, "value", "0");
      Html.AssertAttribute (input, "alt", "Select this row.");
    }
  }
}