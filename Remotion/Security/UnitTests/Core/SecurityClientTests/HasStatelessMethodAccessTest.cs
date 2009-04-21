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
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class HasStatelessMethodAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("InstanceMethod", TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "InstanceMethod");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("InstanceMethod", TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "InstanceMethod");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("InstanceMethod", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "InstanceMethod");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The member 'InstanceMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("InstanceMethod");
      _testHelper.ReplayAll ();

      _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "InstanceMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The member 'InstanceMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissionsAndWithinSecurityFreeSection_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("InstanceMethod");
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "InstanceMethod");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("StaticMethod", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions ("StaticMethod", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.HasStatelessMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
    }
  }
}
