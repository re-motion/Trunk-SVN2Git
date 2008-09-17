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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypeTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());

      DomainObjectCollection groupTypes = GroupType.FindAll ();

      Assert.AreEqual (2, groupTypes.Count);
    }

    [Test]
    public void GetDisplayName ()
    {
      GroupType groupType = GroupType.NewObject();
      groupType.Name = "GroupTypeName";

      Assert.AreEqual ("GroupTypeName", groupType.DisplayName);
    }
  }
}
