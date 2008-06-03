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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StateUsage : AccessControlObject
  {
    public static StateUsage NewObject ()
    {
      return NewObject<StateUsage> ().With ();
    }

    protected StateUsage ()
    {
    }

    [DBBidirectionalRelation ("Usages")]
    [Mandatory]
    public abstract StateDefinition StateDefinition { get; set; }

    [DBBidirectionalRelation ("StateUsages")]
    [Mandatory]
    public abstract StateCombination StateCombination { get; set; }

    protected override void OnCommitting (EventArgs args)
    {
      base.OnCommitting (args);

      if (StateCombination != null && StateCombination.Class != null)
        StateCombination.Class.Touch ();
    }
  }
}
