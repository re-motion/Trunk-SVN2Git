using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class TestWithTwoTenants : DomainTest
  {
    private DatabaseFixtures _dbFixtures;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      _dbFixtures = new DatabaseFixtures ();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_EmptyResult ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests", ((AbstractRoleDefinition) result[0]).Name);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.FindAll ();

      Assert.AreEqual (2, result.Count);
      for (int i = 0; i < result.Count; i++)
      {
        AbstractRoleDefinition abstractRole = (AbstractRoleDefinition) result[i];
        Assert.AreEqual (i, abstractRole.Index, "Wrong Index.");
      }
    }
  }
}
