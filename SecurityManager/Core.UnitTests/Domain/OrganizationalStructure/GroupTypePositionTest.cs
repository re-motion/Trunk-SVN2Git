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
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
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
