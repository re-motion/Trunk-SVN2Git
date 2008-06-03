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

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class File : DomainObject
  {
    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    protected File()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Number { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract Client Client { get; set; }
  }
}
