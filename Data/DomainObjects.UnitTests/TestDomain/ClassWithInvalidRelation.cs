using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithInvalidRelation")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithInvalidRelation : TestDomainBase
  {
    protected ClassWithInvalidRelation ()
    {
    }

    [DBBidirectionalRelation ("ClassWithInvalidRelation", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public abstract ClassWithGuidKey ClassWithGuidKey { get; set; }
  }
}