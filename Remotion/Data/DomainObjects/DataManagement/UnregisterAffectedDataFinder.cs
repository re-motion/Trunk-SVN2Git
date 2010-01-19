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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Finds the <see cref="DataContainer"/> and <see cref="RelationEndPointID"/> instances that are affected by an 
  /// <see cref="DataManager.Unregister(System.Collections.Generic.IEnumerable{Remotion.Data.DomainObjects.ObjectID})"/> operation.
  /// </summary>
  public static class UnregisterAffectedDataFinder
  {
    public static DataContainer[] GetAndCheckDataContainers (DataContainerMap dataContainerMap, IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var loadedDataContainers = objectIDs.Select (id => dataContainerMap[id]).Where (dc => dc != null);
      var notUnchangedDataContainers = loadedDataContainers.Where (dc => dc.State != StateType.Unchanged);
      if (notUnchangedDataContainers.Any ())
      {
        var message =
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + SeparatedStringBuilder.Build (", ", notUnchangedDataContainers, dc => String.Format ("'{0}' ({1})", dc.ID, dc.State))
            + ".";
        throw new InvalidOperationException (message);
      }
      
      return loadedDataContainers.ToArray();
    }

    public static RelationEndPointID[] GetAndCheckEndPointIDs (RelationEndPointMap relationEndPointMap, IEnumerable<DataContainer> unregisteredDataContainers)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);
      ArgumentUtility.CheckNotNull ("unregisteredDataContainers", unregisteredDataContainers);

      var endPointIDsToBeUnloaded = unregisteredDataContainers.SelectMany (
          dc => GetEndPointIDs (relationEndPointMap, dc).Select (id => new { UnloadedID = dc.ID, EndPointID = id }));

      var changedEndPointIDs = endPointIDsToBeUnloaded.Where (idData => relationEndPointMap[idData.EndPointID].HasChanged);
      if (changedEndPointIDs.Any ())
      {
        var groupedEndPoints = changedEndPointIDs.GroupBy (idData => idData.UnloadedID);

        var endPointString = SeparatedStringBuilder.Build (
            ", ",
            groupedEndPoints,
            grouping => String.Format (
                "'{0}' ({1})", 
                grouping.Key, 
                SeparatedStringBuilder.Build (", ", grouping, idData => idData.EndPointID.ToString())));

        var message = "The following objects cannot be unloaded because they take part in a relation that has been changed: " + endPointString + ".";
        throw new InvalidOperationException (message);
      }

      return endPointIDsToBeUnloaded.Select (idData => idData.EndPointID).ToArray();
    }

    public static RelationEndPointID[] GetEndPointIDs (RelationEndPointMap relationEndPointMap, DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var unloadedEndPointIDs = from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                                from unloadedEndPointID in GetEndPointIDs (relationEndPointMap, associatedEndPointID)
                                select unloadedEndPointID;
      return unloadedEndPointIDs.ToArray ();
    }

    private static IEnumerable<RelationEndPointID> GetEndPointIDs (RelationEndPointMap relationEndPointMap, RelationEndPointID endPointID)
    {
      if (!endPointID.Definition.IsVirtual && !relationEndPointMap.Contains (endPointID))
        throw new InvalidOperationException ("Real end point has not been registered.");

      var maybeRealEndPointID = Maybe.ForValue (endPointID).Where (id => !id.Definition.IsVirtual);

      var maybeVirtualNullEndPointID = 
          Maybe.ForValue (endPointID)
              .Where (id => id.Definition.IsVirtual)
              .Where (id => id.Definition.Cardinality == CardinalityType.One)
              .Select (id => (ObjectEndPoint) relationEndPointMap[id])
              .Where (endPoint => endPoint.OppositeObjectID == null)
              .Select (endPoint => endPoint.ID);

      var maybeOppositeEndPointID = 
          Maybe.ForValue (endPointID)
              .Where (id => !id.Definition.IsVirtual)
              .Where (id => id.Definition.Cardinality == CardinalityType.One)
              .Select (id => (ObjectEndPoint) relationEndPointMap[id])
              .Where (endPoint => endPoint.OppositeObjectID != null)
              .Select (endPoint => new RelationEndPointID (endPoint.OppositeObjectID, endPoint.Definition.GetOppositeEndPointDefinition ()))
              .Where (relationEndPointMap.Contains);
      
      return Maybe.EnumerateValues (
          maybeRealEndPointID,
          maybeVirtualNullEndPointID,
          maybeOppositeEndPointID);
    }
  }
}