using System;

using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class OrderItem : DomainObject
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem> ().With ();
    }

    protected OrderItem ()
    {
    }

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
