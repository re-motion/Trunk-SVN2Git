// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Security;
using Remotion.Data.DomainObjects.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTest : DomainTest
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

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }
    
    [Test]
    public void FindByUnqiueIdentifier_ValidGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: testGroup");

      Assert.AreEqual ("UID: testGroup", foundGroup.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: NotExistingGroup");

      Assert.IsNull (foundGroup);
    }

    [Test]
    public void Find_GroupsByTenantID ()
    {
      DomainObjectCollection groups = Group.FindByTenantID (_expectedTenantID);

      Assert.AreEqual (9, groups.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "NewTenant2", "UID: testTenant");
      _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "NewGroup2", "UID: testGroup", null, tenant);

      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject group = CreateGroup ();

      IObjectSecurityStrategy objectSecurityStrategy = group.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject group = CreateGroup ();

      Assert.AreSame (group.GetSecurityStrategy (), group.GetSecurityStrategy ());
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject group = CreateGroup ();

      Assert.AreSame (typeof (Group), group.GetSecurableType ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Group group = CreateGroup ();
      IDomainObjectSecurityContextFactory factory = group;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      group.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Group group = CreateGroup ();

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (group.Tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsFalse (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoTenant ()
    {
      Group group = CreateGroup ();
      group.Tenant = null;

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsNull (securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsFalse (securityContext.IsStateless);
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory();
      Group group = factory.CreateGroup ();

      Assert.IsNotEmpty (group.UniqueIdentifier);
    }

    [Test]
    public void GetDisplayName_WithShortName ()
    {
      Group group = CreateGroup ();
      group.Name = "LongGroupName";
      group.ShortName = "ShortName";

      Assert.AreEqual ("ShortName (LongGroupName)", group.DisplayName);
    }

    [Test]
    public void GetDisplayName_NoShortName ()
    {
      Group group = CreateGroup ();
      group.Name = "LongGroupName";
      group.ShortName = null;

      Assert.AreEqual ("LongGroupName", group.DisplayName);
    }

    #region IBusinessObjectWithIdentifier.UniqueIdentifier tests

    [Test]
    public void SetAndGet_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
    }

    [Test]
    public void SetAndGet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      group.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateTenant (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions ();

      bool isFound = false;
      foreach (PropertyBase property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == typeof (Group))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue (isFound, "Property UnqiueIdentifier declared on Group was not found.");
    }

    #endregion

    //TODO: Refactor to mock
    [Test]
    public void SearchParentGroups ()
    {
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (GroupPropertiesSearchService), new GroupPropertiesSearchService ());
      IBusinessObjectClass groupClass = BindableObjectProvider.GetBindableObjectClass (typeof (Group));
      IBusinessObjectReferenceProperty parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      Assert.That (parentGroupProperty, Is.Not.Null);

      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      List<Group> expectedParentGroups = group.GetPossibleParentGroups (group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      Assert.That (parentGroupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actualParentGroups = parentGroupProperty.SearchAvailableObjects (group, null);
      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }

    [Test]
    public void DeleteGroup_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        var group = testHelper.CreateGroup("group", null, testHelper.CreateTenant ("tenant"));
        var ace = testHelper.CreateAceWithSpecificGroup(group);

        group.Delete ();

        Assert.IsTrue (ace.IsDiscarded);
      }
    }

    [Test]
    public void DeleteGroup_WithRole ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        Tenant tenant = testHelper.CreateTenant ("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid ().ToString (), null, tenant);
        Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, tenant);
        User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition ("Position");
        Role role = testHelper.CreateRole (user, roleGroup, position);

        roleGroup.Delete ();

        Assert.IsTrue (role.IsDiscarded);
      }
    }

    private Group CreateGroup ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: TestGroup", null, tenant);

      return group;
    }
  }
}
