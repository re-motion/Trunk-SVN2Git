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
  /// Represents a modification to a relation that only calls <see cref="RelationEndPointModification.Perform"/> on its 
  /// steps. Specifically, it does not notify the client transaction when its steps are executed, and it does not
  /// call <see cref="RelationEndPointModification.Begin"/> or <see cref="RelationEndPointModification.End"/> on its steps.
  /// This is used when <see cref="RelationEndPointModification.CreateBidirectionalModification"/> is called on a "no-op"
  /// <see cref="RelationEndPointModification"/> such as <see cref="ObjectEndPointSetSameModification"/> or 
  /// <see cref="CollectionEndPointSelfReplaceModification"/>. Such relation modifications should be performed (in order to touch the 
  /// data stored by the end point, for example), but they should not raise any events.
  /// </summary>
  public class CompositeRelationModificationWithoutEvents : CompositeRelationModification
  {
    public CompositeRelationModificationWithoutEvents (params IRelationEndPointModification[] modificationSteps)
        : this ((IEnumerable<IRelationEndPointModification>) modificationSteps)
    {
    }

    public CompositeRelationModificationWithoutEvents (IEnumerable<IRelationEndPointModification> modificationSteps)
        : base (modificationSteps)
    {
    }

    public void Perform ()
    {
      foreach (var modificationStep in GetModificationSteps ())
        modificationStep.Perform ();
    }

    public override void ExecuteAllSteps ()
    {
      Perform ();
    }
  }
}
