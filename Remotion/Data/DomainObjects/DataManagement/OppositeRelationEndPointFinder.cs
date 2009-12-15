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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Holds convenience methods to find opposite <see cref="RelationEndPoint"/> instances.
  /// </summary>
  public static class OppositeRelationEndPointFinder
  {
    public static IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (RelationEndPointMap relationEndPointMap, DomainObject domainObject)
    {
      return from endPointID in relationEndPointMap.ClientTransaction.GetDataContainer (domainObject).AssociatedRelationEndPointIDs
             let endPoint = relationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID)
             from oppositeEndPoint in GetOppositeRelationEndPoints (relationEndPointMap, endPoint)
             select oppositeEndPoint;
    }

    public static IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (RelationEndPointMap relationEndPointMap, RelationEndPoint endPoint)
    {
      if (endPoint.OppositeEndPointDefinition.IsAnonymous)
        return Enumerable.Empty<RelationEndPoint> ();

      if (endPoint.Definition.Cardinality == CardinalityType.One)
      {
        var objectEndPoint = (IObjectEndPoint) endPoint;

        RelationEndPoint oppositeEndPoint = GetOppositeRelationEndPoint (relationEndPointMap, objectEndPoint);
        return oppositeEndPoint == null ? Enumerable.Empty<RelationEndPoint> () : Enumerable.Repeat (oppositeEndPoint, 1);
      }
      else
      {
        var collectionEndPoint = (ICollectionEndPoint) endPoint;
        return GetOppositeRelationEndPoints (relationEndPointMap, collectionEndPoint);
      }
    }

    public static RelationEndPoint GetOppositeRelationEndPoint (RelationEndPointMap relationEndPointMap, IObjectEndPoint objectEndPoint)
    {
      if (objectEndPoint.OppositeObjectID == null)
      {
        return null;
      }
      else
      {
        var oppositeEndPointID = new RelationEndPointID (objectEndPoint.OppositeObjectID, objectEndPoint.Definition.GetOppositeEndPointDefinition());
        return relationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
      }
    }

    public static IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (RelationEndPointMap relationEndPointMap, ICollectionEndPoint collectionEndPoint)
    {
      return from oppositeDomainObject in collectionEndPoint.OppositeDomainObjects.Cast<DomainObject> ()
             let oppositeEndPointID = new RelationEndPointID (oppositeDomainObject.ID, collectionEndPoint.Definition.GetOppositeEndPointDefinition())
             let oppositeEndPoint = relationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID)
             select oppositeEndPoint;
    }
  }
}