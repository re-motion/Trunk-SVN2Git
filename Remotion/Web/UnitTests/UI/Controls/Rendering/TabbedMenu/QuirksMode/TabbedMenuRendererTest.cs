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
using System.Drawing;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.TabbedMenu.QuirksMode
{
  [TestFixture]
  public class TabbedMenuRendererTest : RendererTestBase
  {
    private TabbedMenuMock _control;
    private ControlInvoker _invoker;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _control = new TabbedMenuMock();
      _invoker = new ControlInvoker (_control);
      Page page = new Page();
      page.Controls.Add (_control);
    }

    [Test]
    public void RenderEmptyMenu ()
    {
      AssertControl (false, false, false);
    }

    [Test]
    public void RenderEmptyMenuInDesignMode ()
    {
      _control.SetDesignMode (true);
      AssertControl (true, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusText ()
    {
      _control.StatusText = "Status";
      AssertControl (false, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusTextInDesignMode ()
    {
      _control.SetDesignMode (true);
      _control.StatusText = "Status";
      AssertControl (true, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithCssClass ()
    {
      _control.CssClass = "CustomCssClass";
      AssertControl (false, false, true);
    }

    [Test]
    public void RenderEmptyMenuWithBackgroundColor ()
    {
      _control.SubMenuBackgroundColor = Color.Yellow;
      AssertControl (false, false, false);
    }

    private void AssertControl (bool isDesignMode, bool hasStatusText, bool hasCssClass)
    {
      _invoker.PreRenderRecursive();
      _control.RenderControl (Html.Writer);

      var document = Html.GetResultDocument();
      var table = document.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("class", hasCssClass ? "CustomCssClass" : "tabbedMenu");
      if (isDesignMode)
        table.AssertStyleAttribute ("width", "100%");
      table.AssertChildElementCount (2);

      var trMainMenu = table.GetAssertedChildElement ("tr", 0);
      trMainMenu.AssertChildElementCount (1);

      var tdMainMenu = trMainMenu.GetAssertedChildElement ("td", 0);
      tdMainMenu.AssertAttributeValueEquals ("colspan", "2");
      tdMainMenu.AssertAttributeValueEquals ("class", "tabbedMainMenuCell");
      tdMainMenu.AssertChildElementCount (0);

      var trSubMenu = table.GetAssertedChildElement ("tr", 1);
      trSubMenu.AssertChildElementCount (2);

      var tdSubMenu = trSubMenu.GetAssertedChildElement ("td", 0);
      tdSubMenu.AssertAttributeValueEquals ("class", "tabbedSubMenuCell");
      if (!_control.SubMenuBackgroundColor.IsEmpty)
        tdSubMenu.AssertStyleAttribute ("background-color", ColorTranslator.ToHtml (Color.Yellow));
      tdSubMenu.AssertChildElementCount (0);

      var tdMenuStatus = trSubMenu.GetAssertedChildElement ("td", 1);
      tdMenuStatus.AssertAttributeValueEquals ("class", "tabbedMenuStatusCell");
      tdMenuStatus.AssertChildElementCount (0);
      tdMenuStatus.AssertTextNode (hasStatusText ? "Status" : "&nbsp;", 0);
    }
  }
}