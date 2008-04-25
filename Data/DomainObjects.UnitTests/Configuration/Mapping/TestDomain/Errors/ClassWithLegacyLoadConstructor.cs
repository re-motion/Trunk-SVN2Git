using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [Instantiable]
  public abstract class ClassWithLegacyLoadConstructor: DomainObject
  {
    protected ClassWithLegacyLoadConstructor ()
    {
    }

    protected ClassWithLegacyLoadConstructor (DataContainer dataContainer)
    {
    }
  }
}