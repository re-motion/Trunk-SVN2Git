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

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class SecurityTestWithVisible : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.CreateMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      Expect.Call (_mockWebSecurityAdapter.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (true);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Call (_mockWebSecurityAdapter.HasAccess(_mockSecurableObject, new EventHandler (TestHandler))).Return (false);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = true;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Visible = false;
      _mocks.ReplayAll ();

      bool isVisible = button.Visible;

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.SecurableObject = _mockSecurableObject;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.SecurableObject = _mockSecurableObject;

      return button;
    }
  }
}
