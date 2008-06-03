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
  [FirstStorageGroupAttribute]
  public abstract class Company : DomainObject
  {
    public static Company NewObject ()
    {
      return NewObject<Company>().With();
    }

    protected Company ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string PhoneNumber { get; set; }

    [DBBidirectionalRelation ("Company")]
    [Mandatory]
    public abstract Ceo Ceo { get; set; }

    [DBBidirectionalRelation ("Company", ContainsForeignKey = true)]
    public abstract Address Address { get; set; }
  }
}
