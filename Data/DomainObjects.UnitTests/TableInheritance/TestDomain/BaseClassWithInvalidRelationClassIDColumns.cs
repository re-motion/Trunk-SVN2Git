using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_BaseClassWithInvalidRelationClassIDColumns")]
  [DBTable ("TableInheritance_BaseClassWithInvalidRelationClassIDColumns")]
  [TableInheritanceTestDomain]
  public abstract class BaseClassWithInvalidRelationClassIDColumns : DomainObject
  {
    protected BaseClassWithInvalidRelationClassIDColumns ()
    {
    }

    public abstract Client Client { get; set; }

    public abstract DomainBase DomainBase { get; set; }

    public abstract DomainBase DomainBaseWithInvalidClassIDValue { get; set; }

    public abstract DomainBase DomainBaseWithInvalidClassIDNullValue { get; set; }
  }
}