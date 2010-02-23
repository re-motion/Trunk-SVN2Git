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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.StandardMode
{
  [TestFixture]
  public class BocListTableBlockRendererTest : BocListRendererTestBase
  {
    private StubServiceLocator ServiceLocator { get; set; }

    [Test]
    public void RenderPopulatedList ()
    {
      InitializePopulatedList();
      CommonInitialize();

      XmlNode tbody;
      RenderAndAssertTable (out tbody);

      var trData1 = Html.GetAssertedChildElement (tbody, "tr", 0);
      Html.AssertAttribute (trData1, "class", "dataStub");

      var trData2 = Html.GetAssertedChildElement (tbody, "tr", 1);
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

      XmlNode tbody;
      RenderAndAssertTable (out tbody);

      var trData1 = Html.GetAssertedChildElement (tbody, "tr", 0);
      Html.AssertAttribute (trData1, "class", "emptyStub");
    }

    [Test]
    public void RenderDummyTable ()
    {
      Initialize (false);
      CommonInitialize();
      List.Stub (mock => mock.IsEmptyList).Return (true);

      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer (HttpContext, Html.Writer, List, CssClassContainer.Instance, ServiceLocator);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var table = Html.GetAssertedChildElement (document, "table", 0);
      var tr = Html.GetAssertedChildElement (table, "tr", 0);
      var td = Html.GetAssertedChildElement (tr, "td", 0);
      Html.AssertTextNode (td, HtmlHelper.WhiteSpace, 0);
    }

    private void RenderAndAssertTable (out XmlNode tbody)
    {
      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer (HttpContext, Html.Writer, List, CssClassContainer.Instance, ServiceLocator);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);

      var table = Html.GetAssertedChildElement (div, "table", 0);

      var colgroup = Html.GetAssertedChildElement (table, "colgroup", 0);

      Html.GetAssertedChildElement (colgroup, "col", 0);
      Html.GetAssertedChildElement (colgroup, "col", 1);
      Html.GetAssertedChildElement (colgroup, "col", 2);

      var thead = Html.GetAssertedChildElement (table, "thead", 1);

      var trTitle = Html.GetAssertedChildElement (thead, "tr", 0);
      Html.AssertAttribute (trTitle, "class", "titleStub");

      tbody = Html.GetAssertedChildElement (table, "tbody", 2);
    }

    private void CommonInitialize ()
    {
      ServiceLocator = new StubServiceLocator();
      ServiceLocator.SetRowRendererFactory (new StubRowRendererFactory());

      List.Stub (list => list.IsSelectionEnabled).Return (true);
      List.Stub (mock => mock.IsDesignMode).Return (false);
      List.FixedColumns.Add (new StubColumnDefinition());
      List.FixedColumns.Add (new StubColumnDefinition());
      List.FixedColumns.Add (new StubColumnDefinition());
      List.Stub (list => list.GetColumns()).Return (List.FixedColumns.ToArray());

      List.Stub (mock => mock.IsPagingEnabled).Return (true);
      List.Stub (mock => mock.PageSize).Return (5);
    }

    private void InitializePopulatedList ()
    {
      Initialize (true);

      var sortingProvider = MockRepository.GenerateStub<IBocListSortingOrderProvider>();
      IBusinessObject firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject) ((TypeWithReference) BusinessObject).SecondValue;
      BocListRow[] rows = new[]
                          {
                              new BocListRow (sortingProvider, 0, firstObject),
                              new BocListRow (sortingProvider, 1, secondObject)
                          };
      int firstRow;
      List.Stub (list => list.GetRowsToDisplay(out firstRow)).Return (rows);
    }
  }
}
