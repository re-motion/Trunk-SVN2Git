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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class GetHierachy : TenantTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Test_NoChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root }));
    }

    [Test]
    public void Test_NoGrandChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root, child1, child2 }));
    }

    [Test]
    public void Test_WithGrandChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root, child1, child2, grandChild1 }));
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnChild ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var child1SecurityContext = ((ISecurityContextFactory) child1).CreateSecurityContext();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var tenants = root.GetHierachy().ToArray();

        Assert.That (tenants, Is.EquivalentTo (new[] { root, child2 }));
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var rootSecurityContext = ((ISecurityContextFactory) root).CreateSecurityContext();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (rootSecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (rootSecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var tenants = root.GetHierachy().ToArray();

        Assert.That (tenants, Is.Empty);
      }
    }
  }
}