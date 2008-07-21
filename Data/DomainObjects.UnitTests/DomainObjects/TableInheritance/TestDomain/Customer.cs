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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain
{
  public enum CustomerType
  {
    Standard = 0,
    Premium = 1
  }

  [ClassID ("TI_Customer")]
  [Instantiable]
  public abstract class Customer: Person
  {
    public new static Customer NewObject ()
    {
      return NewObject<Customer> ().With ();
    }

    public new static Customer GetObject (ObjectID id)
    {
      return GetObject<Customer> (id);
    }

    protected Customer()
    {
    }

    public abstract CustomerType CustomerType { get; set; }

    public abstract DateTime CustomerSince { get; set; }

    [DBBidirectionalRelation ("Customers")]
    public abstract Region Region { get; set; }

    [DBBidirectionalRelation ("Customer")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
