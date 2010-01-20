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
    public static DataContainer[] GetAndCheckAffectedDataContainers (DataContainerMap dataContainerMap, IEnumerable<ObjectID> objectIDs)
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

    public static RelationEndPointID[] GetAndCheckAffectedEndPointIDs (
        RelationEndPointMap relationEndPointMap, 
        IEnumerable<DataContainer> unregisteredDataContainers)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);
      ArgumentUtility.CheckNotNull ("unregisteredDataContainers", unregisteredDataContainers);

      var endPointIDsToBeUnloaded = from dc in unregisteredDataContainers
                                   from endPointID in GetAndCheckAffectedEndPointIDs (relationEndPointMap, dc)
                                   select endPointID;
      return endPointIDsToBeUnloaded.ToArray();
    }

    public static RelationEndPointID[] GetAndCheckAffectedEndPointIDs (RelationEndPointMap relationEndPointMap, DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var unloadedEndPointIDs = from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                                from unloadedEndPointID in GetAndCheckAffectedEndPointIDs (relationEndPointMap, associatedEndPointID)
                                select unloadedEndPointID;
      return unloadedEndPointIDs.ToArray ();
    }

    private static IEnumerable<RelationEndPointID> GetAndCheckAffectedEndPointIDs (RelationEndPointMap relationEndPointMap, RelationEndPointID endPointID)
    {
      if (!endPointID.Definition.IsVirtual && !relationEndPointMap.Contains (endPointID))
        throw new InvalidOperationException ("Real end point has not been registered.");

      var maybeLoadedEndPoint = Maybe
          .ForValue (relationEndPointMap[endPointID])
          .Where (endPoint => EnsureUnchanged (endPoint.ObjectID, endPoint));

      var maybeRealEndPointID = Maybe.ForValue (endPointID).Where (id => !id.Definition.IsVirtual);

      var maybeVirtualNullEndPointID =
          maybeLoadedEndPoint
              .Where (endPoint => endPoint.Definition.IsVirtual)
              .Select (endPoint => endPoint as ObjectEndPoint)
              .Where (endPoint => endPoint.OppositeObjectID == null)
              .Select (endPoint => endPoint.ID);

      var maybeOppositeEndPointID = 
          maybeLoadedEndPoint
              .Where (endPoint => !endPoint.Definition.IsVirtual)
              .Select (endPoint => endPoint as ObjectEndPoint)
              .Where (endPoint => endPoint.OppositeObjectID != null)
              .Select (endPoint => new RelationEndPointID (endPoint.OppositeObjectID, endPoint.Definition.GetOppositeEndPointDefinition ()))
              .Select (oppositeID => relationEndPointMap[oppositeID]) // only loaded opposite end points!
              .Where (oppositeEndPoint => EnsureUnchanged (endPointID.ObjectID, oppositeEndPoint))
              .Select (oppositeEndPoint => oppositeEndPoint.ID);
      
      return Maybe.EnumerateValues (
          maybeRealEndPointID,
          maybeVirtualNullEndPointID,
          maybeOppositeEndPointID);
    }

    private static bool EnsureUnchanged (ObjectID unloadedObjectID, IEndPoint endPoint)
    {
      if (endPoint.HasChanged)
      {
        var message = string.Format (
            "Object '{0}' cannot be unloaded because one of its relations has been changed. Only unchanged objects can be unloaded. "
            + "Changed end point: '{1}'.",
            unloadedObjectID, 
            endPoint.ID);
        throw new InvalidOperationException (message);
      }

      return true;
    }
  }
}