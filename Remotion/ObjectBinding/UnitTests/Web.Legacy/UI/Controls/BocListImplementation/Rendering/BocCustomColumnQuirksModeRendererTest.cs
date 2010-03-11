// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCustomColumnQuirksModeRendererTest : ColumnRendererTestBase<BocCustomColumnDefinition>
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
                         new BocListCustomColumnTuple (firstObject, 0, new WebControl (HtmlTextWriterTag.Div)),
                         new BocListCustomColumnTuple (secondObject, 1, new HtmlGenericControl ("div"))
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

      IBocColumnRenderer renderer = new BocCustomColumnQuirksModeRenderer (
          HttpContext, List, Column, CssClassContainer.Instance);
      renderer.RenderDataCell (Html.Writer, 0, false, EventArgs);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderCellWithInnerHtmlControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;
      List.OnPreRender();

      IBocColumnRenderer renderer = new BocCustomColumnQuirksModeRenderer (
          HttpContext, List, Column, CssClassContainer.Instance);
      renderer.RenderDataCell (Html.Writer, 0, false, EventArgs);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "onclick", "BocList_OnCommandClick();");
    }

    [Test]
    public void RenderCellDirectly ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.NoControls;
      List.OnPreRender();

      IBocColumnRenderer renderer = new BocCustomColumnQuirksModeRenderer (
          HttpContext, List, Column, CssClassContainer.Instance);
      renderer.RenderDataCell (Html.Writer, 0, false, EventArgs);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", CssClassContainer.Instance.DataCellOdd);
    }
  }
}