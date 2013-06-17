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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityStrategyTests
{
  [TestFixture]
  public class HasAccessWithReentrancy_SecurityStrategyTest
  {
    private class GlobalAccessTypeCache : IGlobalAccessTypeCache
    {
      private LazyLockingCachingAdapter<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> _innerCache;

      public GlobalAccessTypeCache ()
      {
        _innerCache = CacheFactory.CreateWithLazyLocking<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>();
      }

      public bool IsNull
      {
        get { return false; }
      }

      public AccessType[] GetOrCreateValue (Tuple<ISecurityContext, ISecurityPrincipal> key, Func<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> valueFactory)
      {
        return _innerCache.GetOrCreateValue (key, valueFactory);
      }

      public bool TryGetValue (Tuple<ISecurityContext, ISecurityPrincipal> key, out AccessType[] value)
      {
        return _innerCache.TryGetValue (key, out value);
      }

      public void Clear ()
      {
        _innerCache.Clear();
      }
    }

    private ISecurityProvider _securityProviderStub;
    private IGlobalAccessTypeCache _globalAccessTypeCache;
    private ICache<ISecurityPrincipal, AccessType[]> _localAccessTypeCache;
    private ISecurityPrincipal _principalStub;
    private SecurityContext _context;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      _globalAccessTypeCache = new GlobalAccessTypeCache ();
      _localAccessTypeCache = CacheFactory.Create<ISecurityPrincipal, AccessType[]>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _principalStub.Stub (_ => _.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void ReentrancyInGlobalCache_WithEqualSecurityContext_ThrowsInvalidOperationException ()
    {
      var securityStrategy = new SecurityStrategy (_localAccessTypeCache, _globalAccessTypeCache);
      _securityProviderStub.Stub (_ => _.GetAccess (_context, _principalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      var secondContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      secondContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      var firstContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      bool isExceptionThrownBySecondHasAccess = false;
      firstContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (null).WhenCalled (
          mi =>
          {
            var exception = Assert.Throws<InvalidOperationException> (
                () => securityStrategy.HasAccess (
                    secondContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Find)));
            isExceptionThrownBySecondHasAccess = true;
            throw exception;
          });

      Assert.That (
          () => securityStrategy.HasAccess (firstContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Read)),
          Throws.InvalidOperationException
                .With.Message.StartsWith (
                    "Multiple reentrancies on SecurityStrategy.HasAccess(...) are not allowed as they can indicate a possible infinite recursion."));

      Assert.That (isExceptionThrownBySecondHasAccess, Is.True);
    }

    [Test]
    public void ReentrancyInGlobalCache_AccrossMultipleSecurityStrategyInstances_ThrowsInvalidOperationException ()
    {
      var firstSecurityStrategy = new SecurityStrategy (_localAccessTypeCache, _globalAccessTypeCache);
      var secondSecurityStrategy = new SecurityStrategy (_localAccessTypeCache, _globalAccessTypeCache);
      _securityProviderStub.Stub (_ => _.GetAccess (_context, _principalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      var secondContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      secondContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      var firstContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      bool isExceptionThrownBySecondHasAccess = false;
      firstContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (null).WhenCalled (
          mi =>
          {
            var exception = Assert.Throws<InvalidOperationException> (
                () => secondSecurityStrategy.HasAccess (
                    secondContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Find)));
            isExceptionThrownBySecondHasAccess = true;
            throw exception;
          });

      Assert.That (
          () =>
          firstSecurityStrategy.HasAccess (firstContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Read)),
          Throws.InvalidOperationException);

      Assert.That (isExceptionThrownBySecondHasAccess, Is.True);
    }

    [Test]
    public void ExceptionDuringAccessTypeRetrieval_ResetsReentrancyForSubsequentCalls ()
    {
      var securityStrategy = new SecurityStrategy (_localAccessTypeCache, _globalAccessTypeCache);
      _securityProviderStub.Stub (_ => _.GetAccess (_context, _principalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      var exception = new Exception();
      var firstContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      firstContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Throw (exception);

      Assert.That (
          () => securityStrategy.HasAccess (firstContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Edit)),
          Throws.Exception.SameAs (exception));

      var secondContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      secondContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      Assert.That (
          securityStrategy.HasAccess (secondContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Edit)),
          Is.False);
      Assert.That (
          securityStrategy.HasAccess (secondContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Read)),
          Is.True);
      _securityProviderStub.AssertWasCalled (_ => _.GetAccess (_context, _principalStub), mo => mo.Repeat.Once());
    }

    [Test]
    public void ReentrancyCheckIsScopedToThread_MultipleThreadsCanPerformSecurityEvaluationConcurrently ()
    {
      var securityStrategy = new SecurityStrategy (_localAccessTypeCache, _globalAccessTypeCache);
      _securityProviderStub.Stub (_ => _.GetAccess (_context, _principalStub)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      var secondContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      secondContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      var firstContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      bool isFirstContextCreated = false;
      firstContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (null).WhenCalled (
          mi =>
          {
            isFirstContextCreated = true;
            ThreadRunner.Run (
                () =>
                {
                  var securityStrategyOnOtherThread =
                      new SecurityStrategy (CacheFactory.Create<ISecurityPrincipal, AccessType[]>(), _globalAccessTypeCache);
                  var hasAccess = securityStrategyOnOtherThread.HasAccess (
                      secondContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Read));
                  Assert.That (hasAccess, Is.True);
                });
            mi.ReturnValue = _context;
          });

      var result = securityStrategy.HasAccess (firstContextFactoryStub, _securityProviderStub, _principalStub, AccessType.Get (GeneralAccessTypes.Read));

      Assert.That (isFirstContextCreated, Is.True);
      _securityProviderStub.AssertWasCalled (_ => _.GetAccess (_context, _principalStub), mo => mo.Repeat.Once());
      Assert.That (result, Is.True);
    }
  }
}