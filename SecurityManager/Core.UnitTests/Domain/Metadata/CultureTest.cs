using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitSecurableClassDefinitionWithLocalizedNames (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_Existing ()
    {
      Culture foundCulture = Culture.Find ("de");

      Assert.IsNotNull (foundCulture);
      Assert.AreNotEqual (StateType.New, foundCulture.State);
      Assert.AreEqual ("de", foundCulture.CultureName);
    }

    [Test]
    public void Find_NotExisting ()
    {
      Culture foundCulture = Culture.Find ("hu");

      Assert.IsNull (foundCulture);
    }
  }
}
