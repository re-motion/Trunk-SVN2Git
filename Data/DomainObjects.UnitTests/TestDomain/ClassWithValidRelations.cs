using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithValidRelations")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithValidRelations : TestDomainBase
  {
    public static ClassWithValidRelations NewObject ()
    {
      return NewObject<ClassWithValidRelations> ().With();
    }

    protected ClassWithValidRelations()
    {
    }

    [DBBidirectionalRelation ("ClassWithValidRelationsOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyOptionalID")]
    public abstract ClassWithGuidKey ClassWithGuidKeyOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithValidRelationsNonOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyNonOptionalID")]
    [Mandatory]
    public abstract ClassWithGuidKey ClassWithGuidKeyNonOptional { get; set; }
  }
}