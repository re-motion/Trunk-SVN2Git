using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  [DBTable ("TableWithoutProperties")]
  [FirstStorageGroupAttribute]
  public abstract class ClassWithoutProperties : DomainObject
  {
  }
}