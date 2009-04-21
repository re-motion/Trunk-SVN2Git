// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class OrderItem : TestDomainBase
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem> ();
    }

    public static OrderItem NewObject (Order order)
    {
      return NewObject<OrderItem> (ParamList.Create (order));
    }

    public static OrderItem NewObject (string product)
    {
      return NewObject<OrderItem> (ParamList.Create (product));
    }

    public new static OrderItem GetObject (ObjectID id)
    {
      return GetObject<OrderItem> (id);
    }

    public static event EventHandler StaticLoadHandler;
    public event EventHandler ProtectedLoaded;

    protected OrderItem()
    {
    }

    protected OrderItem (Order order)
    {
      ArgumentUtility.CheckNotNull ("order", order);
      Order = order;
    }

    protected OrderItem (string product)
    {
      Product = product;
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
      if (ProtectedLoaded != null)
        ProtectedLoaded (this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler (this, EventArgs.Empty);
      base.OnLoaded (loadMode);
    }
  }
}
