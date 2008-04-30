using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Order : TestDomainBase
  {
    public static Order NewObject ()
    {
      return NewObject<Order> ().With();
    }

    public new static Order GetObject (ObjectID id)
    {
      return GetObject<Order> (id);
    }

    public new static Order GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<Order> (id, includeDeleted);
    }

    public event EventHandler ProtectedLoaded;
    public static event EventHandler StaticLoadHandler;

    public readonly bool CtorCalled = false;

    protected Order ()
    {
      CtorCalled = true;
    }

    [DBColumn ("OrderNo")]
    public abstract int OrderNumber { get; set; }

    [StorageClassNone]
    public TransactionalAccessor<int> OrderNumberTx
    {
      get { return GetTransactionalAccessor<int> (Properties[typeof (Order), "OrderNumber"]); }
    }

    public abstract DateTime DeliveryDate { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Official Official { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public abstract OrderTicket OrderTicket { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public abstract ObjectList<OrderItem> OrderItems { get; }

    public new void PreparePropertyAccess (string propertyName)
    {
      base.PreparePropertyAccess (propertyName);
    }

    public new void PropertyAccessFinished()
    {
      base.PropertyAccessFinished();
    }

    [StorageClassNone]
    public new PropertyAccessor CurrentProperty
    {
      get { return base.CurrentProperty; }
    }

    [StorageClassNone]
    public Customer OriginalCustomer
    {
      get { return Properties[typeof (Order), "Customer"].GetOriginalValue<Customer>(); }
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

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler (this, EventArgs.Empty);
    }
  }
}