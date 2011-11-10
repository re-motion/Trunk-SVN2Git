// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class GetHierarchy : GroupTestBase
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
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That (groups, Is.EquivalentTo (new[] { root }));
    }

    [Test]
    public void Test_NoGrandChildren ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup ("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup ("Child2", "UID: Child2", root, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That (groups, Is.EquivalentTo (new[] { root, child1, child2 }));
    }

    [Test]
    public void Test_WithGrandChildren ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup ("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup ("Child2", "UID: Child2", root, tenant);
      Group grandChild1 = TestHelper.CreateGroup ("GrandChild1", "UID: GrandChild1", child1, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That (groups, Is.EquivalentTo (new[] { root, child1, child2, grandChild1 }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup ("Child1", "UID: Child1", root, tenant);
      Group grandChild1 = TestHelper.CreateGroup ("GrandChild1", "UID: GrandChild1", child1, tenant);
      Group grandChild2 = TestHelper.CreateGroup ("GrandChild2", "UID: GrandChild2", grandChild1, tenant);
      root.Parent = grandChild2;

      try
      {
        grandChild1.GetHierachy().ToArray();
        Assert.Fail();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("The hierarchy for group '" + grandChild1.ID + "' cannot be resolved because a circular reference exists."));
      }
    }

    [Test]
    public void Test_WithCircularHierarchy_GroupIsOwnChild_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);
      root.Parent = root;

      try
      {
        root.GetHierachy ().ToArray ();
        Assert.Fail ();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("The hierarchy for group '" + root.ID + "' cannot be resolved because a circular reference exists."));
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnChild ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup ("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup ("Child2", "UID: Child2", root, tenant);
      TestHelper.CreateGroup ("GrandChild1", "UID: GrandChild1", child1, tenant);

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var child1SecurityContext = ((ISecurityContextFactory) child1).CreateSecurityContext ();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var groups = root.GetHierachy ().ToArray ();

        Assert.That (groups, Is.EquivalentTo (new[] { root, child2 }));
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup ("Root", "UID: Root", null, tenant);

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      securityProviderStub.Stub (
          stub => stub.GetAccess (Arg<SecurityContext>.Is.Anything, Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (root.GetHierachy(), Is.Empty);
      }
    }
  }
}