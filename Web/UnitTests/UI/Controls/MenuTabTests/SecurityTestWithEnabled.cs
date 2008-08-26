/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;
    private NavigationCommand _mockNavigationCommand;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.StrictMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.StrictMock<ISecurableObject> ();
      _mockNavigationCommand = _mocks.StrictMock<NavigationCommand> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsDisabled = false;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsDisabled = true;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (true);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (false);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = true;
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }

    private MainMenuTab CreateMainMenuTab ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.Command = _mockNavigationCommand;
      
      return mainMenuTab;
    }

    private MainMenuTab CreateMainMenuTabWithoutCommand ()
    {
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.None;
      mainMenuTab.Command = null;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;

      return mainMenuTab;
    }
  }
}
