using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithOneSideRelationProperties : ClassWithOneSideRelationPropertiesNotInMapping
  {
    protected ClassWithOneSideRelationProperties ()
    {
    }

    [DBBidirectionalRelation ("NoAttribute")]
    public abstract ObjectList<ClassWithManySideRelationProperties> NoAttribute { get; set; }

    [DBBidirectionalRelation ("NotNullable")]
    [Mandatory]
    public abstract ObjectList<ClassWithManySideRelationProperties> NotNullable { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToMany", SortExpression = "The Sort Expression")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BidirectionalOneToMany { get; }
  }
}