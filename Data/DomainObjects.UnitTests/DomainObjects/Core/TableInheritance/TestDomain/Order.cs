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

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Order")]
  [DBTable ("TableInheritance_Order")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class Order: DomainObject
  {
    public static Order NewObject()
    {
      return NewObject<Order>().With();
    }

    public static Order GetObject (ObjectID id)
    {
      return GetObject<Order> (id);
    }

    protected Order()
    {
    }

    public abstract int Number { get; set; }

    public abstract DateTime OrderDate { get; set; }

    [DBBidirectionalRelation ("Orders")]
    public abstract Customer Customer { get; set; }
  }
}
