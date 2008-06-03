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

namespace Remotion.Web.UnitTests.UI.Controls.WebMenuItemTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;
    private Command _mockCommand;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.CreateMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();
      _mockCommand = _mocks.CreateMock<Command> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsDisabled = false;

      bool isEnabled = menuItem.EvaluateEnabled ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsDisabled = true;

      bool isEnabled = menuItem.EvaluateEnabled ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (true);
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (false);
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsDisabled = true;
      _mocks.ReplayAll ();

      bool isEnabled = menuItem.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }

    private WebMenuItem CreateWebMenuItem ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.Command = _mockCommand;
      
      return menuItem;
    }

    private WebMenuItem CreateWebMenuItemWithoutCommand ()
    {
      WebMenuItem menuItem = new WebMenuItem ();
      menuItem.Command.Type = CommandType.None;
      menuItem.Command = null;
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.SecurableObject = _mockSecurableObject;

      return menuItem;
    }
  }
}
