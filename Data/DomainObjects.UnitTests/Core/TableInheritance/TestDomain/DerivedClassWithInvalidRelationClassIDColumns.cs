using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithInvalidRelationClassIDColumns")]
  [Instantiable]
  public abstract class DerivedClassWithInvalidRelationClassIDColumns : BaseClassWithInvalidRelationClassIDColumns
  {
    protected DerivedClassWithInvalidRelationClassIDColumns ()
    {
    }
  }
}
