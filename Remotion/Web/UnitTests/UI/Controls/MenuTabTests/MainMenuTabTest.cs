// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Specialized;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Web.UI.Controls;
using Remotion.Development.Web.UnitTesting.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class MainMenuTabTest
  {
    private MockRepository _mocks;
    private NavigationCommand _mockNavigationCommand1;
    private NavigationCommand _mockNavigationCommand2;
    private SubMenuTab _mockSubMenuTab1;
    private SubMenuTab _mockSubMenuTab2;
    private TabbedMenu _mockTabbedMenu;
    private MainMenuTab _mainMenuTab;
    private WebTabStyle _style;

    private HtmlTextWriterSingleTagMock _htmlWriter;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();

      _mockTabbedMenu = _mocks.PartialMock<TabbedMenu> ();

      _mockNavigationCommand1 = _mocks.PartialMock<NavigationCommand> ();
      _mocks.Replay (_mockNavigationCommand1);

      _mockNavigationCommand2 = _mocks.PartialMock<NavigationCommand> ();
      _mocks.Replay (_mockNavigationCommand2);

      _mainMenuTab = new MainMenuTab ();
      _mainMenuTab.ItemID = "MainMenuTab";
      _mainMenuTab.Command.Type = CommandType.None;
      _mainMenuTab.OwnerControl = _mockTabbedMenu;

      _mockSubMenuTab1 = CreateSubMenuTab ("SubMenuTab1", _mockNavigationCommand1);
      _mainMenuTab.SubMenuTabs.Add (_mockSubMenuTab1);

      _mockSubMenuTab2 = CreateSubMenuTab ("SubMenuTab2", _mockNavigationCommand2);
      _mainMenuTab.SubMenuTabs.Add (_mockSubMenuTab2);

      _style = new WebTabStyle ();

      _htmlWriter = new HtmlTextWriterSingleTagMock ();

      _mocks.BackToRecord (_mockSubMenuTab1);
      _mocks.BackToRecord (_mockSubMenuTab2);
      _mocks.BackToRecord (_mockNavigationCommand1);
      _mocks.BackToRecord (_mockNavigationCommand2);
    }

    [Test]
    public void Render_WithoutMainMenuItemCommandAndFirstSubMenuItemActive ()
    {
      NameValueCollection additionalParameters = new NameValueCollection ();
      Expect.Call (_mockTabbedMenu.GetUrlParameters (_mockSubMenuTab1)).Return (additionalParameters);
      Expect.Call (_mockSubMenuTab1.EvaluateVisible ()).Return (true);
      Expect.Call (_mockSubMenuTab1.EvaluateEnabled ()).Return (true);
      Expect.Call (_mockSubMenuTab2.EvaluateVisible ()).Repeat.Never ();
      Expect.Call (_mockSubMenuTab2.EvaluateEnabled ()).Repeat.Never ();
      _mockNavigationCommand1.RenderBegin (_htmlWriter, string.Empty, new string[0], string.Empty, null, additionalParameters, false, _style);

      _mocks.ReplayAll ();

      _mainMenuTab.RenderBeginTagForCommand (_htmlWriter, true, _style);

      _mocks.VerifyAll ();
    }

    [Test]
    public void Render_WithoutMainMenuItemCommandAndFirstSubMenuItemNotVisible ()
    {
      NameValueCollection additionalParameters = new NameValueCollection ();
      Expect.Call (_mockTabbedMenu.GetUrlParameters (_mockSubMenuTab2)).Return (additionalParameters);
      Expect.Call (_mockSubMenuTab1.EvaluateVisible ()).Return (false);
      Expect.Call (_mockSubMenuTab1.EvaluateEnabled ()).Repeat.Never ();
      Expect.Call (_mockSubMenuTab2.EvaluateVisible ()).Return (true);
      Expect.Call (_mockSubMenuTab2.EvaluateEnabled ()).Return (true);
      _mockNavigationCommand2.RenderBegin (_htmlWriter, string.Empty, new string[0], string.Empty, null, additionalParameters, false, _style);
      _mocks.ReplayAll ();

      _mainMenuTab.RenderBeginTagForCommand (_htmlWriter, true, _style);

      _mocks.VerifyAll ();
    }

    [Test]
    public void Render_WithoutMainMenuItemCommandAndFirstSubMenuItemNotEnabled ()
    {
      NameValueCollection additionalParameters = new NameValueCollection ();
      Expect.Call (_mockTabbedMenu.GetUrlParameters (_mockSubMenuTab2)).Return (additionalParameters);
      Expect.Call (_mockSubMenuTab1.EvaluateVisible ()).Return (true);
      Expect.Call (_mockSubMenuTab1.EvaluateEnabled ()).Return (false);
      Expect.Call (_mockSubMenuTab2.EvaluateVisible ()).Return (true);
      Expect.Call (_mockSubMenuTab2.EvaluateEnabled ()).Return (true);
      _mockNavigationCommand2.RenderBegin (_htmlWriter, string.Empty, new string[0], string.Empty, null, additionalParameters, false, _style);
      _mocks.ReplayAll ();

      _mainMenuTab.RenderBeginTagForCommand (_htmlWriter, true, _style);

      _mocks.VerifyAll ();
    }

    private SubMenuTab CreateSubMenuTab (string itemID, NavigationCommand command)
    {
      SubMenuTab tab = _mocks.PartialMock<SubMenuTab> ();
      _mocks.Replay (tab);
      tab.ItemID = itemID;
      tab.Command = command;

      return tab;
    }
  }
}
