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
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.ObjectSecurityStrategyTests
{
  [TestFixture]
  public class HasAccessWithFilter_ObjectSecurityStratetyTest
  {
    private ISecurityProvider _securityProviderStub;
    private IAccessTypeFilter _accessTypeFilterMock;
    private ISecurityContextFactory _securityContextFactoryStub;
    private ISecurityPrincipal _principalStub;
    private SecurityContext _context;
    private CacheInvalidationToken _cacheInvalidationToken;
    private IObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      _accessTypeFilterMock = MockRepository.GenerateStrictMock<IAccessTypeFilter>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _principalStub.Stub (_ => _.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);

      _securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();
      _securityContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      _cacheInvalidationToken = CacheInvalidationToken.Create();
      _strategy = new ObjectSecurityStrategy (_securityContextFactoryStub, _accessTypeFilterMock, _cacheInvalidationToken);
    }

    [Test]
    public void HasAccess_FiltersResultButRequiredAccessTypeRemains_ReturnsTrue ()
    {
      var availableAccessTypes = new[]
                                 {
                                     AccessType.Get (GeneralAccessTypes.Create),
                                     AccessType.Get (GeneralAccessTypes.Delete),
                                     AccessType.Get (GeneralAccessTypes.Read)
                                 };
      _securityProviderStub
          .Stub (_ => _.GetAccess (_context, _principalStub))
          .Return (availableAccessTypes);

      _accessTypeFilterMock
          .Expect (_ => _.Filter (availableAccessTypes, _context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Delete),
                  AccessType.Get (GeneralAccessTypes.Create)
              });

      bool hasAccess = _strategy.HasAccess (
          _securityProviderStub,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Delete) });

      Assert.That (hasAccess, Is.True);
      _accessTypeFilterMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_FiltersResultAndRemovesRequiredAccessType_ReturnsFalse ()
    {
      var availableAccessTypes = new[]
                                 {
                                     AccessType.Get (GeneralAccessTypes.Create),
                                     AccessType.Get (GeneralAccessTypes.Delete),
                                     AccessType.Get (GeneralAccessTypes.Read)
                                 };
      _securityProviderStub
          .Stub (_ => _.GetAccess (_context, _principalStub))
          .Return (availableAccessTypes);

      _accessTypeFilterMock
          .Expect (_ => _.Filter (availableAccessTypes, _context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Create)
              });

      bool hasAccess = _strategy.HasAccess (
          _securityProviderStub,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Delete) });

      Assert.That (hasAccess, Is.False);
      _accessTypeFilterMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_FilterAppliesBeforeCache_ReturnsTrueOrFalseDependingOnRequest ()
    {
      var availableAccessTypes = new[]
                                 {
                                     AccessType.Get (GeneralAccessTypes.Create),
                                     AccessType.Get (GeneralAccessTypes.Delete),
                                     AccessType.Get (GeneralAccessTypes.Read)
                                 };
      _securityProviderStub
          .Stub (_ => _.GetAccess (_context, _principalStub))
          .Return (availableAccessTypes);

      _accessTypeFilterMock
          .Expect (_ => _.Filter (availableAccessTypes, _context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Delete),
                  AccessType.Get (GeneralAccessTypes.Create)
              })
          .Repeat.Once();

      bool hasAccessOnFirstCall = _strategy.HasAccess (
          _securityProviderStub,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Delete) });

      Assert.That (hasAccessOnFirstCall, Is.True);

      bool hasAccessOnSecondCall = _strategy.HasAccess (
          _securityProviderStub,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnSecondCall, Is.False);
      _accessTypeFilterMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_InjectsNewAccessType_ThrowsInvalidOperationException ()
    {
      var availableAccessTypes = new[]
                                 {
                                     AccessType.Get (GeneralAccessTypes.Create),
                                     AccessType.Get (GeneralAccessTypes.Delete),
                                     AccessType.Get (GeneralAccessTypes.Read)
                                 };
      _securityProviderStub
          .Stub (_ => _.GetAccess (_context, _principalStub))
          .Return (availableAccessTypes);

      _accessTypeFilterMock
          .Expect (_ => _.Filter (availableAccessTypes, _context, _principalStub))
          .Return (
              new[]
              {
                  AccessType.Get (GeneralAccessTypes.Delete),
                  AccessType.Get (GeneralAccessTypes.Create),
                  AccessType.Get (GeneralAccessTypes.Search),
                  AccessType.Get (GeneralAccessTypes.Edit)
              });

      Assert.That (
          () => _strategy.HasAccess (
              _securityProviderStub,
              _principalStub,
              new[] { AccessType.Get (GeneralAccessTypes.Delete) }),
          Throws.InvalidOperationException.With.Message.StringStarting (
              "The access type filter injected additional access types ('Search|Remotion.Security.GeneralAccessTypes, Remotion.Security', 'Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security') into the filter result."));

      _accessTypeFilterMock.VerifyAllExpectations();
    }
  }
}