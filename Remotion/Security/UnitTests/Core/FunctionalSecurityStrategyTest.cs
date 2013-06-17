// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
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
    private ISecurityPrincipal _stubUser;
    private AccessType[] _accessTypeResult;
    private FunctionalSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.StrictMock<ISecurityStrategy> ();
      _stubSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();

      _stubUser = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_stubUser.User).Return ("user");
      _accessTypeResult = new[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };

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
      Assert.That (_strategy.SecurityStrategy, Is.SameAs (_mockSecurityStrategy));
    }

    [Test]
    [Ignore ("TODO RM-5521: test GLobalAccessTypeCache")]
    public void Initialize_WithDefaults ()
    {
      FunctionalSecurityStrategy strategy = new FunctionalSecurityStrategy ();

      Assert.IsInstanceOf (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOf (typeof (NullCache<ISecurityPrincipal, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      //Assert.That (((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider, Is.SameAs (stubGlobalCacheProvider));
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (true).Constraints (
          new FunctionalSecurityContextFactoryConstraint ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests"),
          Mocks_Is.Same (_stubSecurityProvider),
          Mocks_Is.Same (_stubUser),
          Mocks_List.Equal (_accessTypeResult));
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (false).Constraints (
          new FunctionalSecurityContextFactoryConstraint ("Remotion.Security.UnitTests.Core.SampleDomain.SecurableObject, Remotion.Security.UnitTests"),
          Mocks_Is.Same (_stubSecurityProvider),
          Mocks_Is.Same (_stubUser),
          Mocks_List.Equal (_accessTypeResult));
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.EqualTo (false));
    }
  }
}
