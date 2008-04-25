using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class SecondDerivedClass : ConcreteClass
  {
    public new static SecondDerivedClass NewObject()
    {
      return NewObject<SecondDerivedClass>().With();
    }

    protected SecondDerivedClass()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInSecondDerivedClass { get; set; }

    [DBColumn ("ClassWithRelationsInSecondDerivedClassID")]
    public abstract ClassWithRelations ClassWithRelationsToSecondDerivedClass { get; set; }
  }
}