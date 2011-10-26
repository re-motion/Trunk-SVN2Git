// This file is part of re-strict (www.re-motion.org)
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
    private OrganizationalStructureTestHelper _helper;

    public override void SetUp ()
    {
      base.SetUp();

      _helper = new OrganizationalStructureTestHelper();
      _helper.Transaction.EnterNonDiscardingScope();
    }


    [Test]
    public void Get_Members ()
    {
      var tenant = _helper.CreateTenant ("TheTenant", "Tenant UID");
      var group = _helper.CreateGroup ("TheGroup", "Group UID", null, tenant);
      var user = _helper.CreateUser ("UserName", "FN", "LN", null, group, tenant);

      var substitution = Substitution.NewObject();
      substitution.SubstitutingUser = user;
      substitution.SubstitutedUser = _helper.CreateUser ("SubstitutingUser", "FN", "LN", null, group, tenant);

      var tenantProxy = TenantProxy.Create (tenant);
      var userProxy = UserProxy.Create (user);
      var substitutionProxy = SubstitutionProxy.Create (substitution);

      var innerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      innerPrincipalStub.Stub (stub => stub.Tenant).Return (tenantProxy);
      innerPrincipalStub.Stub (stub => stub.User).Return (userProxy);
      innerPrincipalStub.Stub (stub => stub.Substitution).Return (substitutionProxy);

      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalStub);

      Assert.That (decorator.Tenant, Is.SameAs (tenantProxy));
      Assert.That (decorator.User, Is.SameAs (userProxy));
      Assert.That (decorator.Substitution, Is.SameAs (substitutionProxy));
    }

    [Test]
    public void Refresh ()
    {
      var innerPrincipalMock = MockRepository.GenerateMock<ISecurityManagerPrincipal>();
      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalMock);

      decorator.Refresh();

      innerPrincipalMock.AssertWasCalled (mock => mock.Refresh());
    }

    [Test]
    public void GetTenants ()
    {
      var innerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      var tenantProxies = new TenantProxy[0];
      innerPrincipalStub.Stub (stub => stub.GetTenants (true)).Return (tenantProxies);

      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalStub);

      Assert.That (decorator.GetTenants (true), Is.SameAs (tenantProxies));
    }

    [Test]
    public void GetActiveSubstitutions ()
    {
      var innerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      var substitutionProxies = new SubstitutionProxy[0];
      innerPrincipalStub.Stub (stub => stub.GetActiveSubstitutions()).Return (substitutionProxies);

      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalStub);

      Assert.That (decorator.GetActiveSubstitutions(), Is.SameAs (substitutionProxies));
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      var innerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      var securityPrincipal = MockRepository.GenerateStub<ISecurityPrincipal>();
      innerPrincipalStub.Stub (stub => stub.GetSecurityPrincipal()).Return (securityPrincipal);

      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalStub);

      Assert.That (decorator.GetSecurityPrincipal(), Is.SameAs (securityPrincipal));
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
      var innerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      innerPrincipalStub.Stub (stub => stub.IsNull).Return (false);

      ISecurityManagerPrincipal decorator = new LockingSecurityManagerPrincipalDecorator (innerPrincipalStub);
      Assert.That (decorator.IsNull, Is.False);
    }
  }
}