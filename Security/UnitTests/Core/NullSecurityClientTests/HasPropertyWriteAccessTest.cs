using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class HasPropertyWriteAccessTest
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

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      Assert.IsTrue (hasAccess);
      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
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
    public void Test_WithSecurityStrategyIsNull()
    {
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }
  }
}