using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class RevisionTest : DomainTest
  {
    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (0, Revision.GetRevision ());
      }
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Revision.IncrementRevision ();
      }

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (1, Revision.GetRevision ());
      }
    }
  }
}