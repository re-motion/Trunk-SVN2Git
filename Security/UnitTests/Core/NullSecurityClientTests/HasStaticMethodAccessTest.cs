using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class HasStaticMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_AccessGranted()
    {
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

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
        hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }
  }
}