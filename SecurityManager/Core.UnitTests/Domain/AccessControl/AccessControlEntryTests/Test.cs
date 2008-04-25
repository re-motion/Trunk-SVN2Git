using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      _testHelper.CreateWriteAccessType (ace, null);
      _testHelper.CreateDeleteAccessType (ace, null);

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
                                                    "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType"
        )]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AllowAccess (accessType);
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, true);

      ace.RemoveAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (0, allowedAccessTypes.Length);
    }

    [Test]
    public void AttachAccessType_TwoNewAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType0 = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Access Type 0", 0);
      AccessTypeDefinition accessType1 = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Access Type 1", 1);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, ace.State);

        ace.AttachAccessType (accessType0);
        ace.AttachAccessType (accessType1);

        Assert.AreEqual (2, ace.Permissions.Count);
        Permission permission0 = ace.Permissions[0];
        Assert.AreSame (accessType0, permission0.AccessType);
        Assert.AreEqual (0, permission0.Index);
        Permission permission1 = ace.Permissions[1];
        Assert.AreSame (accessType1, permission1.AccessType);
        Assert.AreEqual (1, permission1.Index);
        Assert.AreEqual (StateType.Changed, ace.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
                                                    "The access type 'Test' has already been attached to this access control entry.\r\nParameter name: accessType"
        )]
    public void AttachAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AttachAccessType (accessType);
      ace.AttachAccessType (accessType);
    }

    [Test]
    public void Get_PermissionsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (10, ClientTransaction.NewRootTransaction());

      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);

      Assert.AreEqual (10, ace.Permissions.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (string.Format ("Access Type {0}", i), (ace.Permissions[i]).AccessType.Name);
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreEqual (StateType.New, ace.State);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreEqual (StateType.New, ace.State);

      ace.Touch();

      Assert.AreEqual (StateType.New, ace.State);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ace.Index = 1;
      Assert.AreEqual (1, ace.Index);
    }

    [Test]
    public void ClearSpecificTenantOnCommit ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (0, ClientTransaction.NewRootTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.Tenant = TenantSelection.OwningTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");

      Assert.IsNotNull (ace.SpecificTenant);
      ClientTransactionScope.CurrentTransaction.Commit();
      Assert.IsNull (ace.SpecificTenant);
    }

    [Test]
    public void ClearSpecificTenantOnCommitWhenObjectIsDeleted ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (0, ClientTransaction.NewRootTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.Tenant = TenantSelection.SpecificTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");
      ClientTransactionScope.CurrentTransaction.Commit();
      using (ClientTransaction.NewRootTransaction().EnterNonDiscardingScope())
      {
        AccessControlEntry aceActual = AccessControlEntry.GetObject (aceID);
        aceActual.Tenant = TenantSelection.OwningTenant;

        Assert.IsNotNull (aceActual.SpecificTenant);
        aceActual.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }
  }
}