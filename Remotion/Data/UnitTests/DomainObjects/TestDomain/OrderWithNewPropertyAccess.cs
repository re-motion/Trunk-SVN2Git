// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderWithNewPropertyAccess: DomainObject, ISupportsGetObject
  {
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
      get { return Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderWithNewPropertyAccess.Customer"].GetOriginalValue<Customer>(); }
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
