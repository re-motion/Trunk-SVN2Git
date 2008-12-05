// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class Order : DomainObject
  {
    public static Order NewObject()
    {
      return NewObject<Order>().With();
    }

    protected Order()
    {
    }

    public abstract int Number { get; set; }

    public abstract OrderPriority Priority { get; set; }

    [DBBidirectionalRelation ("Orders")]
    [Mandatory]
    public abstract Customer Customer { get; set; }

    [DBBidirectionalRelation ("Orders")]
    [Mandatory]
    public abstract Official Official { get; set; }

    [DBBidirectionalRelation ("Order")]
    [Mandatory]
    public abstract ObjectList<OrderItem> OrderItems { get; set; }

    [DBBidirectionalRelation ("TransactionOrder")]
    [StorageClassTransaction]
    [Mandatory]
    public abstract ObjectList<OrderItem> TransactionOrderItems { get; set; }
  }
}
