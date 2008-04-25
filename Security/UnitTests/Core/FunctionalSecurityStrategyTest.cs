using System;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Collections;
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.MockConstraints;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Security.UnitTests.Core
{

  [TestFixture]
  public class FunctionalSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityProvider _stubSecurityProvider;
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;
    private FunctionalSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.CreateMock<ISecurityStrategy> ();
      _stubSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };

      _strategy = new FunctionalSecurityStrategy (_mockSecurityStrategy);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_mockSecurityStrategy, _strategy.SecurityStrategy);
    }
    
    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      FunctionalSecurityStrategy strategy = new FunctionalSecurityStrategy ();

      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (NullCache<string, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, ((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (true).Constraints (
          new FunctionalSecurityContextFactoryConstraint ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests"),
          Mocks_Is.Same (_stubSecurityProvider),
          Mocks_Is.Same (_user),
          Mocks_List.Equal (_accessTypeResult));
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (false).Constraints (
          new FunctionalSecurityContextFactoryConstraint ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests"),
          Mocks_Is.Same (_stubSecurityProvider),
          Mocks_Is.Same (_user),
          Mocks_List.Equal (_accessTypeResult));
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
