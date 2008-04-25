using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class AccessTypeReferenceTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessTypeReference accessTypeReference = AccessTypeReference.NewObject();

      accessTypeReference.Index = 1;
      Assert.AreEqual (1, accessTypeReference.Index);
    }
  }
}