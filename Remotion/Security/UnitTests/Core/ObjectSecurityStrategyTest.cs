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
using Remotion.Development.UnitTesting;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ObjectSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityProvider _stubSecurityProvider;
    private ISecurityContextFactory _stubContextFactory;
    private ISecurityPrincipal _stubUser;
    private AccessType[] _accessTypeResult;
    private ObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.StrictMock<ISecurityStrategy> ();
      _stubSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      _stubContextFactory = _mocks.StrictMock<ISecurityContextFactory> ();

      _stubUser = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_stubUser.User).Return ("user");
      _accessTypeResult = new[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };

      _strategy = new ObjectSecurityStrategy (_stubContextFactory, _mockSecurityStrategy);

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
      Assert.That (_strategy.SecurityContextFactory, Is.SameAs (_stubContextFactory));
      Assert.That (_strategy.SecurityStrategy, Is.SameAs (_mockSecurityStrategy));
    }

    [Test]
    [Ignore ("TODO RM-5521: test GLobalAccessTypeCache")]
    public void Initialize_WithDefaults ()
    {
      ObjectSecurityStrategy strategy = new ObjectSecurityStrategy (_stubContextFactory);

      Assert.That (strategy.SecurityContextFactory, Is.SameAs (_stubContextFactory));
      Assert.IsInstanceOf (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOf (typeof (Cache<ISecurityPrincipal, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      //Assert.That (((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider, Is.SameAs (stubGlobalCacheProvider));
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _stubUser, _accessTypeResult)).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _stubUser, _accessTypeResult)).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityProvider, _stubUser, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void InvalidateLocalCache ()
    {
      _mockSecurityStrategy.InvalidateLocalCache ();
      _mocks.ReplayAll ();

      _strategy.InvalidateLocalCache ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void Serialization ()
    {
      SecurityStrategy securityStrategy = new SecurityStrategy(new NullCache<ISecurityPrincipal, AccessType[]>(), new NullGlobalAccessTypeCache());
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      ObjectSecurityStrategy strategy = new ObjectSecurityStrategy (factory, securityStrategy);

      ObjectSecurityStrategy deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);
      Assert.That (deserializedStrategy, Is.Not.SameAs (strategy));

      Assert.That (deserializedStrategy.SecurityContextFactory, Is.Not.Null);
      Assert.That (deserializedStrategy.SecurityContextFactory, Is.Not.SameAs (strategy.SecurityContextFactory));

      Assert.That (deserializedStrategy.SecurityStrategy, Is.Not.Null);
      Assert.That (deserializedStrategy.SecurityStrategy, Is.Not.SameAs (strategy.SecurityStrategy));
    }
  }
}
