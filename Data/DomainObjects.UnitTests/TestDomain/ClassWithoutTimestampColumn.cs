using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutTimestampColumn")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutTimestampColumn: TestDomainBase
  {
    protected ClassWithoutTimestampColumn()
    {
    }
  }
}