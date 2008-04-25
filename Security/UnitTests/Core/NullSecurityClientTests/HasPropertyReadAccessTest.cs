using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class HasPropertyReadAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_AccessGranted()
    {
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      _testHelper.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityClient.HasPropertyReadAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull()
    {
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyReadAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }
  }
}