using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.UnitTests.Security.Configuration;
using Rhino.Mocks;
using Remotion.Security.Configuration;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeSecurityAdapterTestWithoutPermissionAttribute
  {
    // types

    // static members

    // member fields

    private IWxeSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _mockUserProvider;

    // construction and disposing

    public WxeSecurityAdapterTestWithoutPermissionAttribute ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WxeSecurityAdapter ();

      _mocks = new MockRepository ();

      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void CheckAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      _securityAdapter.CheckAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (new TestFunctionWithoutPermissions ());

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccess_AccessGranted ()
    {
      _mocks.ReplayAll ();
      
      bool hasAccess = _securityAdapter.HasStatelessAccess (typeof (TestFunctionWithoutPermissions));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}