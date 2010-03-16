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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Security;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class TenantTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ObjectID _expectedTenantID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      _dbFixtures = new DatabaseFixtures ();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantID = tenant.ID;
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    public override void TearDown ()
    {
      base.TearDown ();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void FindAll ()
    {
      DomainObjectCollection tenants = Tenant.FindAll ();

      Assert.AreEqual (2, tenants.Count);
      Assert.AreEqual (_expectedTenantID, tenants[1].ID);
    }

    [Test]
    public void FindByUnqiueIdentifier_ValidTenant ()
    {
      Tenant foundTenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");

      Assert.AreEqual ("UID: testTenant", foundTenant.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingTenant ()
    {
      Tenant foundTenant = Tenant.FindByUnqiueIdentifier ("UID: NotExistingTenant");

      Assert.IsNull (foundTenant);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameidentifierTwice ()
    {
      _testHelper.CreateTenant ("TestTenant", "UID: testTenant");

      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");

      Assert.IsNotEmpty (tenant.UniqueIdentifier);
    }

    [Test]
    public void GetDisplayName ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Tenantname", "UID");

      Assert.AreEqual ("Tenantname", tenant.DisplayName);
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject tenant = _testHelper.CreateTenant ("Tenant", "UID: Tenant");

      IObjectSecurityStrategy objectSecurityStrategy = tenant.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject tenant = _testHelper.CreateTenant ("Tenant", "UID: Tenant");

      Assert.AreSame (tenant.GetSecurityStrategy (), tenant.GetSecurityStrategy ());
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject tenant = _testHelper.CreateTenant ("Tenant", "UID: Tenant");

      Assert.AreSame (typeof (Tenant), tenant.GetSecurableType ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Tenant", "UID: Tenant");
      IDomainObjectSecurityContextFactory factory = tenant;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      tenant.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Tenant", "UID: Tenant");

      ISecurityContext securityContext = ((ISecurityContextFactory) tenant).CreateSecurityContext ();
      Assert.AreEqual (tenant.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.IsNull (securityContext.OwnerGroup);
      Assert.AreEqual (tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsFalse (securityContext.IsStateless);
    }

    [Test]
    public void GetHierachy_NoChildren ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root }));
    }

    [Test]
    public void GetHierachy_NoGrandChildren ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = _testHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = _testHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root, child1, child2 }));
    }

    [Test]
    public void GetHierachy_WithGrandChildren ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = _testHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = _testHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;
      Tenant grandChild1 = _testHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      var tenants = root.GetHierachy().ToArray();

      Assert.That (tenants, Is.EquivalentTo (new[] { root, child1, child2, grandChild1 }));
    }

    [Test]
    public void GetHierachy_WithSecurity_PermissionDeniedOnChild ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = _testHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = _testHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;
      Tenant grandChild1 = _testHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
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
    public void GetHierachy_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = _testHelper.CreateTenant ("Child1", "UID: Child1");
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

    #region IBusinessObjectWithIdentifier.UniqueIdentifier tests

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", tenant.UniqueIdentifier);
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (tenant.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (tenant.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", tenant.UniqueIdentifier);
      Assert.AreEqual (tenant.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;
      tenant.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions ();

      bool isFound = false;
      foreach (PropertyBase property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == typeof (Tenant))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue (isFound, "Property UnqiueIdentifier declared on Tenant was not found.");
    }

    #endregion
  }
}
