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
  public class BocRowRendererTest : RendererTestBase
  {
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      InitializeBocList();
      List.FixedColumns.Add (new StubColumnDefinition());
    }

    [Test]
    public void RenderTitlesRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer (Html.Writer, List, new StubServiceLocator());
      renderer.RenderTitlesRow ();
     

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode tr = Html.GetAssertedChildElement (document.DocumentNode, "tr", 0, false);

      Html.AssertWhiteSpaceTextNode (tr, 0);

      HtmlNode th = Html.GetAssertedChildElement (tr, "th", 1, false);

      Html.AssertWhiteSpaceTextNode (tr, 2);
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      // List.Stub (mock => mock.Index).Return (RowIndex.InitialOrder);
      ((ObjectBinding.Web.UI.Controls.BocList) List).Index = RowIndex.InitialOrder;
      IBocRowRenderer renderer = new BocRowRenderer (Html.Writer, List, new StubServiceLocator ());
      renderer.RenderTitlesRow ();


      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode tr = Html.GetAssertedChildElement (document.DocumentNode, "tr", 0, false);

      Html.AssertWhiteSpaceTextNode (tr, 0);

      HtmlNode thIndex = Html.GetAssertedChildElement (tr, "th", 1, false);
      Html.AssertAttribute (thIndex, "class", "bocListTitleCell bocListTitleCellIndex");

      HtmlNode thColumn = Html.GetAssertedChildElement (tr, "th", 2, false);

      Html.AssertWhiteSpaceTextNode (tr, 3);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      // List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      ((ObjectBinding.Web.UI.Controls.BocList) List).Selection = RowSelection.Multiple;
      IBocRowRenderer renderer = new BocRowRenderer (Html.Writer, List, new StubServiceLocator ());
      renderer.RenderTitlesRow ();


      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode tr = Html.GetAssertedChildElement (document.DocumentNode, "tr", 0, false);

      Html.AssertWhiteSpaceTextNode (tr, 0);

      HtmlNode thSelector = Html.GetAssertedChildElement (tr, "th", 1, false);

      HtmlNode thColumn = Html.GetAssertedChildElement (tr, "th", 2, false);

      Html.AssertWhiteSpaceTextNode (tr, 3);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer (Html.Writer, List, new StubServiceLocator ());
      renderer.RenderDataRow (BusinessObject, 0, 0, 0);

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode tr = Html.GetAssertedChildElement (document.DocumentNode, "tr", 0, false);
      Html.AssertAttribute (tr, "class", "bocListDataRow");

      Html.AssertWhiteSpaceTextNode (tr, 0);

      HtmlNode th = Html.GetAssertedChildElement (tr, "td", 1, false);

      Html.AssertWhiteSpaceTextNode (tr, 2);
    }
  }
}