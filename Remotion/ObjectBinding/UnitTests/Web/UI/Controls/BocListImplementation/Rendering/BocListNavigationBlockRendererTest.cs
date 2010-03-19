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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListNavigationBlockRendererTest : BocListRendererTestBase
  {
    private const string c_pageInfo = "current page: {0} (of {1})";
    private const string c_tripleBlank = HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.HasNavigator).Return (true);
      List.Stub (mock => mock.PageInfo).Return (c_pageInfo);
    }

    [Test]
    public void RenderOnlyPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (0);
      List.Stub (mock => mock.PageCount).Return (1);

      var renderer = new BocListNavigationBlockRenderer (
          HttpContext, List, new ResourceUrlFactory (ResourceTheme.ClassicBlue), CssClassContainer.Instance);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", CssClassContainer.Instance.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 1) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "img", 1);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "img", 3);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "img", 5);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "img", 7);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderFirstPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (0);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (
          HttpContext, List, new ResourceUrlFactory (ResourceTheme.ClassicBlue), CssClassContainer.Instance);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", CssClassContainer.Instance.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 2) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "img", 1);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "img", 3);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertActiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertActiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderLastPage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (1);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (
          HttpContext, List, new ResourceUrlFactory (ResourceTheme.ClassicBlue), CssClassContainer.Instance);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", CssClassContainer.Instance.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 2, 2) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertActiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertActiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "img", 5);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "img", 7);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderMiddlePage ()
    {
      List.Stub (mock => mock.CurrentPage).Return (1);
      List.Stub (mock => mock.PageCount).Return (3);

      var renderer = new BocListNavigationBlockRenderer (
          HttpContext, List, new ResourceUrlFactory (ResourceTheme.ClassicBlue), CssClassContainer.Instance);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", CssClassContainer.Instance.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 2, 3) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertActiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertActiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertActiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertActiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    private void AssertActiveIcon (XmlNode link, string command)
    {
      Html.AssertAttribute (link, "onclick", "postBackEventReference");
      Html.AssertAttribute (link, "href", "#");

      var icon = Html.GetAssertedChildElement (link, "img", 0);
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void AssertInactiveIcon (XmlNode icon, string command)
    {
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}Inactive.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}