using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class HasConstructorAccessTest
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

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessTypes.Create, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();
      bool hasAccess;
      
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
