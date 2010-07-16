// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Reflection;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyWriteAccessTest_WithPropertyName
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private IPropertyInformation _propertyInformation;
    
    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
      _propertyInformation = MockRepository.GenerateMock<IPropertyInformation>();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.Second);

      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.Second, true);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      Assert.IsTrue (hasAccess);
      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.Second);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.Second, false);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      _testHelper.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, true);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, false);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation);
      _testHelper.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, TestAccessTypes.First);
      _testHelper.ReplayAll();

      _securityClient.HasPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, (Enum[]) null);
      _testHelper.ReplayAll();

      _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetPropertyInformation ("InstanceProperty", _propertyInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions (_propertyInformation, (Enum[]) null);
      _testHelper.ReplayAll();

      using (new SecurityFreeSection())
      {
        _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll();
    }

  }
}