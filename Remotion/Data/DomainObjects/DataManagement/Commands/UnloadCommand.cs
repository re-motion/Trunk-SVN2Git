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

    private readonly DataContainer[] _affectedDataContainers;
    private readonly ReadOnlyCollection<DomainObject> _affectedDomainObjects;

    private readonly RelationEndPointID[] _affectedEndPointIDs;

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
      
      _affectedDataContainers = GetAndCheckAffectedDataContainers ();
      _affectedDomainObjects = _affectedDataContainers.Select (dc => dc.DomainObject).ToList ().AsReadOnly ();

      _affectedEndPointIDs = (from dataContainer in _affectedDataContainers
                              from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                              from unloadedEndPointID in GetAndCheckAffectedEndPointIDs (associatedEndPointID)
                              select unloadedEndPointID).ToArray();
    }

    public DataContainer[] AffectedDataContainers
    {
      get { return _affectedDataContainers; }
    }

    public RelationEndPointID[] AffectedEndPointIDs
    {
      get { return _affectedEndPointIDs; }
    }

    public void NotifyClientTransactionOfBegin ()
    {
      if (_affectedDomainObjects.Count > 0)
        _clientTransaction.TransactionEventSink.ObjectsUnloading (_affectedDomainObjects);
    }

    public void Begin ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        for (int i = 0; i < _affectedDomainObjects.Count; i++)
          _affectedDomainObjects[i].OnUnloading();
      }
    }

    public void Perform ()
    {
      UnregisterEndPoints (_affectedEndPointIDs);
      UnregisterDataContainers (_affectedDataContainers);
    }

    public void End ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        for (int i = _affectedDomainObjects.Count - 1; i >= 0; i--)
          _affectedDomainObjects[i].OnUnloaded();
      }
    }

    public void NotifyClientTransactionOfEnd ()
    {
      if (_affectedDomainObjects.Count > 0)
        _clientTransaction.TransactionEventSink.ObjectsUnloaded (_affectedDomainObjects);
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }

    private DataContainer[] GetAndCheckAffectedDataContainers ()
    {
      var affectedDataContainers = _objectIDs.Select (id => _dataContainerMap[id]).Where (dc => dc != null).ToArray();
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

    private IEnumerable<RelationEndPointID> GetAndCheckAffectedEndPointIDs (RelationEndPointID endPointID)
    {
      Assertion.IsFalse (
          !endPointID.Definition.IsVirtual && !_relationEndPointMap.Contains (endPointID), 
          "Real end point is always registered when data container exists.");

      var maybeLoadedEndPoint = Maybe.ForValue (_relationEndPointMap[endPointID])
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
              .Select (oppositeID => _relationEndPointMap[oppositeID]) // only loaded opposite end points!
              .Where (oppositeEndPoint => EnsureUnchanged (endPointID.ObjectID, oppositeEndPoint))
              .Select (oppositeEndPoint => oppositeEndPoint.ID);
      
      return Maybe.EnumerateValues (
          maybeRealEndPointID,
          maybeVirtualNullEndPointID,
          maybeOppositeEndPointID);
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

    private void UnregisterEndPoints (IEnumerable<RelationEndPointID> unloadedEndPointIDs)
    {
      foreach (var unloadedEndPointID in unloadedEndPointIDs)
      {
        if (unloadedEndPointID.Definition.Cardinality == CardinalityType.One)
        {
          _relationEndPointMap.RemoveEndPoint (unloadedEndPointID);
        }
        else
        {
          var unloadedCollectionEndPoint = (CollectionEndPoint) _relationEndPointMap[unloadedEndPointID];
          unloadedCollectionEndPoint.Unload ();
        }
      }
    }
  }
}