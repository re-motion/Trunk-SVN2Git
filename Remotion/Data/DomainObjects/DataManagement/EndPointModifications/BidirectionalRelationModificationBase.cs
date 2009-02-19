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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents all modification steps needed to change a bidirectional relation.
  /// </summary>
  /// <remarks>
  /// Bidirectional relation modifications always comprise multiple steps: they need to be performed on either side of the relation being changed, 
  /// and usually they also invole one "previous" or "new" related object. (Eg. an insert modificaton has a previous related object (possibly null),
  /// a remove modification has a new related object (null).) This class aggregates these modification steps and allows executing them all at once,
  /// with events being raised before and after the full operation.
  /// </remarks>
  public abstract class BidirectionalRelationModificationBase
  {
    private readonly List<RelationEndPointModification> _modifications;

    protected BidirectionalRelationModificationBase (params RelationEndPointModification[] modifications)
    {
      ArgumentUtility.CheckNotNull ("modifications", modifications);
      _modifications = new List<RelationEndPointModification>(modifications);
    }

    public ReadOnlyCollection<RelationEndPointModification> GetModificationSteps ()
    {
      return _modifications.AsReadOnly ();
    }

    public void AddModificationStep (RelationEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);
      _modifications.Add (modification);
    }

    public abstract void ExecuteAllSteps ();
  }
}