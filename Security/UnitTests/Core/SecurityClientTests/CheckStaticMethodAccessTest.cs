using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CheckStaticMethodAccessTest
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
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessTypes.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       ExpectedMessage = "The member 'StaticMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod");
      _testHelper.ReplayAll ();

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The member 'StaticMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissionsAndWithinSecurityFreeSection_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod");
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredStaticMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "IPermissionProvider.GetRequiredStaticMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
    }
  }
}
