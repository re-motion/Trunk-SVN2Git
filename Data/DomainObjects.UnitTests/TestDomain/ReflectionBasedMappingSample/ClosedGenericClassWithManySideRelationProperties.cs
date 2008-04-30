using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClosedGenericClassWithManySideRelationProperties
      : GenericClassWithManySideRelationPropertiesNotInMapping<ClosedGenericClassWithOneSideRelationProperties>
  {
    protected ClosedGenericClassWithManySideRelationProperties ()
    {
    }
  }
}