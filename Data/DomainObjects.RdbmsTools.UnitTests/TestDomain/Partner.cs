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
  public abstract class Partner : Company
  {
    public new static Partner NewObject()
    {
      return NewObject<Partner>().With();
    }

    protected Partner()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Description { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn("PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }
  }
}
