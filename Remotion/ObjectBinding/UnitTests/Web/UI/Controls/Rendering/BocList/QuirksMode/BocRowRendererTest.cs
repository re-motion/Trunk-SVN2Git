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
using System.Collections;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  [TestFixture]
  public class BocRowRendererTest : BocListRendererTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      List.FixedColumns.Add (new StubColumnDefinition());
      List.Stub (mock => mock.GetColumns()).Return (List.FixedColumns.ToArray());
      List.Stub (mock => mock.AreDataRowsClickSensitive()).Return (true);

      List.Stub (mock => mock.SortingOrder).Return (new ArrayList { SortingDirection.Ascending });
    }

    [Test]
    public void RenderTitlesRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderTitlesRow();


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderTitlesRow();


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      var thIndex = Html.GetAssertedChildElement (tr, "th", 0);
      Html.AssertAttribute (thIndex, "class", List.CssClassTitleCell, HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (thIndex, "class", List.CssClassTitleCellIndex, HtmlHelper.AttributeValueCompareMode.Contains);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderTitlesRow();


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderDataRow (BusinessObject, 0, 0, 0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", List.CssClassDataRow);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderDataRowSelected ()
    {
      List.SelectorControlCheckedState.Add (0);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderDataRow (BusinessObject, 0, 0, 0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", List.CssClassDataRowSelected);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRow ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());
      renderer.RenderEmptyListDataRow();

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "td", 0);
    }
  }
}