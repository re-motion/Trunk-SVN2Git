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
  public class GroupTypePositionTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void GetDisplayName_WithGroupTypeAndPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();

      Assert.AreEqual ("GroupTypeName / PositionName", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.Position = null;

      Assert.AreEqual ("GroupTypeName / ", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutGroupType ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;

      Assert.AreEqual (" / PositionName", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutGroupTypeAndWithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;
      groupTypePosition.Position = null;

      Assert.AreEqual (" / ", groupTypePosition.DisplayName);
    }

    private static GroupTypePosition CreateGroupTypePosition ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory ();

      GroupTypePosition groupTypePosition = GroupTypePosition.NewObject();

      groupTypePosition.GroupType = GroupType.NewObject();
      groupTypePosition.GroupType.Name = "GroupTypeName";

      groupTypePosition.Position = factory.CreatePosition ();
      groupTypePosition.Position.Name = "PositionName";

      return groupTypePosition;
    }
  }
}
