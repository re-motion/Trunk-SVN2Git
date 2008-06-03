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
using Remotion.Security;
using Remotion.Web.Security.UI;
using Remotion.Web.UnitTests.Security.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Security.UI.WebSecurityAdapterTests
{
  [TestFixture]
  public class PermissionFromWxeFunctionTest
  {
    private IWebSecurityAdapter _securityAdapter;
    private WebPermissionProviderTestHelper _testHelper;
  
    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WebSecurityAdapter ();

      _testHelper = new WebPermissionProviderTestHelper ();
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _testHelper.WxeSecurityAdapter);
    }

    [TearDown]
    public void TearDown ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasStatelessAccessForWxeFunction (typeof (TestFunctionWithThisObject), true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasStatelessAccessForWxeFunction (typeof (TestFunctionWithThisObject), false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetWxeFunctionPermission (typeof (TestFunctionWithThisObject))]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }

  }
}
