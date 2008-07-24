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
