using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutIDColumn")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutIDColumn : TestDomainBase
  {
    protected ClassWithoutIDColumn()
    {
    }
  }
}
