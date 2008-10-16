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

namespace Remotion.Web.UnitTests.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class Security
  {
    private MockRepository _mockRepository;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;
    private WxeContext _wxeContext;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction ();
      WxeContextFactory contextFactory = new WxeContextFactory ();
      _wxeContext = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository ();
      _mockWxeSecurityAdapter = _mockRepository.StrictMock<IWxeSecurityAdapter> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _mockWxeSecurityAdapter);
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      _mockRepository.ReplayAll ();

      function.Execute (_wxeContext);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
      _mockRepository.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mockRepository.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}