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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class OrderItem : TestDomainBase
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem> ().With();
    }

    public static OrderItem NewObject (Order order)
    {
      return NewObject<OrderItem> ().With (order);
    }

    public new static OrderItem GetObject (ObjectID id)
    {
      return GetObject<OrderItem> (id);
    }

    public event EventHandler ProtectedLoaded;

    protected OrderItem()
    {
    }

    protected OrderItem (Order order)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    public abstract int Position { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Product { get; set; }

    [DBBidirectionalRelation ("OrderItems")]
    [Mandatory]
    public abstract Order Order { get; set; }

    [StorageClassNone]
    public object OriginalOrder
    {
      get { return Properties[typeof (OrderItem), "Order"].GetOriginalValue<Order>(); }
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
    }
  }
}
