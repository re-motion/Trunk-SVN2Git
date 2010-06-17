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

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
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

    public event EventHandler ProtectedLoaded;
    public static event EventHandler StaticLoadHandler;
    public static event EventHandler StaticInitializationHandler;

    public readonly bool CtorCalled;
    public bool OnReferenceInitializingCalledBeforeCtor;

    public bool OnReferenceInitializingCalled;
    public ClientTransaction OnReferenceInitializingTx;
    public ObjectID OnReferenceInitializingID;
    public ClientTransaction OnReferenceInitializingBindingTransaction;

    public bool OnLoadedCalled;
    public ClientTransaction OnLoadedTx;
    public LoadMode OnLoadedLoadMode;

    public bool OnUnloadingCalled;
    public ClientTransaction OnUnloadingTx;
    public DateTime OnUnloadingDateTime;
    public bool OnUnloadedCalled;
    public ClientTransaction OnUnloadedTx;
    public DateTime OnUnloadedDateTime;

    protected Order ()
    {
      CtorCalled = true;
      OnReferenceInitializingCalledBeforeCtor = OnReferenceInitializingCalled;
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

    protected override void OnReferenceInitializing ()
    {
      base.OnReferenceInitializing ();

      OnReferenceInitializingCalled = true;
      OnReferenceInitializingTx = ClientTransaction.Current;
      OnReferenceInitializingID = ID;
      OnReferenceInitializingBindingTransaction = HasBindingTransaction ? GetBindingTransaction() : null;

      if (StaticInitializationHandler != null)
        StaticInitializationHandler (this, EventArgs.Empty);
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      OnLoadedCalled = true;
      OnLoadedTx = ClientTransaction.Current;
      OnLoadedLoadMode = loadMode;
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler (this, EventArgs.Empty);
    }

    protected override void OnUnloading ()
    {
      base.OnUnloading ();
      OnUnloadingCalled = true;
      OnUnloadingTx = ClientTransaction.Current;

      OnUnloadingDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadingDateTime)
      {
      }
    }

    protected override void OnUnloaded ()
    {
      base.OnUnloading ();
      OnUnloadedCalled = true;
      OnUnloadedTx = ClientTransaction.Current;

      OnUnloadedDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadedDateTime)
      {
      }
    }
  }
}
