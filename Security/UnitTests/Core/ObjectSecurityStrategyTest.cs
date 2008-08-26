/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Security.Principal;
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
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;
    private ObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.StrictMock<ISecurityStrategy> ();
      _stubSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      _stubContextFactory = _mocks.StrictMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };

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
      Assert.AreSame (_stubContextFactory, _strategy.SecurityContextFactory);
      Assert.AreSame (_mockSecurityStrategy, _strategy.SecurityStrategy);
    }

    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.StrictMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      ObjectSecurityStrategy strategy = new ObjectSecurityStrategy (_stubContextFactory);

      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (Cache<string, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, ((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
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
      SecurityStrategy securityStrategy = new SecurityStrategy(new NullCache<string, AccessType[]>(), new NullGlobalAccessTypeCacheProvider());
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      ObjectSecurityStrategy strategy = new ObjectSecurityStrategy (factory, securityStrategy);

      ObjectSecurityStrategy deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);
      Assert.AreNotSame (strategy, deserializedStrategy);

      Assert.IsNotNull (deserializedStrategy.SecurityContextFactory);
      Assert.AreNotSame (strategy.SecurityContextFactory, deserializedStrategy.SecurityContextFactory);

      Assert.IsNotNull (deserializedStrategy.SecurityStrategy);
      Assert.AreNotSame (strategy.SecurityStrategy, deserializedStrategy.SecurityStrategy);
    }
  }
}
