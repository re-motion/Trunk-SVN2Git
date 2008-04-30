using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithMixedPropertiesNotInMapping : DomainObject
  {
    protected ClassWithMixedPropertiesNotInMapping ()
    {
    }

    public abstract string BaseString { get; set; }

    public abstract ClassWithOneSideRelationProperties BaseUnidirectionalOneToOne { get; set; }

    private ClassWithOneSideRelationProperties BasePrivateUnidirectionalOneToOne
    {
      get { throw new NotImplementedException (); }
      set { throw new NotImplementedException (); }
    }
  }
}