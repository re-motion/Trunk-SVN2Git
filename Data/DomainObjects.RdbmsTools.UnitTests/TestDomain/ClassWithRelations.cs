using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable ("TableWithRelations")]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class ClassWithRelations : DomainObject
  {
    public static ClassWithRelations NewObject()
    {
      return NewObject<ClassWithRelations>().With();
    }

    protected ClassWithRelations()
    {
    }

    public abstract DerivedClass DerivedClass { get; set; }
  }
}