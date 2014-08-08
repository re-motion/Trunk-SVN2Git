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
  public class HasAccessWithCaching_ObjectSecurityStrategyTest
  {
    private ISecurityProvider _securityProviderMock;
    private ISecurityContextFactory _securityContextFactoryStub;
    private ISecurityPrincipal _principalStub;
    private SecurityContext _context;
    private CacheInvalidationToken _cacheInvalidationToken;
    private ObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _securityProviderMock = MockRepository.GenerateStrictMock<ISecurityProvider>();
      _securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _principalStub.Stub (_ => _.User).Return ("user");
      _context = SecurityContext.Create (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum>(), new Enum[0]);
      _securityContextFactoryStub.Stub (_ => _.CreateSecurityContext()).Return (_context);

      _cacheInvalidationToken = CacheInvalidationToken.Create();
      _strategy = new ObjectSecurityStrategy (_securityContextFactoryStub, _cacheInvalidationToken);
    }

    [Test]
    public void HasAccess_WithAccessGranted_OnlyRequestsAccessTypesOnce_ReturnsTrue ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (new[]{AccessType.Get (GeneralAccessTypes.Read)})
          .Repeat.Once();

      bool hasAccessOnFirstCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnFirstCall, Is.True);

      bool hasAccessOnSecondCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnSecondCall, Is.True);

      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_WithAccessDenied_OnlyRequestsAccessTypesOnce_ReturnsFalse ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (new[]{AccessType.Get (GeneralAccessTypes.Create)})
          .Repeat.Once();

      bool hasAccessOnFirstCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnFirstCall, Is.False);

      bool hasAccessOnSecondCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnSecondCall, Is.False);

      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_WithAccessGranted_AndWithAccessDenied_OnlyRequestsAccessTypesOnce_ReturnsBooleanBasedOnRequestedAccess ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (new[]{AccessType.Get (GeneralAccessTypes.Create)})
          .Repeat.Once();

      bool hasAccessOnFirstCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnFirstCall, Is.False);

      bool hasAccessOnSecondCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Create) });

      Assert.That (hasAccessOnSecondCall, Is.True);

      _securityProviderMock.VerifyAllExpectations();
    }

    [Test]
    public void HasAccess_WithCacheInvalidation_RequestsNewAccessTypes_ReturnsBooleanBasedOnRequestedAccess ()
    {
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (new[]{AccessType.Get (GeneralAccessTypes.Read)})
          .Repeat.Once();

      bool hasAccessOnFirstCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccessOnFirstCall, Is.True);
      _securityProviderMock.VerifyAllExpectations();

      _strategy.CacheInvalidationToken.Invalidate();

      _securityProviderMock.BackToRecord();
      _securityProviderMock
          .Expect (_ => _.GetAccess (_context, _principalStub))
          .Return (new[]{AccessType.Get (GeneralAccessTypes.Create)})
          .Repeat.Once();
      _securityProviderMock.Replay();

      bool hasAccessOnSecondCall = _strategy.HasAccess (
          _securityProviderMock,
          _principalStub,
          new[] { AccessType.Get (GeneralAccessTypes.Create) });

      Assert.That (hasAccessOnSecondCall, Is.True);
      _securityProviderMock.VerifyAllExpectations();
    }
  }
}
