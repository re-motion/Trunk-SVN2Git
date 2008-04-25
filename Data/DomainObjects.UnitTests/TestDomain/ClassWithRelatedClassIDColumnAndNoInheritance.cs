using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithRelatedClassIDColumnAndNoInheritance")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithRelatedClassIDColumnAndNoInheritance : TestDomainBase
  {
    public static ClassWithRelatedClassIDColumnAndNoInheritance NewObject ()
    {
      return NewObject<ClassWithRelatedClassIDColumnAndNoInheritance> ().With();
    }

    protected ClassWithRelatedClassIDColumnAndNoInheritance()
    {
    }

    [DBBidirectionalRelation ("ClassWithRelatedClassIDColumnAndNoInheritance", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public abstract ClassWithGuidKey ClassWithGuidKey { get; set; }
  }
}
