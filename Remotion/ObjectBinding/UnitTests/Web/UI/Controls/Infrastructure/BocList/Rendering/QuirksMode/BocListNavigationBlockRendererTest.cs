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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocListNavigationBlockRendererTest : RendererTestBase
  {
    private const string c_pageInfo = "current page: {0} (of {1})";
    private const string c_tripleBlank = HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace;

    [SetUp]
    public void SetUp ()
    {
      Initialize ();

      List.Stub (mock => mock.HasNavigator).Return (true);
      List.Stub (mock => mock.PageInfo).Return (c_pageInfo);
      List.Stub (mock => mock.CssClassNavigator).Return ("cssClassNavigator");
    }

    [Test]
    public void RenderOnlyPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (0);
      List.Stub (mock => mock.PageCount).Return (1);

      var renderer = new BocListNavigationBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render();

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertAttribute (div, "class", List.CssClassNavigator);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "position", "relative");

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 1) + c_tripleBlank, 0, false);

      HtmlNode firstIcon = Html.GetAssertedChildElement (div, "img", 1, false);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2, false);

      HtmlNode previousIcon = Html.GetAssertedChildElement (div, "img", 3, false);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4, false);

      HtmlNode nextIcon = Html.GetAssertedChildElement (div, "img", 5, false);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6, false);

      HtmlNode lastIcon = Html.GetAssertedChildElement (div, "img", 7, false);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8, false);
    }

    [Test]
    public void RenderFirstPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (0);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render ();

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertAttribute (div, "class", List.CssClassNavigator);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "position", "relative");

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 2) + c_tripleBlank, 0, false);

      HtmlNode firstIcon = Html.GetAssertedChildElement (div, "img", 1, false);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2, false);

      HtmlNode previousIcon = Html.GetAssertedChildElement (div, "img", 3, false);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4, false);

      HtmlNode nextIcon = Html.GetAssertedChildElement (div, "a", 5, false);
      AssertActiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6, false);

      HtmlNode lastIcon = Html.GetAssertedChildElement (div, "a", 7, false);
      AssertActiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8, false);
    }

    [Test]
    public void RenderLastPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (1);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render ();

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertAttribute (div, "class", List.CssClassNavigator);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "position", "relative");

      Html.AssertTextNode (div, string.Format (c_pageInfo, 2, 2) + c_tripleBlank, 0, false);

      HtmlNode firstIcon = Html.GetAssertedChildElement (div, "a", 1, false);
      AssertActiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2, false);

      HtmlNode previousIcon = Html.GetAssertedChildElement (div, "a", 3, false);
      AssertActiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4, false);

      HtmlNode nextIcon = Html.GetAssertedChildElement (div, "img", 5, false);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6, false);

      HtmlNode lastIcon = Html.GetAssertedChildElement (div, "img", 7, false);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8, false);
    }

    [Test]
    public void RenderMiddlePage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (1);
      List.Stub (mock => mock.PageCount).Return (3);

      var renderer = new BocListNavigationBlockRenderer (HttpContext, Html.Writer, List);
      renderer.Render ();

      HtmlDocument document = Html.GetResultDocument ();

      HtmlNode div = Html.GetAssertedChildElement (document.DocumentNode, "div", 0, false);
      Html.AssertAttribute (div, "class", List.CssClassNavigator);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "position", "relative");

      Html.AssertTextNode (div, string.Format (c_pageInfo, 2, 3) + c_tripleBlank, 0, false);

      HtmlNode firstIcon = Html.GetAssertedChildElement (div, "a", 1, false);
      AssertActiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2, false);

      HtmlNode previousIcon = Html.GetAssertedChildElement (div, "a", 3, false);
      AssertActiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4, false);

      HtmlNode nextIcon = Html.GetAssertedChildElement (div, "a", 5, false);
      AssertActiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6, false);

      HtmlNode lastIcon = Html.GetAssertedChildElement (div, "a", 7, false);
      AssertActiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8, false);
    }

    private void AssertActiveIcon (HtmlNode link, string command)
    {
      Html.AssertAttribute (link, "onclick", "postBackEventReference; return false;");
      Html.AssertAttribute (link, "href", "#");

      HtmlNode icon = Html.GetAssertedChildElement (link, "img", 0, false);
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (icon, "vertical-align", "middle");
      Html.AssertStyleAttribute (icon, "border-style", "none");
    }

    private void AssertInactiveIcon (HtmlNode icon, string command)
    {
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}Inactive.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
      Html.AssertStyleAttribute (icon, "vertical-align", "middle");
      Html.AssertStyleAttribute (icon, "border-style", "none");
    }
  }
}