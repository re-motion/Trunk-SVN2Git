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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class OnCommitting : GroupTestBase
  {
    [Test]
    public void OnCommitting_WithCircularParentHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      grandParent.Parent = child;

      try
      {
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.Fail();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("Group '" + grandParent.ID + "' cannot be committed because it would result in a cirucular parent hierarchy."));
      }
    }

    [Test]
    public void OnCommitting_WithCircularParentHierarchy_GroupIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group root = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Root", Guid.NewGuid().ToString(), null, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      root.Parent = root;

      try
      {
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.Fail();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("Group '" + root.ID + "' cannot be committed because it would result in a cirucular parent hierarchy."));
      }
    }

    [Test]
    public void OnCommitting_WithCircularParentHierarchy_ChecksOnlyIfParentHasChanged_ThrowsInvalidOperationException ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), null, tenant);
      grandParent.Parent = child;

      ClientTransactionScope.CurrentTransaction.Commit();

      parent.Name = "NewName";
      child.Parent = parent;

      try
      {
        // Order of DomainObjects is stable and equal to order in which the objects have been added to DataManager
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.Fail();
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message,
            Is.EqualTo ("Group '" + child.ID + "' cannot be committed because it would result in a cirucular parent hierarchy."));
      }
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void OnCommitting_WithCircularParentHierarchy_ChangesHappensInDifferentTransactions_ThrowsConcurrencyViolationException ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), null, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = Group.GetObject (parent.ID);
        parentInOtherThread.Parent = Group.GetObject (grandParent.ID);
        ClientTransaction.Current.Commit();
      }

      Group child = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);
      grandParent.Parent = child;

      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    public void OnCommitting_ReloadsUnchangedParentGroupsDuringOnCommittingToPreventConcurrencyExceptions ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = Group.GetObject (parent.ID);
        parentInOtherThread.Name = "NewName";
        ClientTransaction.Current.Commit();
      }

      _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandChild", Guid.NewGuid().ToString(), child, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void OnCommitting_SimultanousChangesToParentInDifferentTransactions_ThrowsConcurrencyExceptions ()
    {
      Tenant tenant = _testHelper.CreateTenant (ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = Group.GetObject (parent.ID);
        parentInOtherThread.Name = "NewName";
        ClientTransaction.Current.Commit();
      }

      _testHelper.CreateGroup (ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();
    }
  }
}