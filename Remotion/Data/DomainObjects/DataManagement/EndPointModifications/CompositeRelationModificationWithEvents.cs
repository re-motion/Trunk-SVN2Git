// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents a modification to a relation that notifies the client transaction when its steps are executed, and which
  /// calls <see cref="RelationEndPointModification.Begin"/> and calls <see cref="RelationEndPointModification.End"/> on all of its steps.
  /// </summary>
  public class CompositeRelationModificationWithEvents : CompositeRelationModification
  {
    public CompositeRelationModificationWithEvents (params IRelationEndPointModification[] modificationSteps)
        : this ((IEnumerable<IRelationEndPointModification>) modificationSteps)
    {
    }

    public CompositeRelationModificationWithEvents (IEnumerable<IRelationEndPointModification> modificationSteps)
        : base (modificationSteps)
    {
    }

    public void Begin ()
    {
      foreach (var modificationStep in GetModificationSteps())
        modificationStep.Begin ();
    }

    public void Perform ()
    {
      foreach (var modificationStep in GetModificationSteps ())
        modificationStep.Perform ();
    }

    public void End ()
    {
      foreach (var modificationStep in GetModificationSteps ())
        modificationStep.End ();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      foreach (var modificationStep in GetModificationSteps ())
        modificationStep.NotifyClientTransactionOfBegin ();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      foreach (var modificationStep in GetModificationSteps ())
        modificationStep.NotifyClientTransactionOfEnd ();
    }

    public override void ExecuteAllSteps ()
    {
      NotifyClientTransactionOfBegin();
      Begin();

      Perform();

      NotifyClientTransactionOfEnd();
      End();
    }
  }
}
