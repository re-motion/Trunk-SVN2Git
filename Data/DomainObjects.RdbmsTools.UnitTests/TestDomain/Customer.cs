using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [Instantiable]
  public abstract class Customer : Company
  {
    public new static Customer NewObject()
    {
      return NewObject<Customer>().With();
    }

    protected Customer()
    {
    }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "Number ASC")]
    public abstract ObjectList<Order> Orders { get; }

    [Mandatory]
    public abstract Official PrimaryOfficial { get; set; }
  }
}