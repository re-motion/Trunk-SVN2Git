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
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocListMenuBlockRendererTest : RendererTestBase
  {
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      InitializeMockList ();
    }

    [Test]
    //[Ignore("TODO: make AvailableViewsList (DropDownList) mockable")]
    public void RenderWithAvailableViews ()
    {
      BocColumnDefinition[] columns = new BocColumnDefinition[3];
      for (int i = 0; i < columns.Length; i++)
        columns[i] = new StubColumnDefinition ();

      DropDownList dropDownList = MockRepository.GenerateMock<DropDownList> ();
      List.Stub (mock => mock.AvailableViewsList).Return (dropDownList);
      List.Stub (mock => mock.HasAvailableViewsList).Return (true);
      List.Stub (mock => mock.AvailableViewsListTitle).Return ("Views List Title");
      List.Stub (mock => mock.CssClassAvailableViewsListLabel).Return ("CssClass");

      dropDownList.Stub (mock => mock.RenderControl (Html.Writer)).Do (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).WriteLine ("mocked dropdown list"));
      //dropDownList.Expect (mock => mock.RenderControl (Html.Writer)).Do (
      //    invocation => ((HtmlTextWriter) invocation.Arguments[0]).WriteLine ("mocked dropdown list"));

      var renderer = new BocListMenuBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render ();

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "margin-bottom", "5pt");

      HtmlNode span = Html.GetAssertedChildElement (div, "span", 0, true);
      Html.AssertAttribute (span, "class", "CssClass");
      Html.AssertTextNode (span, "Views List Title", 0, false);

      Html.AssertTextNode (div, HtmlHelper.WhiteSpace + "mocked dropdown list", 1, true);
    }
  }
}