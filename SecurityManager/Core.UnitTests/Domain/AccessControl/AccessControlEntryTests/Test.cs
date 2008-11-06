/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetAllowedAccessTypes(), Is.EquivalentTo (new[] { readAccessType }));
    }

    [Test]
    public void GetDeniedAccessTypes_DeleteDenied ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetDeniedAccessTypes(), Is.EquivalentTo (new[] { deleteAccessType }));
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    public void DenyAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.DenyAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That (allowedAccessTypes, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType")]
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
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);

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
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (10, ClientTransaction.CreateRootTransaction());

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
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (0, ClientTransaction.CreateRootTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.TenantSelection = TenantSelection.OwningTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");

      Assert.IsNotNull (ace.SpecificTenant);
      ClientTransactionScope.CurrentTransaction.Commit();
      Assert.IsNull (ace.SpecificTenant);
    }

    [Test]
    public void ClearSpecificTenantOnCommitWhenObjectIsDeleted ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (0, ClientTransaction.CreateRootTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.TenantSelection = TenantSelection.SpecificTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");
      ClientTransactionScope.CurrentTransaction.Commit();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        AccessControlEntry aceActual = AccessControlEntry.GetObject (aceID);
        aceActual.TenantSelection = TenantSelection.OwningTenant;

        Assert.IsNotNull (aceActual.SpecificTenant);
        aceActual.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }
  }
}