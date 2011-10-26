﻿// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetSubstitution : DomainTest
  {
    private User _user;
    private Tenant _tenant;
    private Substitution _substitution;
    private SecurityManagerPrincipal _principal;

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      _user = User.FindByUserName ("substituting.user");
      _tenant = _user.Tenant;
      _substitution = _user.GetActiveSubstitutions().First();

      _principal = new SecurityManagerPrincipal (_tenant.ID, _user.ID, _substitution.ID);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Test ()
    {
      var substitutionProxy = _principal.Substitution;

      Assert.That (substitutionProxy.ID, Is.EqualTo (_substitution.ID));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That (_principal.Substitution, Is.SameAs (_principal.Substitution));
    }

    [Test]
    public void DoesNotCacheDuringSerialization ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (Tuple.Create (_principal, _principal.Substitution));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      SubstitutionProxy deserialziedSubstitution = deserialized.Item2;

      Assert.That (deserialziedSecurityManagerPrincipal.Substitution, Is.Not.SameAs (deserialziedSubstitution));
    }

    [Test]
    public void RefreshDoesNotResetCacheWithOldRevision ()
    {
      SubstitutionProxy proxy = _principal.Substitution;
      _principal.Refresh();
      Assert.That (proxy, Is.SameAs (_principal.Substitution));
    }

    [Test]
    public void RefreshResetsCacheWithNewRevision ()
    {
      SubstitutionProxy proxy = _principal.Substitution;
      Revision.IncrementRevision();
      _principal.Refresh();
      Assert.That (proxy, Is.Not.SameAs (_principal.Substitution));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;
      Revision.IncrementRevision();
      _principal.Refresh();

      var substitutionProxy = _principal.Substitution;

      Assert.That (substitutionProxy.ID, Is.EqualTo (_substitution.ID));
    }
  }
}