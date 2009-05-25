﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocSimpleColumnRendererTest : ColumnRendererTestBase<BocSimpleColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.Command = null;
      Column.IsDynamic = false;
      Column.IsReadOnly = false;
      Column.ColumnTitle = "FirstColumn";
      Column.PropertyPathIdentifier = "DisplayName";
      Column.FormatString = "{0}";

      base.SetUp();
    }

    [Test]
    public void RenderBasicCell ()
    {
      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, false, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      Html.AssertTextNode (span, "referencedObject1", 0, false);
    }

    [Test]
    public void RenderCommandCell ()
    {
      Column.Command = new BocListItemCommand (CommandType.Href);

      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, false, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode a = Html.GetAssertedChildElement (td, "a", 0, false);

      Html.AssertTextNode (a, "referencedObject1", 0, false);
      Html.AssertAttribute (a, "href", "");
      Html.AssertAttribute (a, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderIconCell ()
    {
      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column);

      renderer.RenderDataCell (0, true, EventArgs);
      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      Html.AssertIcon (span, EventArgs.BusinessObject, null);

      Html.AssertTextNode (span, HtmlHelper.WhiteSpace + BusinessObject.GetPropertyString ("FirstValue"), 1, false);
    }

    [Test]
    public void RenderEditModeControl ()
    {
      var firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;

      EditableRow editableRow = new EditableRow (List);
      editableRow.ControlFactory = new EditableRowControlFactory();
      editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
      editableRow.CreateControls (firstObject, List.GetColumns());
      ((BusinessObjectBoundEditableWebControl)editableRow.GetEditControl (0)).ReadOnly = false;
      List.EditModeController.Stub (mock => mock.GetEditableRow (0)).Return (editableRow);

      List.Stub (mock => mock.Validators).Return (new ArrayList());

      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);
      
      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      HtmlNode span = Html.GetAssertedChildElement (td, "span", 0, false);
      Html.AssertAttribute (span, "class", List.CssClassContent);

      HtmlNode clickSpan = Html.GetAssertedChildElement (span, "span", 0, false);
      Html.AssertAttribute (clickSpan, "onclick", "BocList_OnCommandClick();");

      HtmlNode controlWrapperSpan = Html.GetAssertedChildElement (clickSpan, "span", 0, false);
      Html.AssertStyleAttribute (controlWrapperSpan, "width", "100%");
      Html.AssertStyleAttribute (controlWrapperSpan, "display", "block");
    }
  }
}