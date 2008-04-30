using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassHavingClassIDAttribute : DomainObject
  {
    protected ClassHavingClassIDAttribute ()
    {
    }
  }
}