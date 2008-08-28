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
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeFunctionSecurityTest : WxeTest
  {
    private MockRepository _mocks;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockWxeSecurityAdapter = _mocks.StrictMock<IWxeSecurityAdapter> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _mockWxeSecurityAdapter);
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void ExecuteFunctionWithAccessDenied ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      LastCall.Throw (new PermissionDeniedException ("Test Exception"));
      _mocks.ReplayAll ();

      try
      {
        function.Execute ();
      }
      catch (WxeUnhandledException e)
      {
        _mocks.VerifyAll ();

        Assert.IsInstanceOfType (typeof (PermissionDeniedException), e.InnerException);
        return;
      }
      Assert.Fail ("Expected PermissionDeniedException.");
    }

    [Test]
    public void ExecuteFunctionWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);

      TestFunction function = new TestFunction ();
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
