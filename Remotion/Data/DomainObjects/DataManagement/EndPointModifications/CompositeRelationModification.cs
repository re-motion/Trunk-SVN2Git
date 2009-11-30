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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Collects all <see cref="IRelationEndPointModification"/> steps needed to change a relation.
  /// </summary>
  /// <remarks>
  /// Bidirectional relation modifications always comprise multiple steps: they need to be performed on either side of the relation being changed, 
  /// and usually they also invole one "previous" or "new" related object. (Eg. an insert modificaton has a previous related object (possibly 
  /// <see langword="null" />), a remove modification has a new related object (<see langword="null" />).) Subclasses of 
  /// <see cref="CompositeRelationModification"/> aggregate these  modification steps and allow executing them all at once, with events being raised 
  /// before and after the full operation.
  /// </remarks>
  public abstract class CompositeRelationModification
  {
    private readonly List<IRelationEndPointModification> _modificationSteps;

    protected CompositeRelationModification (IEnumerable<IRelationEndPointModification> modificationSteps)
    {
      ArgumentUtility.CheckNotNull ("modificationSteps", modificationSteps);
      _modificationSteps = new List<IRelationEndPointModification>(modificationSteps);
    }

    public ReadOnlyCollection<IRelationEndPointModification> GetModificationSteps ()
    {
      return _modificationSteps.AsReadOnly ();
    }

    public void AddModificationStep (IRelationEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);
      _modificationSteps.Add (modification);
    }

    public abstract void ExecuteAllSteps ();
  }
}
