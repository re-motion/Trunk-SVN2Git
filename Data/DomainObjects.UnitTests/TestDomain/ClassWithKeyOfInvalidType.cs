using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithKeyOfInvalidType")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithKeyOfInvalidType : TestDomainBase
  {
    protected ClassWithKeyOfInvalidType ()
    {
    }
  }
}
