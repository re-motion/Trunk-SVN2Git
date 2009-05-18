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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocIndexColumnRendererTest : RendererTestBase
  {
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      InitializeBocList();
      ((BocListMock) List).Index = RowIndex.InitialOrder;
    }

    [Test]
    public void RenderIndexTitleCell ()
    {
      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer (Html.Writer, List);
      renderer.RenderTitleCell();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode th = Html.GetAssertedChildElement (document.DocumentNode, "th", 0, false);
      Html.AssertAttribute (th, "class", "bocListTitleCell bocListTitleCellIndex");

      HtmlNode span = Html.GetAssertedChildElement (th, "span", 0, false);
      Html.AssertTextNode (span, "No.", 0, false);
    }

    [Test]
    public void RenderIndexDataCell ()
    {
      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer (Html.Writer, List);
      renderer.RenderDataCell (0, "selectorID", 0, "bocListTableCell");

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListTableCell bocListDataCellIndex");

      HtmlNode label = Html.GetAssertedChildElement (td, "label", 0, false);
      Html.AssertAttribute (label, "class", "bocListContent");
      Html.AssertAttribute (label, "for", "selectorID");

      Html.AssertTextNode (label, "1", 0, false);
    }
  }
}