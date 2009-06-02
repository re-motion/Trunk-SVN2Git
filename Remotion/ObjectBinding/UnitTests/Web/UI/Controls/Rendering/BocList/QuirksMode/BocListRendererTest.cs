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
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  [TestFixture]
  public class BocListRendererTest : BocListRendererTestBase
  {
    private StubServiceLocator ServiceLocator { get; set; }

    [SetUp]
    public void SetUp ()
    {
      ServiceLocator = new StubServiceLocator();
      ServiceLocator.SetTableBlockRendererFactory (new StubRendererFactory());
      ServiceLocator.SetMenuBlockRendererFactory (new StubRendererFactory());
      ServiceLocator.SetNavigationBlockRendererFactory (new StubRendererFactory());

      Initialize();

      List.Stub (mock => mock.HasNavigator).Return (true);
    }

    [Test]
    public void RenderWithMenuBlock ()
    {
      Unit menuBlockWidth = new Unit (123, UnitType.Pixel);
      Unit menuBlockOffset = new Unit (12, UnitType.Pixel);

      List.Stub (mock => mock.HasMenuBlock).Return (true);
      List.Stub (mock => mock.MenuBlockWidth).Return (menuBlockWidth);
      List.Stub (mock => mock.MenuBlockOffset).Return (menuBlockOffset);

      XmlNode colgroup;
      RenderAndAssertTable (out colgroup);

      var colMenu = Html.GetAssertedChildElement (colgroup, "col", 1, true);
      Html.AssertStyleAttribute (colMenu, "width", menuBlockWidth.ToString());
      Html.AssertStyleAttribute (colMenu, "padding-left", menuBlockOffset.ToString());
    }

    [Test]
    public void RenderWithoutMenuBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (false);

      XmlNode colgroup;
      RenderAndAssertTable (out colgroup);

      Html.AssertChildElementCount (colgroup, 1);
    }

    private void RenderAndAssertTable (out XmlNode colgroup)
    {
      var renderer = new BocListRenderer (HttpContext, Html.Writer, List, ServiceLocator);
      renderer.Render();

      var document = Html.GetResultDocument();

      var table = Html.GetAssertedChildElement (document, "table", 0, false);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertStyleAttribute (table, "width", "100%");

      colgroup = Html.GetAssertedChildElement (table, "colgroup", 0, true);

      var colTableAndNavigation = Html.GetAssertedChildElement (colgroup, "col", 0, true);

      var tr = Html.GetAssertedChildElement (table, "tr", 1, true);

      var td = Html.GetAssertedChildElement (tr, "td", 0, true);
      Html.AssertStyleAttribute (td, "vertical-align", "top");

      Html.GetAssertedChildElement (td, "div", 0, false);
    }
  }
}