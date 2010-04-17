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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
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
      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderTitlesRow (Html.Writer);


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderTitlesRow (Html.Writer);


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      var thIndex = Html.GetAssertedChildElement (tr, "th", 0);
      Html.AssertAttribute (thIndex, "class", CssClassContainer.Instance.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (thIndex, "class", CssClassContainer.Instance.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderTitlesRow (Html.Writer);


      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "th", 0);

      Html.GetAssertedChildElement (tr, "th", 1);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderDataRow (Html.Writer, BusinessObject, 0, 0, 0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", CssClassContainer.Instance.DataRow);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderDataRowSelected ()
    {
      List.SelectorControlCheckedState.Add (0);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderDataRow (Html.Writer, BusinessObject, 0, 0, 0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);
      Html.AssertAttribute (tr, "class", CssClassContainer.Instance.DataRowSelected);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRow ()
    {
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, new StubServiceLocator());
      renderer.RenderEmptyListDataRow (Html.Writer);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement (document, "tr", 0);

      Html.GetAssertedChildElement (tr, "td", 0);
    }

    [Test]
    public void RendererCachesColumnRenderers()
    {
      List = MockRepository.GenerateMock<IBocList>();

      List.Stub (list => list.FixedColumns).Return (new BocColumnDefinitionCollection (List));
      var simpleColumnDefinition = new BocSimpleColumnDefinition();
      List.FixedColumns.Add (simpleColumnDefinition);
      List.Stub (mock => mock.GetColumns()).Return (List.FixedColumns.ToArray());
      List.Stub (mock => mock.IsIndexEnabled).Return (true);
      List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);
      List.Stub (mock => mock.IsSelectionEnabled).Return (true);
      List.Stub (list => list.SelectorControlCheckedState).Return (new List<int>());
      List.Stub (list => list.GetResourceManager()).Return (MultiLingualResources.GetResourceManager (typeof (BocList.ResourceIdentifier)));

      var mockRepository = new MockRepository();

      var serviceLocatorMock = MockRepository.GenerateMock<IServiceLocator>();

      var indexColumnRendererFactoryMock = MockRepository.GenerateMock<IBocIndexColumnRendererFactory>();
      indexColumnRendererFactoryMock.Expect (mock => mock.CreateRenderer (HttpContext, List))
          .Return (MockRepository.GenerateStub<IBocIndexColumnRenderer>());
      serviceLocatorMock.Expect (mock => mock.GetInstance<IBocIndexColumnRendererFactory>()).Return (indexColumnRendererFactoryMock);

      var seclectorColumnRendererFactoryMock = MockRepository.GenerateMock<IBocSelectorColumnRendererFactory>();
      seclectorColumnRendererFactoryMock.Expect (mock => mock.CreateRenderer (HttpContext, List))
          .Return (MockRepository.GenerateStub<IBocSelectorColumnRenderer>());
      serviceLocatorMock.Expect (mock => mock.GetInstance<IBocSelectorColumnRendererFactory>()).Return (seclectorColumnRendererFactoryMock);

      var simpleColumnRendererFactoryMock = MockRepository.GenerateMock<IBocColumnRendererFactory<BocSimpleColumnDefinition>>();
      simpleColumnRendererFactoryMock.Expect (mock => mock.CreateRenderer (HttpContext, List, simpleColumnDefinition, serviceLocatorMock))
          .Return (MockRepository.GenerateStub<IBocColumnRenderer>());
      serviceLocatorMock.Expect (mock => mock.GetInstance<IBocColumnRendererFactory<BocSimpleColumnDefinition>>()).Return (
          simpleColumnRendererFactoryMock);

      mockRepository.ReplayAll();

      IBocRowRenderer renderer = new BocRowRenderer (HttpContext, List, CssClassContainer.Instance, serviceLocatorMock);
      renderer.RenderTitlesRow (Html.Writer);
      renderer.RenderDataRow (Html.Writer, BusinessObject, 0, 0, 0);

      mockRepository.VerifyAll();
    }
  }
}