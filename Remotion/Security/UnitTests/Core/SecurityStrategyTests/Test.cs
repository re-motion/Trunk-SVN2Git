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
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityStrategyTests
{
  [TestFixture]
  public class Test
  {
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private ISecurityContextFactory _stubContextFactory;
    private ISecurityPrincipal _userStub;
    private SecurityContext _context;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      _stubContextFactory = _mocks.StrictMock<ISecurityContextFactory>();

      _userStub = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_userStub.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);
      SetupResult.For (_stubContextFactory.CreateSecurityContext()).Return (_context);

      _strategy = new SecurityStrategy (new NullCache<ISecurityPrincipal, AccessType[]>(), new NullGlobalAccessTypeCacheProvider());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (new [] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _userStub, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (new [] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _userStub, AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      AccessType[] mockResult = new[]
                                {
                                    AccessType.Get (GeneralAccessTypes.Create),
                                    AccessType.Get (GeneralAccessTypes.Delete),
                                    AccessType.Get (GeneralAccessTypes.Read)
                                };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (mockResult);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _userStub, AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new[]
                                {
                                    AccessType.Get (GeneralAccessTypes.Create),
                                    AccessType.Get (GeneralAccessTypes.Delete),
                                    AccessType.Get (GeneralAccessTypes.Read)
                                };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (mockResult);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (
          _stubContextFactory,
          _mockSecurityProvider,
          _userStub,
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new[]
                                {
                                    AccessType.Get (GeneralAccessTypes.Create),
                                    AccessType.Get (GeneralAccessTypes.Delete),
                                    AccessType.Get (GeneralAccessTypes.Read)
                                };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (mockResult);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (
          _stubContextFactory,
          _mockSecurityProvider,
          _userStub,
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Find));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (new AccessType[0]);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (
          _stubContextFactory,
          _mockSecurityProvider,
          _userStub,
          AccessType.Get (GeneralAccessTypes.Find),
          AccessType.Get (GeneralAccessTypes.Edit),
          AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _userStub)).Return (null);
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (
          _stubContextFactory,
          _mockSecurityProvider,
          _userStub,
          AccessType.Get (GeneralAccessTypes.Find),
          AccessType.Get (GeneralAccessTypes.Edit),
          AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll();
      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void Serialization ()
    {
      SecurityStrategy strategy =
          new SecurityStrategy (new Cache<ISecurityPrincipal, AccessType[]>(), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider);
      AccessType[] accessTypes = new[] { AccessType.Get (GeneralAccessTypes.Find) };
      strategy.LocalCache.GetOrCreateValue (new SecurityPrincipal ("foo", null, null, null), delegate { return accessTypes; });

      SecurityStrategy deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);
      Assert.That (deserializedStrategy, Is.Not.SameAs (strategy));
      Assert.That (deserializedStrategy.GlobalCacheProvider, Is.SameAs (SecurityConfiguration.Current.GlobalAccessTypeCacheProvider));

      AccessType[] newAccessTypes;
      bool result = deserializedStrategy.LocalCache.TryGetValue (new SecurityPrincipal ("foo", null, null, null), out newAccessTypes);
      Assert.That (result, Is.True);
      Assert.That (newAccessTypes, Is.EquivalentTo (accessTypes));
    }
  }
}
