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
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Security.UnitTests.Core.SecurityStrategyTests
{
  [TestFixture]
  public class HasAccessWithCaching_SecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IGlobalAccessTypeCache _mockGlobalAccessTypeCache;
    private ICache<ISecurityPrincipal, AccessType[]> _mockLocalAccessTypeCache;
    private ISecurityContextFactory _mockContextFactory;
    private ISecurityPrincipal _stubUser;
    private SecurityContext _context;
    private GlobalAccessTypeCacheKey _globalAccessTypeCacheKey;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      _mockGlobalAccessTypeCache = _mocks.StrictMock<IGlobalAccessTypeCache>();
      _mockLocalAccessTypeCache = _mocks.StrictMock<ICache<ISecurityPrincipal, AccessType[]>>();
      _mockContextFactory = _mocks.StrictMock<ISecurityContextFactory>();

        _stubUser = _mocks.Stub<ISecurityPrincipal> ();
        SetupResult.For (_stubUser.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum> (), new Enum[0]);
      _globalAccessTypeCacheKey = new GlobalAccessTypeCacheKey (_context, _stubUser);

      _strategy = new SecurityStrategy (_mockLocalAccessTypeCache, _mockGlobalAccessTypeCache);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void Initialize()
    {
      Assert.That (_strategy.LocalCache, Is.SameAs (_mockLocalAccessTypeCache));
      Assert.That (_strategy.GlobalCache, Is.SameAs (_mockGlobalAccessTypeCache));
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new [] { AccessType.Get (GeneralAccessTypes.Edit) };
      using (_mocks.Ordered())
      {
        AccessType[] value;
        Expect.Call (_mockLocalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactory<ISecurityPrincipal> ());
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.Anything()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<GlobalAccessTypeCacheKey>());
        Expect.Call (_mockSecurityProvider.GetAccess (_context, _stubUser)).Return (accessTypeResult);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheAndNotInGlobalCacheAndAccessDenied ()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      using (_mocks.Ordered())
      {
        AccessType[] value;
        Expect.Call (_mockLocalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactory<ISecurityPrincipal> ());
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.Anything()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockGlobalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.NotNull())
            .Do (GetOrCreateValueFromValueFactory<GlobalAccessTypeCacheKey>());
        Expect.Call (_mockSecurityProvider.GetAccess (_context, _stubUser)).Return (accessTypeResult);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessGranted ()
    {
      AccessType[] accessTypeResult = new [] { AccessType.Get (GeneralAccessTypes.Edit) };
      using (_mocks.Ordered())
      {
        AccessType[] value;
        Expect.Call (_mockLocalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactory<ISecurityPrincipal> ());
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.Anything()).OutRef (new object[] { accessTypeResult })
            .Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithResultNotInLocalCacheButInGlobalCacheAndAccessDenied ()
    {
      using (_mocks.Ordered())
      {
        AccessType[] value;
        Expect.Call (_mockLocalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new object[] { new AccessType[0] })
            .Return (false);
        Expect.Call (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.NotNull ())
            .Do (GetOrCreateValueFromValueFactory<ISecurityPrincipal> ());
        Expect.Call (_mockContextFactory.CreateSecurityContext()).Return (_context);
        Expect.Call (_mockGlobalAccessTypeCache.TryGetValue (null, out value))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Equal (_globalAccessTypeCacheKey), Mocks_Is.Anything()).OutRef (new object[] { new AccessType[0] })
            .Return (true);
      }
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasAccess_WithResultInLocalCacheAndAccessDenied()
    {
      AccessType[] accessTypeResult = new AccessType[0];
      AccessType[] value;
      Expect.Call (_mockLocalAccessTypeCache.TryGetValue (null, out value))
          .IgnoreArguments()
          .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new object[] { accessTypeResult })
          .Return (true);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.")]
    public void HasAccess_WithSecurityContextFactoryReturningNull()
    {
      AccessType[] value;
      SetupResult.For (_mockLocalAccessTypeCache.TryGetValue (null, out value))
          .IgnoreArguments ()
          .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.Anything ()).OutRef (new AccessType[0])
          .Return (false);
      SetupResult.For (_mockLocalAccessTypeCache.GetOrCreateValue (null, null))
          .IgnoreArguments ()
          .Constraints (Mocks_Is.Same (_stubUser), Mocks_Is.NotNull ())
          .Do (GetOrCreateValueFromValueFactory<ISecurityPrincipal> ());
      SetupResult.For (_mockContextFactory.CreateSecurityContext()).Return (null);
      _mocks.ReplayAll();

      _strategy.HasAccess (_mockContextFactory, _mockSecurityProvider, _stubUser, AccessType.Get (GeneralAccessTypes.Edit));
    }

    [Test]
    public void InvalidateLocalCache()
    {
      _mockLocalAccessTypeCache.Clear();
      _mocks.ReplayAll();

      _strategy.InvalidateLocalCache();

      _mocks.VerifyAll();
    }

    /// <summary>
    /// Creates a delegate for ICache.GetOrCreateValue that calls the valueFactory and returns its result.
    /// </summary>
    private Func<TKey, Func<TKey, AccessType[]>, AccessType[]> GetOrCreateValueFromValueFactory<TKey> ()
    {
      return (key, valueFactory) => valueFactory (key);
    }
  }
}
