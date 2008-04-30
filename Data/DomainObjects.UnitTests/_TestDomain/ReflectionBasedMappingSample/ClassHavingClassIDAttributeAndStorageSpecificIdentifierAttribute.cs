using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute")]
  [DBTable ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute : DomainObject
  {
    protected ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
    }
  }
}