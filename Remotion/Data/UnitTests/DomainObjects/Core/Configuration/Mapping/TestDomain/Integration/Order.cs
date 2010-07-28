// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Order : TestDomainBase, IOrder
  {
    public static Order NewObject ()
    {
      return NewObject<Order> ();
    }

    public new static Order GetObject (ObjectID id)
    {
      return GetObject<Order> (id);
    }

    public new static Order GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<Order> (id, includeDeleted);
    }

    [DBColumn ("OrderNo")]
    public abstract int OrderNumber { get; set; }

    [StorageClassNone]
    [LinqPropertyRedirection (typeof (Order), "OrderNumber")]
    public int RedirectedOrderNumber
    {
      get { return OrderNumber; }
    }

    [StorageClassNone]
    [LinqPropertyRedirection (typeof (Order), "RedirectedOrderNumber")]
    public int RedirectedRedirectedOrderNumber
    {
      get { return RedirectedOrderNumber; }
    }
    
    public abstract DateTime DeliveryDate { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Official Official { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public abstract OrderTicket OrderTicket { get; set; }

    [StorageClassNone]
    [LinqPropertyRedirection (typeof (Order), "OrderTicket")]
    public OrderTicket RedirectedOrderTicket
    {
      get { return OrderTicket; }
    }

    [Mandatory]
    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("Order")]
    public virtual ObjectList<OrderItem> OrderItems { get; set; }

    [StorageClassNone]
    [LinqPropertyRedirection (typeof (Order), "OrderItems")]
    public ObjectList<OrderItem> RedirectedOrderItems
    {
      get { return OrderItems; }
    }

    public void PreparePropertyAccess (string propertyName)
    {
      CurrentPropertyManager.PreparePropertyAccess (propertyName);
    }

    public void PropertyAccessFinished()
    {
      CurrentPropertyManager.PropertyAccessFinished();
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
  }
}
