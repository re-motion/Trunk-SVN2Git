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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement
{
  /// <summary>
  /// Represents the state of an <see cref="IObjectEndPoint"/> that is synchronized with the opposite <see cref="IRelationEndPoint"/>.
  /// </summary>
  public class SynchronizedObjectEndPointSyncState : IObjectEndPointSyncState
  {
    private readonly IObjectEndPoint _endPoint;

    public SynchronizedObjectEndPointSyncState (IObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      _endPoint = endPoint;
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      return new ObjectEndPointDeleteCommand (_endPoint);
    }

    public IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      var oppositeEndPointDefinition = _endPoint.Definition.GetOppositeEndPointDefinition ();

      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (_endPoint.OppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand (_endPoint);
      else if (oppositeEndPointDefinition.IsAnonymous)
        return new ObjectEndPointSetUnidirectionalCommand (_endPoint, newRelatedObject);
      else if (oppositeEndPointDefinition.Cardinality == CardinalityType.One)
        return new ObjectEndPointSetOneOneCommand (_endPoint, newRelatedObject);
      else
        return new ObjectEndPointSetOneManyCommand (_endPoint, newRelatedObject);
    }
  }
}