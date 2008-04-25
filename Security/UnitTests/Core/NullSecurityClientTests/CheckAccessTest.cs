using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class CheckAccessTest
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

      _securityClient.CheckAccess (_testHelper.SecurableObject, AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      _testHelper.ReplayAll();

      using (new SecurityFreeSection())
      {
        _securityClient.CheckAccess (_testHelper.SecurableObject, AccessType.Get (TestAccessTypes.First));
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull()
    {
      _testHelper.ReplayAll();

      _securityClient.CheckAccess (new SecurableObject (null), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll();
    }
  }
}