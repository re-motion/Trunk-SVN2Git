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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocTableBlockRendererTest : RendererTestBase
  {
    private StubServiceLocator ServiceLocator { get; set; }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      InitializeMockList ();
      ServiceLocator = new StubServiceLocator();
      ServiceLocator.SetRowRendererFactory (new StubRowRendererFactory());
    }

    [Test]
    [Ignore("TODO: replace dependencies on Control with IControl so that mocked BocList can have a BocColumnDefinitionCollection")]
    public void Render ()
    {
      List.Stub (mock => mock.IsDesignMode).Return (false);
      List.FixedColumns.Add (new StubColumnDefinition());
      List.FixedColumns.Add (new StubColumnDefinition ());
      List.FixedColumns.Add (new StubColumnDefinition ());

      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer (Html.Writer, List, ServiceLocator);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);

      HtmlNode table = Html.GetAssertedChildElement (div, "table", 0, true);

      HtmlNode colgroup = Html.GetAssertedChildElement (table, "colgroup", 0, true);

      HtmlNode col1 = Html.GetAssertedChildElement (colgroup, "col", 0, true);
      HtmlNode col2 = Html.GetAssertedChildElement (colgroup, "col", 1, true);
      HtmlNode col3 = Html.GetAssertedChildElement (colgroup, "col", 2, true);

      HtmlNode thead = Html.GetAssertedChildElement (table, "thead", 1, true);

      HtmlNode trTitle = Html.GetAssertedChildElement (thead, "tr", 0, true);
      Html.AssertAttribute (trTitle, "class", "titleStub");

      HtmlNode tbody = Html.GetAssertedChildElement (table, "tbody", 2, true);

      HtmlNode trData1 = Html.GetAssertedChildElement (tbody, "tr", 0, true);
      Html.AssertAttribute (trData1, "class", "dataStub");

      HtmlNode trData2 = Html.GetAssertedChildElement (tbody, "tr", 1, true);
      Html.AssertAttribute (trData2, "class", "dataStub");
    }
  }
}