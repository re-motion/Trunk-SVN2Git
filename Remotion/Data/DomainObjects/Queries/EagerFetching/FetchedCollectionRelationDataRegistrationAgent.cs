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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for collection-valued relation end-points.
  /// </summary>
  [Serializable]
  public class FetchedCollectionRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (FetchedCollectionRelationDataRegistrationAgent));

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject[] originatingObjects,
        DomainObject[] relatedObjects,
        ILoadedDataContainerProvider loadedDataContainerProvider,
        IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);
      ArgumentUtility.CheckNotNull ("loadedDataContainerProvider", loadedDataContainerProvider);
      ArgumentUtility.CheckNotNull ("virtualEndPointProvider", virtualEndPointProvider);

      if (relationEndPointDefinition.Cardinality != CardinalityType.Many || relationEndPointDefinition.IsAnonymous)
        throw new ArgumentException ("Only collection-valued relations can be handled by this registration agent.", "relationEndPointDefinition");
      
      CheckOriginatingObjects (relationEndPointDefinition, originatingObjects);

      var virtualRelationEndPointDefinition = (VirtualRelationEndPointDefinition) relationEndPointDefinition;
      var groupedRelatedObjects = CorrelateRelatedObjects (relatedObjects, virtualRelationEndPointDefinition, loadedDataContainerProvider);
      RegisterEndPointData (relationEndPointDefinition, virtualEndPointProvider, originatingObjects, groupedRelatedObjects);
    }

    private ILookup<ObjectID, DomainObject> CorrelateRelatedObjects (
        IEnumerable<DomainObject> relatedObjects, 
        VirtualRelationEndPointDefinition relationEndPointDefinition,
        ILoadedDataContainerProvider loadedDataContainerProvider)
    {
      var relatedObjectsWithForeignKey = GetForeignKeysForVirtualEndPointDefinition (relatedObjects, relationEndPointDefinition, loadedDataContainerProvider);
      return relatedObjectsWithForeignKey.ToLookup (k => k.Item1, k => k.Item2);
    }

    private void RegisterEndPointData (
       IRelationEndPointDefinition relationEndPointDefinition,
       IVirtualEndPointProvider virtualEndPointProvider,
       IEnumerable<DomainObject> originatingObjects,
       ILookup<ObjectID, DomainObject> groupedRelatedObjects)
    {
      var relatedObjectsByOriginalObject = groupedRelatedObjects;
      foreach (var originalObject in originatingObjects)
      {
        if (originalObject != null)
        {
          var relationEndPointID = RelationEndPointID.Create (originalObject.ID, relationEndPointDefinition);
          var relatedObjects = relatedObjectsByOriginalObject[originalObject.ID];
          if (!TrySetCollectionEndPointData (virtualEndPointProvider, relationEndPointID, relatedObjects.ToArray ()))
            s_log.DebugFormat ("Relation data for relation end-point '{0}' is discarded; the end-point has already been loaded.", relationEndPointID);
        }
      }
    }

    private bool TrySetCollectionEndPointData (IVirtualEndPointProvider virtualEndPointProvider, RelationEndPointID endPointID, DomainObject[] items)
    {
      var endPoint = (ICollectionEndPoint) virtualEndPointProvider.GetOrCreateVirtualEndPoint (endPointID);
      if (endPoint.IsDataComplete)
        return false;

      endPoint.MarkDataComplete (items);
      return true;
    }
  }
}