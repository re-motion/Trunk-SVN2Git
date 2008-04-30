using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderItemWithNewPropertyAccess : DomainObject
  {
    public static OrderItemWithNewPropertyAccess NewObject ()
    {
      return NewObject<OrderItemWithNewPropertyAccess> ().With();
    }

    protected OrderItemWithNewPropertyAccess()
    {
    }

    [DBBidirectionalRelation ("OrderItems")]
    public abstract OrderWithNewPropertyAccess Order { get; set; }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess OriginalOrder
    {
      get { return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItemWithNewPropertyAccess.Order"].GetOriginalValue <OrderWithNewPropertyAccess>(); }
    }
  }
}
