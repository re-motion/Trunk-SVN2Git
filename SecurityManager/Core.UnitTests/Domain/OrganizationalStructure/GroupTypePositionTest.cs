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