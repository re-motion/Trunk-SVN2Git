using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClosedGenericClass: GenericClassNotInMapping<int>
  {
    protected ClosedGenericClass ()
    {
    }
  }
}
