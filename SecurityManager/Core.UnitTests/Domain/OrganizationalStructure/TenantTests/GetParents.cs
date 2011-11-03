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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class GetParents : TenantTestBase
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
    public void Test_NoParents ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");

      var groups = root.GetParents().ToArray();

      Assert.That (groups, Is.Empty);
    }

    [Test]
    public void Test_NoGrandParents ()
    {
      Tenant parent = TestHelper.CreateTenant ("parent1", "UID: parent");
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      root.Parent = parent;

      var groups = root.GetParents().ToArray();

      Assert.That (groups, Is.EquivalentTo (new[] { parent }));
    }

    [Test]
    public void Test_WithGrandParents ()
    {
      Tenant grandParent = TestHelper.CreateTenant ("Grandparent1", "UID: Grandparent");
      Tenant parent = TestHelper.CreateTenant ("parent1", "UID: parent");
      parent.Parent = grandParent;
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      root.Parent = parent;

      var groups = root.GetParents().ToArray();

      Assert.That (groups, Is.EquivalentTo (new[] { parent, grandParent }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant grandParent2 = TestHelper.CreateTenant ("Grandparent2", "UID: Grandparent2");
      Tenant grandParent1 = TestHelper.CreateTenant ("Grandparent1", "UID: Grandparent1");
      grandParent1.Parent = grandParent2;
      Tenant parent = TestHelper.CreateTenant ("parent1", "UID: parent");
      parent.Parent = grandParent1;
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      root.Parent = parent;
      grandParent2.Parent = root;

      try
      {
        grandParent1.GetParents().ToArray();
        Assert.Fail();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("The parent hierarchy for group '" + grandParent1.ID + "' cannot be resolved because a circular reference exists."));
      }
    }

    [Test]
    public void Test_WithCircularHierarchy_GroupIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      root.Parent = root;

      try
      {
        root.GetParents ().ToArray ();
        Assert.Fail ();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("The parent hierarchy for group '" + root.ID + "' cannot be resolved because a circular reference exists."));
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnParent ()
    {
      Tenant grandParent2 = TestHelper.CreateTenant ("Grandparent1", "UID: Grandparent2");
      Tenant grandParent1 = TestHelper.CreateTenant ("Grandparent1", "UID: Grandparent1");
      grandParent1.Parent = grandParent2;
      Tenant parent = TestHelper.CreateTenant ("parent1", "UID: parent");
      parent.Parent = grandParent1;
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      root.Parent = parent;

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var grandParent1SecurityContext = ((ISecurityContextFactory) grandParent1).CreateSecurityContext();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (grandParent1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (grandParent1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var groups = root.GetParents().ToArray();

        Assert.That (groups, Is.EquivalentTo (new[] { parent }));
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      securityProviderStub.Stub (
          stub => stub.GetAccess (Arg<SecurityContext>.Is.Anything, Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (root.GetParents(), Is.Empty);
      }
    }
  }
}