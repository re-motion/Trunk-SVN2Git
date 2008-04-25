using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class HasMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "InstanceMethod");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "InstanceMethod");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), "InstanceMethod");
    
      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
