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
using Remotion.Security.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

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
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
      _expectedTenantID = tenant.ID;
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
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
      Assert.IsEmpty (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (group.Tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoTenant ()
    {
      Group group = CreateGroup ();
      group.Tenant = null;

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
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

    [Test]
    public void SearchParentGroups ()
    {
      BindableObjectProvider.Current.AddService (typeof (GroupPropertiesSearchService), new GroupPropertiesSearchService());
      IBusinessObjectClass groupClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (Group));
      IBusinessObjectReferenceProperty parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      Assert.That (parentGroupProperty, Is.Not.Null);

      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      List<Group> expectedParentGroups = group.GetPossibleParentGroups (group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      Assert.That (parentGroupProperty.SupportsSearchAvailableObjects (true), Is.True);

      IBusinessObject[] actualParentGroups = parentGroupProperty.SearchAvailableObjects (group, true, null);
      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }

    private Group CreateGroup ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: TestGroup", null, tenant);

      return group;
    }
  }
}
