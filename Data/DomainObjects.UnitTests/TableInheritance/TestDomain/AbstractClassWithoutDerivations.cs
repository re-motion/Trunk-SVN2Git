using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_AbstractClassWithoutDerivations")]
  [TableInheritanceTestDomain]
  public abstract class AbstractClassWithoutDerivations : DomainObject
  {
    protected AbstractClassWithoutDerivations ()
    {
    }

    [DBBidirectionalRelation ("AbstractClassesWithoutDerivations")]
    public abstract DomainBase DomainBase { get; }
  }
}