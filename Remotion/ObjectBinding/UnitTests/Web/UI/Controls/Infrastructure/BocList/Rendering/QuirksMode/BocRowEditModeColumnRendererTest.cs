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

      base.SetUp ();

      EventArgs.IsEditableRow = true;

      ((BocListMock) List).EnableClientScript = true;
      ((BocListMock) List).IsDesignModeOverrideValue = false;
      ((BocListMock) List).ReadOnly = false;
      ((BocListMock) List).DataSource.Mode = DataSourceMode.Edit;
      List.OnPreRender();
    }

    [Test]
    public void RenderEditable ()
    {
      IBocColumnRenderer<BocRowEditModeColumnDefinition> renderer = new BocRowEditModeColumnRenderer (Html.Writer, List, Column);
      EventArgs.IsEditableRow = true;
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode a = Html.GetAssertedChildElement (td, "a", 0, false);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "__doPostBack('ctl00','RowEditMode=0,Edit');BocList_OnCommandClick();");
      Html.AssertTextNode (a, "Bearbeiten", 0, false);
    }

    [Test]
    public void RenderEditing ()
    {
      List.SwitchRowIntoEditMode (0);

      IBocColumnRenderer<BocRowEditModeColumnDefinition> renderer = new BocRowEditModeColumnRenderer (Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode save = Html.GetAssertedChildElement (td, "a", 0, false);
      Html.AssertAttribute (save, "href", "#");
      Html.AssertAttribute (save, "onclick", "__doPostBack('ctl00','RowEditMode=0,Save');BocList_OnCommandClick();");
      Html.AssertTextNode (save, "Speichern", 0, false);

      Html.AssertWhiteSpaceTextNode (td, 1);

      HtmlNode cancel = Html.GetAssertedChildElement (td, "a", 2, false);
      Html.AssertAttribute (cancel, "href", "#");
      Html.AssertAttribute (cancel, "onclick", "__doPostBack('ctl00','RowEditMode=0,Cancel');BocList_OnCommandClick();");
      Html.AssertTextNode (cancel, "Abbrechen", 0, false);
    }
  }
}