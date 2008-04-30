using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [TestDomain]
  public abstract class BaseClassWithoutStorageSpecificIdentifierAttribute : DomainObject
  {
    protected BaseClassWithoutStorageSpecificIdentifierAttribute ()
    {
    }
  }
}
