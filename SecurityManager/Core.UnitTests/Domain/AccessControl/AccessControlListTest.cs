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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlListTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void FindMatchingEntries_WithMatchingAce ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      AccessControlList acl = _testHelper.CreateAcl (entry);
      SecurityToken token = _testHelper.CreateEmptyToken();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries (token);

      Assert.AreEqual (1, foundEntries.Length);
      Assert.Contains (entry, foundEntries);
    }

    [Test]
    public void FindMatchingEntries_WithoutMatchingAce ()
    {
      AccessControlList acl = _testHelper.CreateAcl (_testHelper.CreateAceWithAbstractRole());
      SecurityToken token = _testHelper.CreateEmptyToken();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries (token);

      Assert.AreEqual (0, foundEntries.Length);
    }

    [Test]
    public void FindMatchingEntries_WithMultipleMatchingAces ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType (ace2, readAccessType, null);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role2);

      AccessControlEntry[] entries = acl.FindMatchingEntries (token);

      Assert.AreEqual (2, entries.Length);
      Assert.Contains (ace2, entries);
      Assert.Contains (ace1, entries);
    }

    [Test]
    public void GetAccessTypes_WithMatchingAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      _testHelper.CreateWriteAccessType (ace, null);
      _testHelper.CreateDeleteAccessType (ace, null);
      AccessControlList acl = _testHelper.CreateAcl (ace);
      SecurityToken token = _testHelper.CreateEmptyToken();

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
    }

    [Test]
    public void GetAccessTypes_WithoutMatchingAce ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithAbstractRole();
      _testHelper.CreateReadAccessType (ace, true);
      _testHelper.CreateWriteAccessType (ace, null);
      _testHelper.CreateDeleteAccessType (ace, null);
      AccessControlList acl = _testHelper.CreateAcl (ace);
      SecurityToken token = _testHelper.CreateEmptyToken();

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAccessTypes_WithMultipleMatchingAces ()
    {
      AbstractRoleDefinition role1 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "QualityManager", 0);
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      ace1.SpecificAbstractRole = role1;
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType (ace2, readAccessType, true);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role1, role2);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (2, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
      Assert.Contains (writeAccessType, accessTypes);
    }

    [Test]
    public void GetAccessTypes_WithMultipleMatchingAcesButDifferentPriorities ()
    {
      AbstractRoleDefinition role1 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "QualityManager", 0);
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      ace1.SpecificAbstractRole = role1;
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.SpecificAbstractRole = role2;
      ace2.Priority = 42;
      _testHelper.AttachAccessType (ace2, readAccessType, null);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role1, role2);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (writeAccessType, accessTypes);
    }

    [Test]
    public void FilterAcesByPriority_Empty ()
    {
      AccessControlList acl = _testHelper.CreateAcl();
      AccessControlEntry[] aces = new AccessControlEntry[0];

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (0, entries.Length);
    }

    [Test]
    public void FilterAcesByPriority_OnlyOne ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();

      AccessControlList acl = _testHelper.CreateAcl (ace1);
      AccessControlEntry[] aces = new AccessControlEntry[] {ace1};

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (1, entries.Length);
      Assert.Contains (ace1, entries);
    }

    [Test]
    public void FilterAcesByPriority_Multiple ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      ace1.Priority = 24;
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.Priority = 42;

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      AccessControlEntry[] aces = new AccessControlEntry[] {ace1, ace2};

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (1, entries.Length);
      Assert.Contains (ace2, entries);
    }

    [Test]
    public void FilterAcesByPriority_MultipleAndMultipleFiltered ()
    {
      AccessControlEntry ace1 = AccessControlEntry.NewObject();
      ace1.Priority = 24;
      AccessControlEntry ace2 = AccessControlEntry.NewObject();
      ace2.Priority = 42;
      AccessControlEntry ace3 = AccessControlEntry.NewObject();
      ace3.Priority = 42;

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2, ace3);
      AccessControlEntry[] aces = new AccessControlEntry[] {ace1, ace2, ace3};

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (2, entries.Length);
      Assert.Contains (ace2, entries);
      Assert.Contains (ace3, entries);
    }

    [Test]
    public void CreateStateCombination ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, classDefinition.State);
        Assert.AreEqual (StateType.Unchanged, acl.State);

        StateCombination stateCombination = acl.CreateStateCombination();

        Assert.AreSame (acl, stateCombination.AccessControlList);
        Assert.AreEqual (acl.Class, stateCombination.Class);
        Assert.IsEmpty (stateCombination.StateUsages);
        Assert.AreEqual (StateType.Changed, classDefinition.State);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateStateCombination_WithoutClassDefinition ()
    {
      AccessControlList acl = _testHelper.CreateAcl ((SecurableClassDefinition) null);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        acl.StateCombinations.Add (StateCombination.NewObject());

        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateStateCombination_TwoNewEntries ()
    {
      AccessControlList acl = AccessControlList.NewObject();
      acl.Class = _testHelper.CreateClassDefinition ("SecurableClass");
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        StateCombination stateCombination0 = acl.CreateStateCombination();
        StateCombination stateCombination1 = acl.CreateStateCombination();

        Assert.AreEqual (2, acl.StateCombinations.Count);
        Assert.AreSame (stateCombination0, acl.StateCombinations[0]);
        Assert.AreEqual (0, stateCombination0.Index);
        Assert.AreSame (stateCombination1, acl.StateCombinations[1]);
        Assert.AreEqual (1, stateCombination1.Index);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateAccessControlEntry ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      AccessTypeDefinition readAccessType = _testHelper.AttachAccessType (classDefinition, Guid.NewGuid(), "Read", 0);
      AccessTypeDefinition deleteAccessType = _testHelper.AttachAccessType (classDefinition, Guid.NewGuid(), "Delete", 1);
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        AccessControlEntry entry = acl.CreateAccessControlEntry();

        Assert.AreSame (acl, entry.AccessControlList);
        Assert.AreEqual (2, entry.Permissions.Count);
        Assert.AreSame (readAccessType, (entry.Permissions[0]).AccessType);
        Assert.AreSame (entry, (entry.Permissions[0]).AccessControlEntry);
        Assert.AreSame (deleteAccessType, (entry.Permissions[1]).AccessType);
        Assert.AreSame (entry, (entry.Permissions[1]).AccessControlEntry);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateAccessControlEntry_TwoNewEntries ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        AccessControlEntry ace0 = acl.CreateAccessControlEntry();
        AccessControlEntry acel = acl.CreateAccessControlEntry();

        Assert.AreEqual (2, acl.AccessControlEntries.Count);
        Assert.AreSame (ace0, acl.AccessControlEntries[0]);
        Assert.AreEqual (0, ace0.Index);
        Assert.AreSame (acel, acl.AccessControlEntries[1]);
        Assert.AreEqual (1, acel.Index);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlList acl = _testHelper.CreateAcl (_testHelper.CreateOrderClassDefinitionWithProperties());

      Assert.AreEqual (StateType.New, acl.State);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      AccessControlList acl = _testHelper.CreateAcl (_testHelper.CreateOrderClassDefinitionWithProperties());

      Assert.AreEqual (StateType.New, acl.State);
      acl.Touch();

      Assert.AreEqual (StateType.New, acl.State);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlList acl = AccessControlList.NewObject();

      acl.Index = 1;
      Assert.AreEqual (1, acl.Index);
    }

    [Test]
    public void Get_AccessControlEntriesFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlList expectedAcl =
          dbFixtures.CreateAndCommitAccessControlListWithAccessControlEntries (10, ClientTransactionScope.CurrentTransaction);
      ObjectList<AccessControlEntry> expectedAces = expectedAcl.AccessControlEntries;

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        AccessControlList actualAcl = AccessControlList.GetObject (expectedAcl.ID);

        Assert.AreEqual (10, actualAcl.AccessControlEntries.Count);
        for (int i = 0; i < 10; i++)
          Assert.AreEqual (expectedAces[i].ID, actualAcl.AccessControlEntries[i].ID);
      }
    }

    [Test]
    public void Get_StateCombinationsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlList expectedAcl = dbFixtures.CreateAndCommitAccessControlListWithStateCombinations (10, ClientTransactionScope.CurrentTransaction);
      ObjectList<StateCombination> expectedStateCombinations = expectedAcl.StateCombinations;

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        AccessControlList actualAcl = AccessControlList.GetObject (expectedAcl.ID);

        Assert.AreEqual (10, actualAcl.StateCombinations.Count);
        for (int i = 0; i < 10; i++)
          Assert.AreEqual (expectedStateCombinations[i].ID, actualAcl.StateCombinations[i].ID);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot create StateCombination if no SecurableClassDefinition is assigned to this AccessControlList.")]
    public void CreateStateCombination_BeforeClassIsSet ()
    {
      AccessControlList acl = AccessControlList.NewObject();
      acl.CreateStateCombination();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList.")]
    public void CreateAccessControlEntry_BeforeClassIsSet ()
    {
      AccessControlList acl = AccessControlList.NewObject();
      acl.CreateAccessControlEntry();
    }
  }
}
