using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CheckConstructorAccessTest
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
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessTypes.Create, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckConstructorAccess (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
    }
  }
}
