using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class OrderTicket : TestDomainBase
  {
    public static OrderTicket NewObject ()
    {
      return NewObject<OrderTicket> ().With();
    }

    // New OrderTickets need an associated order for correct initialization.
    public static OrderTicket NewObject (Order order)
    {
      OrderTicket orderTicket = NewObject<OrderTicket>().With (order);
      return orderTicket;
    }

    public new static OrderTicket GetObject (ObjectID id)
    {
      return DomainObject.GetObject<OrderTicket> (id);
    }

    public new static OrderTicket GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<OrderTicket> (id, includeDeleted);
    }

    protected OrderTicket ()
    {
    }

    protected OrderTicket (Order order)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string FileName { get; set; }

    [DBBidirectionalRelation ("OrderTicket", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Order Order { get; set; }
  }
}
