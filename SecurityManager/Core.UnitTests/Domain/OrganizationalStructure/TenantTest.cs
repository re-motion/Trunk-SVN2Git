using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Security;
using Remotion.Security.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

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
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");

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
    public void Get_Current_NotInitialized ()
    {
      Assert.IsNull (Tenant.Current);
    }

    [Test]
    public void SetAndGet_Current()
    {
      Tenant tenant = Tenant.GetObject (_expectedTenantID);
      
      Tenant.Current = tenant;
      Assert.AreSame (tenant, Tenant.Current);

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual(tenant.ID, Tenant.Current.ID);
        Assert.AreNotSame (tenant, Tenant.Current);
      }

      Tenant.Current = null;
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      Tenant tenant = Tenant.GetObject (_expectedTenantID);

      Tenant.Current = tenant;
      Assert.AreSame (tenant, Tenant.Current);

      ThreadRunner.Run (
          delegate ()
          {
            Tenant otherTenant = _testHelper.CreateTenant ("OtherTenant", "UID: OtherTenant");

            Assert.IsNull (Tenant.Current);
            Tenant.Current = otherTenant;
            using (_testHelper.Transaction.EnterNonDiscardingScope())
            {
              Assert.AreSame (otherTenant, Tenant.Current);
            }
          });

      Assert.AreSame (tenant, Tenant.Current);
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

      SecurityContext securityContext = ((ISecurityContextFactory) tenant).CreateSecurityContext ();
      Assert.AreEqual (tenant.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.AreEqual (tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void GetHierachy_NoChildren ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");

      ObjectList<Tenant> tenants = root.GetHierachy ();

      Assert.AreEqual (1, tenants.Count);
      Assert.Contains (root, tenants);
    }

    [Test]
    public void GetHierachy_NoGrandChildren ()
    {
      Tenant root = _testHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = _testHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = _testHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;

      ObjectList<Tenant> tenants = root.GetHierachy ();

      Assert.AreEqual (3, tenants.Count);
      Assert.Contains (root, tenants);
      Assert.Contains (child1, tenants);
      Assert.Contains (child2, tenants);
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

      ObjectList<Tenant> tenants = root.GetHierachy ();

      Assert.AreEqual (4, tenants.Count);
      Assert.Contains (root, tenants);
      Assert.Contains (child1, tenants);
      Assert.Contains (child2, tenants);
      Assert.Contains (grandChild1, tenants);
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
