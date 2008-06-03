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
using System.Reflection;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [SecondStorageGroupAttribute]
  [Instantiable]
  public abstract class Official : DomainObject
  {
    public static Official NewObject()
    {
      return NewObject<Official>().With();
    }

    public static Official GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Official> (id);
    }

    protected Official()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract OrderPriority ResponsibleForOrderPriority { get; set; }

    public abstract CustomerType ResponsibleForCustomerType { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
