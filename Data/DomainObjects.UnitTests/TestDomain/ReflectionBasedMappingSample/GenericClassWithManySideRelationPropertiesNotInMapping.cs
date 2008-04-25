using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class GenericClassWithManySideRelationPropertiesNotInMapping<T> : DomainObject where T : DomainObject
  {
    protected GenericClassWithManySideRelationPropertiesNotInMapping ()
    {
    }

    public abstract T BaseUnidirectional { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne", ContainsForeignKey = true)]
    public abstract T BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany")]
    public abstract T BaseBidirectionalOneToMany { get; set; }
  }
}