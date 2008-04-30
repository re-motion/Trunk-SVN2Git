using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutClassIDColumn")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutClassIDColumn : TestDomainBase
  {
    protected ClassWithoutClassIDColumn ()
    {
    }
  }
}
