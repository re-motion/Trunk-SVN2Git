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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  [TestFixture]
  public class BocCompoundColumnRendererTest : ColumnRendererTestBase<BocCompoundColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCompoundColumnDefinition();
      Column.ColumnTitle = "TestColumn1";
      Column.ColumnTitle = "FirstColumn";
      Column.Command = null;
      Column.EnforceWidth = false;
      Column.FormatString = "{0}";

      base.SetUp();

      Column.PropertyPathBindings.Add (new PropertyPathBinding ("DisplayName"));
    }

    [Test]
    public void RenderEmptyCell ()
    {
      Column.FormatString = string.Empty;

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, false, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      Html.AssertTextNode (span, HtmlHelper.WhiteSpace, 0, false);
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, false, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      Html.AssertTextNode (span, "referencedObject1", 0, false);
    }

    [Test]
    public void RenderEnforcedWidthCell ()
    {
      Column.EnforceWidth = true;
      Column.Width = new Unit (40, UnitType.Pixel);

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, false, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode cropSpan = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (cropSpan, "title", "referencedObject1");
      Html.AssertStyleAttribute (cropSpan, "width", "40px");
      Html.AssertStyleAttribute (cropSpan, "display", "block");
      Html.AssertStyleAttribute (cropSpan, "overflow", "hidden");
      Html.AssertStyleAttribute (cropSpan, "white-space", "nowrap");

      HtmlNode span = Html.GetAssertedChildElement (cropSpan, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      Html.AssertTextNode (span, "referencedObject1", 0, false);
    }
  }
}