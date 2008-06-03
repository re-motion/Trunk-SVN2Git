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
  public abstract class Address : DomainObject
  {
    public static Address NewObject()
    {
      return NewObject<Address>().With();
    }

    protected Address()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Street { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string Zip { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string City { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Country { get; set; }

    [DBBidirectionalRelation ("Address")]
    [Mandatory]
    public abstract Company Company { get; set; }
  }
}
