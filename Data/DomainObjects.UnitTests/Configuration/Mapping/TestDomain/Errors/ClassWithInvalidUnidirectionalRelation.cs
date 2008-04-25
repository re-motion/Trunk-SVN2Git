using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidUnidirectionalRelation: DomainObject
  {
    protected ClassWithInvalidUnidirectionalRelation ()
    {
    }

    public abstract ObjectList<ClassWithInvalidUnidirectionalRelation> LeftSide { get; }
  }
}