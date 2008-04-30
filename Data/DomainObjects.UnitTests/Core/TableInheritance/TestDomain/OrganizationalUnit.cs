using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_OrganizationalUnit")]
  [DBTable ("TableInheritance_OrganizationalUnit")]
  [Instantiable]
  public abstract class OrganizationalUnit: DomainBase
  {
    public static OrganizationalUnit NewObject()
    {
      return NewObject<OrganizationalUnit>().With();
    }

    protected OrganizationalUnit()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}