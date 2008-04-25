using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [Instantiable]
  public abstract class DerivedClassWithStorageSpecificIdentifierAttribute : BaseClassWithoutStorageSpecificIdentifierAttribute
  {
    protected DerivedClassWithStorageSpecificIdentifierAttribute()
    {
    }
  }
}