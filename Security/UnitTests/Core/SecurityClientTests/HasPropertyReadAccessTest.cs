using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyReadAccessTest
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
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty");
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      _securityClient.HasPropertyReadAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredPropertyReadPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredPropertyReadPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }
  }
}
