using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CheckStatelessAccessTest
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
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));
      }

      _testHelper.VerifyAll ();
    }
  }
}
