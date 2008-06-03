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

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_HistoryEntry")]
  [DBTable ("TableInheritance_HistoryEntry")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class HistoryEntry: DomainObject
  {
    public static HistoryEntry NewObject()
    {
      return NewObject<HistoryEntry>().With();
    }

    public static HistoryEntry GetObject (ObjectID id)
    {
      return GetObject<HistoryEntry> (id);
    }

    protected HistoryEntry()
    {
    }

    public abstract DateTime HistoryDate { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 250)]
    public abstract string Text { get; set; }

    [DBBidirectionalRelation ("HistoryEntries")]
    [Mandatory]
    public abstract DomainBase Owner { get; set; }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
