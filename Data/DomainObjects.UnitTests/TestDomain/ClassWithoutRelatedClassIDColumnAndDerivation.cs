using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutRelatedClassIDColumnAndDerivation")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutRelatedClassIDColumnAndDerivation : TestDomainBase
  {
    public static ClassWithoutRelatedClassIDColumnAndDerivation NewObject ()
    {
      return NewObject<ClassWithoutRelatedClassIDColumnAndDerivation> ().With();
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation()
    {
    }

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumnAndDerivation", ContainsForeignKey = true)]
    public abstract Company Company { get; set; }
  }
}
