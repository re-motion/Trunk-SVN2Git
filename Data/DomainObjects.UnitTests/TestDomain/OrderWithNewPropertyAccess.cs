using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderWithNewPropertyAccess: DomainObject
  {
    public static OrderWithNewPropertyAccess NewObject()
    {
      return NewObject<OrderWithNewPropertyAccess>().With();
    }

    public static OrderWithNewPropertyAccess GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderWithNewPropertyAccess> (id);
    }

    protected OrderWithNewPropertyAccess()
    {
    }

    [DBColumn (("OrderNo"))]
    public abstract int OrderNumber { get; set; }

    public virtual DateTime DeliveryDate
    {
      get
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        return CurrentProperty.GetValue<DateTime>();
      }
      set
      {
        // meaningless access to OrderNumber to test nested property calls
        int number = OrderNumber;
        OrderNumber = number;

        CurrentProperty.SetValue (value);
      }
    }

    public virtual Customer Customer
    {
      get { return CurrentProperty.GetValue<Customer>(); }
      set { CurrentProperty.SetValue (value); }
    }

    [StorageClassNone]
    public virtual Customer OriginalCustomer
    {
      get { return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderWithNewPropertyAccess.Customer"].GetOriginalValue<Customer>(); }
    }

    [DBBidirectionalRelation ("Order")]
    public virtual ObjectList<OrderItemWithNewPropertyAccess> OrderItems
    {
      get { return CurrentProperty.GetValue<ObjectList<OrderItemWithNewPropertyAccess>>(); }
    }

    [StorageClassNone]
    public virtual int NotInMapping
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess NotInMappingRelated
    {
      get { return CurrentProperty.GetValue<OrderWithNewPropertyAccess> (); }
      set { CurrentProperty.SetValue (value); }
    }

    [StorageClassNone]
    public virtual ObjectList<OrderItem> NotInMappingRelatedObjects
    {
      get { return CurrentProperty.GetValue<ObjectList<OrderItem>> (); }
    }
  }
}