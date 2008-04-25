using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
     DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      DomainObjectCollection result = AbstractRoleDefinition.FindAll ();

      Assert.AreEqual (0, result.Count);
    }
  }
}
