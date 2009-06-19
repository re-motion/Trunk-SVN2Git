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
using System.Collections;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
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
      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column, CssClassContainer.Instance);

      renderer.RenderDataCell (0, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", CssClassContainer.Instance.Content);

      Html.AssertTextNode (span, "referencedObject1", 0);
    }

    [Test]
    public void RenderCommandCell ()
    {
      Column.Command = new BocListItemCommand (CommandType.Href);

      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column, CssClassContainer.Instance);

      renderer.RenderDataCell (0, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var a = Html.GetAssertedChildElement (td, "a", 0);

      Html.AssertTextNode (a, "referencedObject1", 0);
      Html.AssertAttribute (a, "href", "");
      Html.AssertAttribute (a, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderIconCell ()
    {
      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column, CssClassContainer.Instance);

      renderer.RenderDataCell (0, true, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", CssClassContainer.Instance.Content);

      Html.AssertIcon (span, EventArgs.BusinessObject, null);

      Html.AssertTextNode (span, HtmlHelper.WhiteSpace + BusinessObject.GetPropertyString ("FirstValue"), 1);
    }

    [Test]
    public void RenderEditModeControl ()
    {
      var firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;

      IEditableRow editableRow = MockRepository.GenerateMock<IEditableRow>();
      editableRow.Stub (mock => mock.HasEditControl (0)).IgnoreArguments().Return (true);
      editableRow.Stub (mock => mock.GetEditControl (0)).IgnoreArguments().Return (MockRepository.GenerateStub<IBocTextValue>());
      editableRow.Expect (
          mock => mock.RenderSimpleColumnCellEditModeControl (
                      Html.Writer,
                      Column,
                      firstObject,
                      0,
                      null,
                      List.EditModeController.ShowEditModeValidationMarkers,
                      List.EditModeController.DisableEditModeValidationMessages));

      List.EditModeController.Stub (mock => mock.GetEditableRow (0)).Return (editableRow);

      List.Stub (mock => mock.Validators).Return (new ArrayList());

      var renderer = new BocSimpleColumnRenderer (HttpContext, Html.Writer, List, Column, CssClassContainer.Instance);
      renderer.RenderDataCell (0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", CssClassContainer.Instance.Content);

      var clickSpan = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (clickSpan, "onclick", "BocList_OnCommandClick();");

      editableRow.AssertWasCalled (
          mock => mock.RenderSimpleColumnCellEditModeControl (
                      Html.Writer,
                      Column,
                      firstObject,
                      0,
                      null,
                      List.EditModeController.ShowEditModeValidationMarkers,
                      List.EditModeController.DisableEditModeValidationMessages));
    }
  }
}