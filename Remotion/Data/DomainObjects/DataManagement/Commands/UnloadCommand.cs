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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Encapsulates all logic that is required to unload a <see cref="DomainObject"/>'s data from a <see cref="DataManager"/>.
  /// </summary>
  public class UnloadCommand : IDataManagementCommand
  {
    private readonly ObjectID[] _objectIDs;
    private readonly ClientTransaction _clientTransaction;
    private readonly DataContainerMap _dataContainerMap;
    private readonly RelationEndPointMap _relationEndPointMap;

    private readonly DataContainer[] _unloadedDataContainers;
    private readonly ReadOnlyCollection<DomainObject> _unloadedDomainObjects;
    private readonly RelationEndPoint[] _unloadedEndPoints;

    public UnloadCommand (
        ObjectID[] objectIDs, 
        ClientTransaction clientTransaction, 
        DataContainerMap dataContainerMap, 
        RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      _objectIDs = objectIDs;
      _clientTransaction = clientTransaction;
      _dataContainerMap = dataContainerMap;
      _relationEndPointMap = relationEndPointMap;
      
      _unloadedDataContainers = GetAndCheckUnloadedDataContainers (_objectIDs);
      _unloadedEndPoints = GetAndCheckUnloadedEndPoints (_unloadedDataContainers);
      _unloadedDomainObjects = _unloadedDataContainers.Select (dc => dc.DomainObject).ToList ().AsReadOnly ();
    }

    public DataContainer[] UnloadedDataContainers
    {
      get { return _unloadedDataContainers; }
    }

    public RelationEndPoint[] UnloadedEndPoints
    {
      get { return _unloadedEndPoints; }
    }

    public void NotifyClientTransactionOfBegin ()
    {
      if (_unloadedDomainObjects.Count > 0)
        _clientTransaction.Execute (() => _clientTransaction.TransactionEventSink.ObjectsUnloading (_clientTransaction, _unloadedDomainObjects));
    }

    public void Begin ()
    {
      _clientTransaction.Execute (delegate
      {
        for (int i = 0; i < _unloadedDomainObjects.Count; i++)
          _unloadedDomainObjects[i].OnUnloading ();
      });
    }

    public void Perform ()
    {
      UnregisterEndPoints (_unloadedEndPoints);
      UnregisterDataContainers (_unloadedDataContainers);
    }

    public void End ()
    {
      _clientTransaction.Execute (delegate
      {
        for (int i = _unloadedDomainObjects.Count - 1; i >= 0; i--)
          _unloadedDomainObjects[i].OnUnloaded ();
      });
    }

    public void NotifyClientTransactionOfEnd ()
    {
      if (_unloadedDomainObjects.Count > 0)
        _clientTransaction.Execute (() => _clientTransaction.TransactionEventSink.ObjectsUnloaded (_clientTransaction, _unloadedDomainObjects));
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }

    // The affected DataContainers are all DataContainers to be unloaded. The DataContainers must be unchanged.
    private DataContainer[] GetAndCheckUnloadedDataContainers (IEnumerable<ObjectID> unloadedObjectIDs)
    {
      var affectedDataContainers = unloadedObjectIDs.Select (id => _dataContainerMap[id]).Where (dc => dc != null).ToArray();
      var notUnchangedDataContainers = affectedDataContainers.Where (dc => dc.State != StateType.Unchanged);
      if (notUnchangedDataContainers.Any ())
      {
        var message =
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + SeparatedStringBuilder.Build (", ", notUnchangedDataContainers, dc => String.Format ("'{0}' ({1})", dc.ID, dc.State))
            + ".";
        throw new InvalidOperationException (message);
      }
      
      return affectedDataContainers;
    }

    private RelationEndPoint[] GetAndCheckUnloadedEndPoints (IEnumerable<DataContainer> unloadedDataContainers)
    {
      var associatedEndPointsOfUnloadedContainers = from dataContainer in unloadedDataContainers
                                                    from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                                                    let associatedEndPoint = GetLoadedEndPoint (associatedEndPointID)
                                                    where associatedEndPoint != null
                                                    select associatedEndPoint;
      
      return (from associatedEndPoint in associatedEndPointsOfUnloadedContainers
              from unloadedEndPoint in GetAndCheckUnloadedEndPoints (associatedEndPoint)
              select unloadedEndPoint).ToArray ();
    }

    private RelationEndPoint GetLoadedEndPoint (RelationEndPointID endPointID)
    {
      var loadedEndPoint = _relationEndPointMap[endPointID];
      Assertion.IsTrue (
          loadedEndPoint != null || endPointID.Definition.IsVirtual, 
          "We can be sure that real end points always exist in the RelationEndPointMap.");
      return loadedEndPoint;
    }

    private IEnumerable<RelationEndPoint> GetAndCheckUnloadedEndPoints (RelationEndPoint endPointOfUnloadedDataContainer)
    {
      // All end-points associated with a DataContainer to be unloaded must be unchanged.
      EnsureUnchanged (endPointOfUnloadedDataContainer.ObjectID, endPointOfUnloadedDataContainer);

      // If it is a real end-point, it must be unloaded. Real end-points cannot exist without their DataContainer.
      var maybeRealEndPoint = Maybe
          .ForValue (endPointOfUnloadedDataContainer)
          .Where (endPoint => !endPoint.Definition.IsVirtual);

      // If it is a virtual object end-point and the opposite object is null, it must be unloaded. Virtual end-points pointing to null cannot exist 
      // without their DataContainer.
      var maybeVirtualNullEndPoint =
          Maybe.ForValue (endPointOfUnloadedDataContainer)
              .Where (endPoint => endPoint.Definition.IsVirtual)
              .Where (endPoint => endPoint.Definition.Cardinality == CardinalityType.One )
              .Where (endPoint => ((ObjectEndPoint) endPoint).OppositeObjectID == null);

      // If it is a real object end-point pointing to a non-null object, and the opposite end-point is loaded, the opposite (virtual) end-point 
      // must be unloaded. Virtual end-points cannot exist without their opposite real end-points.
      // In this case, the unloaded opposite virtual end-point must be unchanged. (This only affects 1:n relations where the virtual end-point can be 
      // changed although the (one of many) real end-point is unchanged. For 1:1 relations, the real and virtual end-points are always changed 
      // together.)
      var maybeOppositeEndPoint =
          Maybe.ForValue (endPointOfUnloadedDataContainer)
              .Where (endPoint => !endPoint.Definition.IsVirtual)
              .Select (endPoint => endPoint as ObjectEndPoint)
              .Where (endPoint => endPoint.OppositeObjectID != null)
              .Select (endPoint => new RelationEndPointID (endPoint.OppositeObjectID, endPoint.Definition.GetOppositeEndPointDefinition ()))
              .Select (oppositeID => _relationEndPointMap[oppositeID]) // only loaded opposite end points!
              .Where (oppositeEndPoint => EnsureUnchanged (endPointOfUnloadedDataContainer.ObjectID, oppositeEndPoint));

      // What's not unloaded:
      // - Virtual object end-points whose opposite object is not null. The life-time of virtual object end-points is defined by the life-time of 
      //   the real end-points (unless they point to null).
      // - Virtual collection end-points. The life-time of virtual collection end-points is defined by the life-time of the full set of its real 
      //   end-points.
      // - Opposite real end-points. The life-time of real end-points is coupled to the life-time of the DataContainer.

      // What must not be changed:
      // - All end-points of the object being unloaded.
      // - The opposite virtual end-point if it is unloaded.
      
      return Maybe.EnumerateValues (
          maybeRealEndPoint,
          maybeVirtualNullEndPoint,
          maybeOppositeEndPoint);
    }

    private bool EnsureUnchanged (ObjectID unloadedObjectID, IEndPoint endPoint)
    {
      if (endPoint.HasChanged)
      {
        var message = String.Format (
            "Object '{0}' cannot be unloaded because one of its relations has been changed. Only unchanged objects can be unloaded. "
            + "Changed end point: '{1}'.",
            unloadedObjectID, 
            endPoint.ID);
        throw new InvalidOperationException (message);
      }

      return true;
    }

    private void UnregisterDataContainers (IEnumerable<DataContainer> dataContainers)
    {
      foreach (var dataContainer in dataContainers)
        _dataContainerMap.Remove (dataContainer.ID);
    }

    private void UnregisterEndPoints (IEnumerable<RelationEndPoint> unloadedEndPoints)
    {
      foreach (var unloadedEndPoint in unloadedEndPoints)
      {
        if (unloadedEndPoint.Definition.Cardinality == CardinalityType.One)
        {
          _relationEndPointMap.RemoveEndPoint (unloadedEndPoint.ID);
        }
        else
        {
          var unloadedCollectionEndPoint = (CollectionEndPoint) unloadedEndPoint;
          unloadedCollectionEndPoint.Unload ();
        }
      }
    }
  }
}