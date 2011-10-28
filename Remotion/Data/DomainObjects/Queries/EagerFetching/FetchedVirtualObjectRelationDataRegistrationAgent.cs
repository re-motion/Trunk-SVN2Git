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
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for virtual object-valued relation end-points.
  /// </summary>
  [Serializable]
  public class FetchedVirtualObjectRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (FetchedVirtualObjectRelationDataRegistrationAgent));

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject[] originatingObjects,
        DomainObject[] relatedObjects,
        IDataContainerProvider dataContainerProvider,
        IRelationEndPointProvider relationEndPointProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);
      ArgumentUtility.CheckNotNull ("dataContainerProvider", dataContainerProvider);
      ArgumentUtility.CheckNotNull ("relationEndPointProvider", relationEndPointProvider);

      if (relationEndPointDefinition.Cardinality != CardinalityType.One || !relationEndPointDefinition.IsVirtual)
      {
        throw new ArgumentException (
            "Only virtual object-valued relation end-points can be handled by this registration agent.", 
            "relationEndPointDefinition");
      }

      CheckOriginatingObjects (relationEndPointDefinition, originatingObjects);

      var virtualRelationEndPointDefinition = (VirtualRelationEndPointDefinition) relationEndPointDefinition;
      var groupedRelatedObjects = CorrelateRelatedObjects (relatedObjects, virtualRelationEndPointDefinition, dataContainerProvider);
      RegisterEndPointData (relationEndPointDefinition, relationEndPointProvider, originatingObjects, groupedRelatedObjects);
    }

    private IDictionary<ObjectID, DomainObject> CorrelateRelatedObjects (
        IEnumerable<DomainObject> relatedObjects,
        VirtualRelationEndPointDefinition relationEndPointDefinition,
        IDataContainerProvider dataContainerProvider)
    {
      var relatedObjectsWithForeignKey = GetForeignKeysForVirtualEndPointDefinition (relatedObjects, relationEndPointDefinition, dataContainerProvider);
      var dictionary = new Dictionary<ObjectID, DomainObject>();
      foreach (var tuple in relatedObjectsWithForeignKey.Where (tuple => tuple.Item1 != null))
      {
        try
        {
          dictionary.Add (tuple.Item1, tuple.Item2);
        }
        catch (ArgumentException ex)
        {
          var message = string.Format (
              "Two items in the related object result set point back to the same object. This is not allowed in a 1:1 relation. "
              + "Object 1: '{0}'. Object 2: '{1}'. Foreign key property: '{2}'",
              dictionary[tuple.Item1].ID,
              tuple.Item2,
              relationEndPointDefinition.GetOppositeEndPointDefinition().PropertyName);
          throw new InvalidOperationException (message, ex);
        }
      }
      return dictionary;
    }

    private void RegisterEndPointData (
       IRelationEndPointDefinition relationEndPointDefinition,
       IRelationEndPointProvider relationEndPointProvider,
       IEnumerable<DomainObject> originatingObjects,
       IDictionary<ObjectID, DomainObject> groupedRelatedObjects)
    {
      var relatedObjectsByOriginalObject = groupedRelatedObjects;
      foreach (var originalObject in originatingObjects)
      {
        if (originalObject != null)
        {
          var relationEndPointID = RelationEndPointID.Create (originalObject.ID, relationEndPointDefinition);
          var relatedObject = relatedObjectsByOriginalObject.GetValueOrDefault (originalObject.ID);
          if (!TrySetVirtualObjectEndPointData (relationEndPointProvider, relationEndPointID, relatedObject))
            s_log.DebugFormat ("Relation data for relation end-point '{0}' is discarded; the end-point has already been loaded.", relationEndPointID);
        }
      }
    }

    private bool TrySetVirtualObjectEndPointData (IRelationEndPointProvider relationEndPointProvider, RelationEndPointID endPointID, DomainObject item)
    {
      var endPoint = (IVirtualObjectEndPoint) relationEndPointProvider.GetRelationEndPointWithMinimumLoading (endPointID);
      if (endPoint.IsDataComplete)
        return false;

      endPoint.MarkDataComplete (item);
      return true;
    }
  }
}