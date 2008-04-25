using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class GenericClassWithOneSideRelationPropertiesNotInMapping<T> : DomainObject where T: DomainObject
  {
    protected GenericClassWithOneSideRelationPropertiesNotInMapping ()
    {
    }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne")]
    public abstract T BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany", SortExpression = "The Sort Expression")]
    public abstract ObjectList<T> BaseBidirectionalOneToMany { get; }
  }
}