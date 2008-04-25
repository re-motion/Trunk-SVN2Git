using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithManySideRelationProperties : ClassWithManySideRelationPropertiesNotInMapping
  {
    protected ClassWithManySideRelationProperties ()
    {
    }

    [DBBidirectionalRelation ("NoAttribute")]
    public abstract ClassWithOneSideRelationProperties NoAttribute { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("NotNullable")]
    public abstract ClassWithOneSideRelationProperties NotNullable { get; set; }

    public abstract ClassWithOneSideRelationProperties Unidirectional { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToOne", ContainsForeignKey = true)]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToMany")]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToMany { get; set; }
  }
}