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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

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

      IBusinessObject firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject) ((TypeWithReference) BusinessObject).SecondValue;
      var triplets = new[]
                     {
                         new BocListCustomColumnTuple (firstObject, 0, new WebControl(HtmlTextWriterTag.Div)),
                         new BocListCustomColumnTuple (secondObject, 1, new HtmlGenericControl("div"))
                     };
      var customColumns = new Dictionary<BocColumnDefinition, BocListCustomColumnTuple[]>
                          {
                              { Column, triplets }
                          };
      List.Stub (mock => mock.CustomColumns).Return (customColumns);
    }

    [Test]
    public void RenderCellWithInnerWebControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;
      List.OnPreRender();

      IBocColumnRenderer<BocCustomColumnDefinition> renderer = new BocCustomColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();
      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderCellWithInnerHtmlControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;
      List.OnPreRender ();

      IBocColumnRenderer<BocCustomColumnDefinition> renderer = new BocCustomColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument ();
      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderCellDirectly ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.NoControls;
      List.OnPreRender();

      IBocColumnRenderer<BocCustomColumnDefinition> renderer = new BocCustomColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();
      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);
    }
  }
}