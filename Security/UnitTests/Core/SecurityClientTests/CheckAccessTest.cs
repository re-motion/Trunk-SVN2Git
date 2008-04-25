using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CheckAccessTest
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
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckAccess (_testHelper.SecurableObject, AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckAccess (_testHelper.SecurableObject, AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckAccess (_testHelper.SecurableObject, AccessType.Get (TestAccessTypes.First));
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckAccess (new SecurableObject (null), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
    }
  }
}
