using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class Order : DomainObject
  {
    public static Order NewObject()
    {
      return NewObject<Order>().With();
    }

    protected Order()
    {
    }

    public abstract int Number { get; set; }

    public abstract OrderPriority Priority { get; set; }

    [DBBidirectionalRelation ("Orders")]
    [Mandatory]
    public abstract Customer Customer { get; set; }

    [DBBidirectionalRelation ("Orders")]
    [Mandatory]
    public abstract Official Official { get; set; }

    [DBBidirectionalRelation ("Order")]
    [Mandatory]
    public abstract ObjectList<OrderItem> OrderItems { get; set; }
  }
}