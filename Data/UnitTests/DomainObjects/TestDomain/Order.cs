/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
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
    public LoadMode LastLoadMode;

    protected Order ()
    {
      CtorCalled = true;
    }

    [DBColumn ("OrderNo")]
    public abstract int OrderNumber { get; set; }

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

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      LastLoadMode = loadMode;
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler (this, EventArgs.Empty);
    }
  }
}
