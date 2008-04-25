using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithOneSideRelationPropertiesNotInMapping : DomainObject
  {
    protected ClassWithOneSideRelationPropertiesNotInMapping ()
    {
    }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany", SortExpression = "The Sort Expression")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BaseBidirectionalOneToMany { get; }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToOne")]
    private ClassWithManySideRelationProperties BasePrivateBidirectionalOneToOne
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToMany", SortExpression = "The Sort Expression")]
    private ObjectList<ClassWithManySideRelationProperties> BasePrivateBidirectionalOneToMany
    {
      get { throw new NotImplementedException (); }
    }
  }
}