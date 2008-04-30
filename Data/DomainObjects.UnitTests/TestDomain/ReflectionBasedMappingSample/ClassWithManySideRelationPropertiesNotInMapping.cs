using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithManySideRelationPropertiesNotInMapping : DomainObject
  {
    protected ClassWithManySideRelationPropertiesNotInMapping ()
    {
    }

    public abstract ClassWithOneSideRelationProperties BaseUnidirectional { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne", ContainsForeignKey = true)]
    public abstract ClassWithOneSideRelationProperties BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany")]
    public abstract ClassWithOneSideRelationProperties BaseBidirectionalOneToMany { get; set; }

    private ClassWithOneSideRelationProperties BasePrivateUnidirectional
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToOne", ContainsForeignKey = true)]
    private ClassWithOneSideRelationProperties BasePrivateBidirectionalOneToOne
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToMany")]
    private ClassWithOneSideRelationProperties BasePrivateBidirectionalOneToMany
    {
      get { throw new NotImplementedException (); }
      set { throw new NotImplementedException (); }
    }
  }
}