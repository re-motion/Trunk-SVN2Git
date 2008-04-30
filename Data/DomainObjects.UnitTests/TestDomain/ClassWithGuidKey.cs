using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithGuidKey")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithGuidKey : TestDomainBase
  {
    public static ClassWithGuidKey NewObject ()
    {
      return NewObject<ClassWithGuidKey> ().With();
    }

    protected ClassWithGuidKey()
    {
    }

    [DBBidirectionalRelation ("ClassWithGuidKeyOptional")]
    public abstract ClassWithValidRelations ClassWithValidRelationsOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKeyNonOptional")]
    [Mandatory]
    public abstract ClassWithValidRelations ClassWithValidRelationsNonOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithInvalidRelation ClassWithInvalidRelation { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithRelatedClassIDColumnAndNoInheritance ClassWithRelatedClassIDColumnAndNoInheritance { get; set; }
  }
}