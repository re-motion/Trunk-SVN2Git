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
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocListTableBlockRendererTest : RendererTestBase
  {
    private StubServiceLocator ServiceLocator { get; set; }

    [Test]
    public void RenderPopulatedList ()
    {
      InitializePopulatedList();
      CommonInitialize();

      HtmlNode tbody;
      RenderAndAssertTable (out tbody);

      HtmlNode trData1 = Html.GetAssertedChildElement (tbody, "tr", 0, true);
      Html.AssertAttribute (trData1, "class", "dataStub");

      HtmlNode trData2 = Html.GetAssertedChildElement (tbody, "tr", 1, true);
      Html.AssertAttribute (trData2, "class", "dataStub");
    }


    [Test]
    public void RenderEmptyList ()
    {
      Initialize (false);
      CommonInitialize();
      List.Stub (mock => mock.IsEmptyList).Return (true);
      List.Stub (mock => mock.ShowEmptyListMessage).Return (true);
      List.Stub (mock => mock.ShowEmptyListEditMode).Return (true);

      HtmlNode tbody;
      RenderAndAssertTable(out tbody);

      HtmlNode trData1 = Html.GetAssertedChildElement (tbody, "tr", 0, true);
      Html.AssertAttribute (trData1, "class", "emptyStub");
    }

    [Test]
    public void RenderDummyTable ()
    {
      Initialize (false);
      CommonInitialize ();
      List.Stub (mock => mock.IsEmptyList).Return (true);

      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer (HttpContext, Html.Writer, List, ServiceLocator);
      renderer.Render ();

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode table = Html.GetAssertedChildElement (document.DocumentNode, "table", 0, true);
      HtmlNode tr = Html.GetAssertedChildElement (table, "tr", 0, true);
      HtmlNode td = Html.GetAssertedChildElement (tr, "td", 0, true);
      Html.AssertTextNode (td, HtmlHelper.WhiteSpace, 0, false);
    }

    private void RenderAndAssertTable (out HtmlNode tbody)
    {
      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer (HttpContext, Html.Writer, List, ServiceLocator);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);

      HtmlNode table = Html.GetAssertedChildElement (div, "table", 0, true);

      HtmlNode colgroup = Html.GetAssertedChildElement (table, "colgroup", 0, true);

      Html.GetAssertedChildElement (colgroup, "col", 0, true);
      Html.GetAssertedChildElement (colgroup, "col", 1, true);
      Html.GetAssertedChildElement (colgroup, "col", 2, true);

      HtmlNode thead = Html.GetAssertedChildElement (table, "thead", 1, true);

      HtmlNode trTitle = Html.GetAssertedChildElement (thead, "tr", 0, true);
      Html.AssertAttribute (trTitle, "class", "titleStub");

      tbody = Html.GetAssertedChildElement (table, "tbody", 2, true);
    }

    private void CommonInitialize ()
    {
      ServiceLocator = new StubServiceLocator ();
      ServiceLocator.SetRowRendererFactory (new StubRowRendererFactory ());

      List.Stub (list => list.IsSelectionEnabled).Return (true);
      List.Stub (mock => mock.IsDesignMode).Return (false);
      List.FixedColumns.Add (new StubColumnDefinition ());
      List.FixedColumns.Add (new StubColumnDefinition ());
      List.FixedColumns.Add (new StubColumnDefinition ());
      List.Stub (list => list.GetColumns ()).Return (List.FixedColumns.ToArray ());

      List.Stub (mock => mock.IsPagingEnabled).Return (true);
      List.Stub (mock => mock.PageSize).Return (5);
    }

    private void InitializePopulatedList ()
    {
      Initialize (true);

      var sortingProvider = MockRepository.GenerateStub<IBocListSortingOrderProvider> ();
      IBusinessObject firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject) ((TypeWithReference) BusinessObject).SecondValue;
      BocListRow[] rows = new[]
                          {
                              new BocListRow (sortingProvider, 0, firstObject),
                              new BocListRow (sortingProvider, 1, secondObject)
                          };
      List.Stub (list => list.GetIndexedRows ()).Return (rows);
    }
  }
}