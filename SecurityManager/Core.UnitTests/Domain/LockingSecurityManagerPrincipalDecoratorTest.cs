// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class LockingSecurityManagerPrincipalDecoratorTest : DomainTest
  {
    private ISecurityManagerPrincipal _innerPrincipalMock;
    private LockingSecurityManagerPrincipalDecorator _decorator;
    private LockingSecurityManagerPrincipalDecoratorTestHelper _lockingTestHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _innerPrincipalMock = MockRepository.GenerateMock<ISecurityManagerPrincipal>();
      _decorator = new LockingSecurityManagerPrincipalDecorator (_innerPrincipalMock);

      _lockingTestHelper = new LockingSecurityManagerPrincipalDecoratorTestHelper (_decorator, _innerPrincipalMock);
    }

    [Test]
    public void Get_Members ()
    {
      var helper = new OrganizationalStructureTestHelper();
      helper.Transaction.EnterNonDiscardingScope();

      var tenant = helper.CreateTenant ("TheTenant", "Tenant UID");
      var group = helper.CreateGroup ("TheGroup", "Group UID", null, tenant);
      var user = helper.CreateUser ("UserName", "FN", "LN", null, group, tenant);

      var substitution = Substitution.NewObject();
      substitution.SubstitutingUser = user;
      substitution.SubstitutedUser = helper.CreateUser ("SubstitutingUser", "FN", "LN", null, group, tenant);

      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.Tenant, TenantProxy.Create (tenant));
      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.User, UserProxy.Create (user));
      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.Substitution, SubstitutionProxy.Create (substitution));
    }

    [Test]
    public void Refresh ()
    {
      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.Refresh ());
    }

    [Test]
    public void GetTenants ()
    {
      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.GetTenants (true), new TenantProxy[0]);
    }

    [Test]
    public void GetActiveSubstitutions ()
    {
      _lockingTestHelper.ExpectSynchronizedDelegation (mock => mock.GetActiveSubstitutions (), new SubstitutionProxy[0]);
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      _lockingTestHelper.ExpectSynchronizedDelegation (
          mock => mock.GetSecurityPrincipal(),
          MockRepository.GenerateStub<ISecurityPrincipal>());
    }

    [Test]
    public void Serialization ()
    {
      ISecurityManagerPrincipal decorator =
          new LockingSecurityManagerPrincipalDecorator (
              new SecurityManagerPrincipal (
                  new ObjectID (typeof (Tenant), Guid.NewGuid()),
                  new ObjectID (typeof (User), Guid.NewGuid()),
                  null));

      var deserialized = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserialized.IsNull, Is.EqualTo (decorator.IsNull));
    }

    [Test]
    public void Get_IsNull ()
    {
      _innerPrincipalMock.Stub (mock => mock.IsNull).Return (false);

      Assert.That (((INullObject) _decorator).IsNull, Is.False);
    }
  }
}