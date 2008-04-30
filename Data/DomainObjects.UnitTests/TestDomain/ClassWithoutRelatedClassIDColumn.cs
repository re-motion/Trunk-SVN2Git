using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutRelatedClassIDColumn")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutRelatedClassIDColumn : TestDomainBase
  {
        public static ClassWithoutRelatedClassIDColumn NewObject ()
    {
      return NewObject<ClassWithoutRelatedClassIDColumn> ().With();
    }

    protected ClassWithoutRelatedClassIDColumn()
    {
    }
  
    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumn", ContainsForeignKey = true)]
    public abstract Distributor Distributor { get; set; }
  }
}