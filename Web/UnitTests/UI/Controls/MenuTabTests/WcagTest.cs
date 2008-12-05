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
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Remotion.Development.Web.UnitTesting.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class WcagTest : BaseTest
  {
    [Test]
    public void IsMainMenuTabSetToEventInvisibleWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA ();
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.Event;
      Assert.IsFalse (mainMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsMainMenuTabSetToEventVisibleWithoutWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined ();
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.Event;
      Assert.IsTrue (mainMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsSubMenuTabSetToEventInvisibleWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA ();
      SubMenuTab subMenuTab = new SubMenuTab ();
      subMenuTab.Command.Type = CommandType.Event;
      Assert.IsFalse (subMenuTab.EvaluateVisible ());
    }

    [Test]
    public void IsSubMenuTabSetToEventVisibleWithoutWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined ();
      SubMenuTab subMenuTab = new SubMenuTab ();
      subMenuTab.Command.Type = CommandType.Event;
      Assert.IsTrue (subMenuTab.EvaluateVisible ());
    }
  }
}
