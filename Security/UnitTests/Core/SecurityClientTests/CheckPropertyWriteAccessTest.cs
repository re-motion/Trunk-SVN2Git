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
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CheckPropertyWriteAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatefulSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithDefaultAccessType_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }
  }
}
