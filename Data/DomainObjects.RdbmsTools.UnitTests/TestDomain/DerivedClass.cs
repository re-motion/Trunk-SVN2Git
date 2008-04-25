using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class DerivedClass : ConcreteClass
  {
    public new static DerivedClass NewObject()
    {
      return NewObject<DerivedClass>().With();
    }

    protected DerivedClass()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInDerivedClass { get; set; }
  }
}