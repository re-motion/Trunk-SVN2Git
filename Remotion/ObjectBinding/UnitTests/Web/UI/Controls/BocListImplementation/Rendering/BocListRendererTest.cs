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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListRendererTest : BocListRendererTestBase
  {
    private static readonly Unit s_menuBlockWidth = new Unit (123, UnitType.Pixel);
    private static readonly Unit s_menuBlockOffset = new Unit (12, UnitType.Pixel);

    [SetUp]
    public void SetUp ()
    {
      Initialize();
    }

    [Test]
    public void RenderOnlyTableBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (false);

      var renderer = new BocListRenderer (
          HttpContext, List, CssClassContainer.Instance, new StubRenderer ("table"), new StubRenderer ("navigation"), new StubRenderer ("menu"));
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument ();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", CssClassContainer.Instance.TableBlockWithoutMenuBlock);
      Html.AssertChildElementCount (tableBlock, 1);
      Html.GetAssertedChildElement (tableBlock, "table", 0);
    }

    [Test]
    public void RenderWithMenuBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (true);
      List.Stub (mock => mock.MenuBlockWidth).Return (s_menuBlockWidth);
      List.Stub (mock => mock.MenuBlockOffset).Return (s_menuBlockOffset);

      var renderer = new BocListRenderer (
          HttpContext, List, CssClassContainer.Instance, new StubRenderer ("table"), new StubRenderer ("navigation"), new StubRenderer ("menu"));
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", CssClassContainer.Instance.TableBlockWithMenuBlock);

      Html.AssertStyleAttribute (tableBlock, "right", s_menuBlockWidth.ToString());

      Html.GetAssertedChildElement (tableBlock, "table", 0);

      var menuBlock = Html.GetAssertedChildElement (div, "div", 1);
      Html.AssertAttribute (menuBlock, "class", CssClassContainer.Instance.MenuBlock);
      Html.AssertStyleAttribute (menuBlock, "width", s_menuBlockWidth.ToString());
      Html.AssertStyleAttribute (menuBlock, "padding-left", s_menuBlockOffset.ToString());
      Html.GetAssertedChildElement (menuBlock, "menu", 0);
    }

    [Test]
    public void RenderWithMenuBlockWithoutWidth ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (true);

      var renderer = new BocListRenderer (
          HttpContext, List, CssClassContainer.Instance, new StubRenderer ("table"), new StubRenderer ("navigation"), new StubRenderer ("menu"));
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", CssClassContainer.Instance.TableBlockWithMenuBlock);

      Html.GetAssertedChildElement (tableBlock, "table", 0);

      var menuBlock = Html.GetAssertedChildElement (div, "div", 1);
      Html.AssertAttribute (menuBlock, "class", CssClassContainer.Instance.MenuBlock);
      Html.GetAssertedChildElement (menuBlock, "menu", 0);
    }

    [Test]
    public void RenderWithNavigationBlock ()
    {
      List.Stub (mock => mock.HasNavigator).Return (true);

      var renderer = new BocListRenderer (
          HttpContext, List, CssClassContainer.Instance, new StubRenderer ("table"), new StubRenderer ("navigation"), new StubRenderer ("menu"));
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument ();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", CssClassContainer.Instance.TableBlockWithoutMenuBlock);
      Html.AssertChildElementCount (tableBlock, 2);
      Html.GetAssertedChildElement (tableBlock, "table", 0);
      Html.GetAssertedChildElement (tableBlock, "navigation", 1);
    }
  }
}