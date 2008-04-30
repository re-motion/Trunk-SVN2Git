using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Region")]
  [DBTable ("TableInheritance_Region")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class Region : DomainObject
  {
    public static Region NewObject ()
    {
      return NewObject<Region> ().With();
    }

    public static Region GetObject (ObjectID id)
    {
      return GetObject<Region> (id);
    }

    protected Region()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Region")]
    public abstract ObjectList<Customer> Customers { get; }
  }
}