using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClosedGenericClassWithOneSideRelationProperties
      : GenericClassWithOneSideRelationPropertiesNotInMapping<ClosedGenericClassWithManySideRelationProperties>
  {
    protected ClosedGenericClassWithOneSideRelationProperties ()
    {
    }
  }
}