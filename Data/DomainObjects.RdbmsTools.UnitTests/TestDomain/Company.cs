using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [FirstStorageGroupAttribute]
  public abstract class Company : DomainObject
  {
    public static Company NewObject ()
    {
      return NewObject<Company>().With();
    }

    protected Company ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string PhoneNumber { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Company", ContainsForeignKey = true)]
    public abstract Address Address { get; set; }
  }
}