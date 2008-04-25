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

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());

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
