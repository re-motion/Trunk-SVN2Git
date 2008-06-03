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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Security
{
  [TestFixture]
  public class DomainObjectSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityProvider _stubSecurityProvider;
    private IDomainObjectSecurityContextFactory _stubContextFactory;
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.CreateMock<ISecurityStrategy> ();
      _stubSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      _stubContextFactory = _mocks.CreateMock<IDomainObjectSecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = new NullGlobalAccessTypeCacheProvider ();
    }

    [Test]
    public void Initialize ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.New, _stubContextFactory, _mockSecurityStrategy);
      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.AreSame (_mockSecurityStrategy, strategy.SecurityStrategy);
      Assert.AreEqual (RequiredSecurityForStates.New, strategy.RequiredSecurityForStates);
    }

    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory);

      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (Cache<string, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, ((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_StateIsNew ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_StateIsDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForNew()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.New, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.Deleted, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForNewAndDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.NewAndDeleted, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }
    [Test]
    public void HasAccess_WithAccessGrantedAndStateIsDiscarded ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityProvider, _user, _accessTypeResult)).Return (false);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityProvider, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Serializable]
    class SerializableFactory : IDomainObjectSecurityContextFactory
    {
      public bool IsDiscarded { get { return false; } }
      public bool IsNew { get { return false; } }
      public bool IsDeleted { get { return false; } }

      public ISecurityContext CreateSecurityContext ()
      {
        throw new NotImplementedException ();
      }
    }

    [Test]
    public void Serialize ()
    {
      IDomainObjectSecurityContextFactory factory = new SerializableFactory ();
      ISecurityStrategy securityStrategy = new SecurityStrategy ();
      
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.NewAndDeleted, factory, securityStrategy);
      DomainObjectSecurityStrategy deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);

      Assert.AreNotSame (strategy, deserializedStrategy);
      Assert.AreEqual (RequiredSecurityForStates.NewAndDeleted, deserializedStrategy.RequiredSecurityForStates);
      Assert.AreNotSame (factory, deserializedStrategy.SecurityContextFactory);
      Assert.IsInstanceOfType (typeof (SerializableFactory), deserializedStrategy.SecurityContextFactory);
      Assert.AreNotSame (securityStrategy, deserializedStrategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (SecurityStrategy), deserializedStrategy.SecurityStrategy);
    }
  }
}
