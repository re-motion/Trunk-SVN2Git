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
      return GetObject<OrderTicket> (id);
    }

    public new static OrderTicket GetObject (ObjectID id, bool includeDeleted)
    {
      return GetObject<OrderTicket> (id, includeDeleted);
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


    [StorageClassTransaction]
    public abstract int Int32TransactionProperty { get; set; }
  }
}
