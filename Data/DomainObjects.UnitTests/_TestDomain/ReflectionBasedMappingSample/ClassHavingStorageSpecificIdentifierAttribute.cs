using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable ("ClassHavingStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassHavingStorageSpecificIdentifierAttribute : DomainObject
  {

    protected ClassHavingStorageSpecificIdentifierAttribute ()
    {
    }

    public abstract int NoAttribute { get; set; }

    [DBColumn ("CustomName")]
    public abstract int StorageSpecificName { get; set; }
  }
}