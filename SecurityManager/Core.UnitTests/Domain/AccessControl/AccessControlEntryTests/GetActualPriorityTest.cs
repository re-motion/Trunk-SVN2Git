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

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class GetActualPriorityTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void CustomPriority ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.Priority = 42;

      Assert.AreEqual (42, ace.ActualPriority);
    }

    [Test]
    public void EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      Assert.AreEqual (0, ace.ActualPriority);
    }

    [Test]
    public void AceWithAbstractRole ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.SpecificAbstractRole = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      Assert.AreEqual (AccessControlEntry.AbstractRolePriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUser ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.UserSelection = UserSelection.Owner;

      Assert.AreEqual (AccessControlEntry.UserPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithGroup ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.GroupSelection = GroupSelection.OwningGroup;

      Assert.AreEqual (AccessControlEntry.GroupPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithTenant ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.TenantSelection = TenantSelection.OwningTenant;

      Assert.AreEqual (AccessControlEntry.TenantPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUserAndGroup ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.UserSelection = UserSelection.Owner;
      ace.GroupSelection = GroupSelection.OwningGroup;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.GroupPriority;
      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUserAndAbstractRoleAndGroupAndTenant ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      ace.UserSelection = UserSelection.Owner;
      ace.SpecificAbstractRole = AbstractRoleDefinition.NewObject (Guid.NewGuid(), "Test", 42);
      ace.GroupSelection = GroupSelection.OwningGroup;
      ace.TenantSelection = TenantSelection.OwningTenant;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.AbstractRolePriority + AccessControlEntry.GroupPriority
                             + AccessControlEntry.TenantPriority;

      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }
  }
}
