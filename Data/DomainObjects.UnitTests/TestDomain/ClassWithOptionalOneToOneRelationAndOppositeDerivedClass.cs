using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithOptionalOneToOneRelationAndOppositeDerivedClass")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithOptionalOneToOneRelationAndOppositeDerivedClass : TestDomainBase
  {
    protected ClassWithOptionalOneToOneRelationAndOppositeDerivedClass ()
    {
    }

    public abstract Company Company { get; set; }
  }
}
