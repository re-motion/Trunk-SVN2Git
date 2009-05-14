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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocCustomColumnRendererTest : ColumnRendererTestBase<BocCustomColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCustomColumnDefinition();
      Column.CustomCell = new StubCustomCellDefinition();

      base.SetUp();

      List.IsDesignModeOverrideValue = false;
    }

    [Test]
    public void RenderCellWithInnerControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;
      List.OnPreRender();

      IBocColumnRenderer<BocCustomColumnDefinition> renderer = new BocCustomColumnRenderer (Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();
      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "onclick", "BocList_OnCommandClick();");

      HtmlNode div = Html.GetAssertedChildElement (span, "div", 0, false);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "vertical-align", "middle");
    }

    [Test]
    public void RenderCellDirectly ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.NoControls;
      List.OnPreRender();

      IBocColumnRenderer<BocCustomColumnDefinition> renderer = new BocCustomColumnRenderer (Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();
      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");
    }
  }
}