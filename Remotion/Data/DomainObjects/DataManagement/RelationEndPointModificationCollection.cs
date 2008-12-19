// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
