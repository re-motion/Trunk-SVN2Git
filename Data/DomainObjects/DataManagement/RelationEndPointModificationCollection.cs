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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointModificationCollection
  {
    private readonly List<RelationEndPointModification> _modifications;

    public RelationEndPointModificationCollection (params RelationEndPointModification[] modifications)
    {
      ArgumentUtility.CheckNotNull ("modifications", modifications);
      _modifications = new List<RelationEndPointModification>(modifications);
    }

    public void Add (RelationEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);
      _modifications.Add (modification);
    }

    public void Begin ()
    {
      foreach (RelationEndPointModification modification in _modifications)
        modification.Begin();
    }

    public void Perform ()
    {
      foreach (RelationEndPointModification modification in _modifications)
        modification.Perform ();
    }

    public void End ()
    {
      foreach (RelationEndPointModification modification in _modifications)
        modification.End ();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      foreach (RelationEndPointModification modification in _modifications)
        modification.NotifyClientTransactionOfBegin();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      foreach (RelationEndPointModification modification in _modifications)
        modification.NotifyClientTransactionOfEnd ();
    }

    public void ExecuteAllSteps ()
    {
      NotifyClientTransactionOfBegin ();
      Begin ();
      Perform ();
      NotifyClientTransactionOfEnd ();
      End ();
    }
  }
}
