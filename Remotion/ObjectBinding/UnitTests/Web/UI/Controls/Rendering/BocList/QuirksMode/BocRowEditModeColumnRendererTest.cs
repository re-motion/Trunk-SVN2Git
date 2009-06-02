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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  [TestFixture]
  public class BocRowEditModeColumnRendererTest : ColumnRendererTestBase<BocRowEditModeColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocRowEditModeColumnDefinition();
      Column.EditText = "Bearbeiten";
      Column.SaveText = "Speichern";
      Column.CancelText = "Abbrechen";
      Column.Show = BocRowEditColumnDefinitionShow.Always;

      base.SetUp();

      EventArgs.IsEditableRow = true;

      List.Stub (mock => mock.EnableClientScript).Return (true);
      List.Stub (mock => mock.IsDesignMode).Return (false);
      List.Stub (mock => mock.IsReadOnly).Return (false);
      List.DataSource.Mode = DataSourceMode.Edit;

      List.OnPreRender();
    }

    [Test]
    public void RenderEditable ()
    {
      IBocColumnRenderer<BocRowEditModeColumnDefinition> renderer = new BocRowEditModeColumnRenderer (HttpContext, Html.Writer, List, Column);
      EventArgs.IsEditableRow = true;
      renderer.RenderDataCell (0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (a, "Bearbeiten", 0);
    }

    [Test]
    public void RenderEditing ()
    {
      List.EditModeController.Stub (mock => mock.EditableRowIndex).Return (0);

      IBocColumnRenderer<BocRowEditModeColumnDefinition> renderer = new BocRowEditModeColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", List.CssClassDataCellOdd);

      var save = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (save, "href", "#");
      Html.AssertAttribute (save, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (save, "Speichern", 0);

      var cancel = Html.GetAssertedChildElement (td, "a", 1);
      Html.AssertAttribute (cancel, "href", "#");
      Html.AssertAttribute (cancel, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (cancel, "Abbrechen", 0);
    }
  }
}