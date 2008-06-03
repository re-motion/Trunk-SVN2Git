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
  [Instantiable]
  public abstract class Customer : Company
  {
    public new static Customer NewObject()
    {
      return NewObject<Customer>().With();
    }

    protected Customer()
    {
    }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "Number ASC")]
    public abstract ObjectList<Order> Orders { get; }

    [Mandatory]
    public abstract Official PrimaryOfficial { get; set; }
  }
}
