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
  public class SecurityTestWithVisible : BaseTest
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
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.IsVisible = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsVisible = true;

      bool isVisible = menuItem.EvaluateVisible ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsVisible = false;

      bool isVisible = menuItem.EvaluateVisible ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (true);
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (false);
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = false;
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
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
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.SecurableObject = _mockSecurableObject;
      
      return menuItem;
    }
  }
}
