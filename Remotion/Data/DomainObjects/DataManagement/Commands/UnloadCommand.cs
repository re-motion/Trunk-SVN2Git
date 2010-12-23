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
using Remotion.Collections;
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
    private readonly string[] _unloadProblems;

    private readonly ReadOnlyCollection<DomainObject> _unloadedDomainObjects;

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

      var unloadProblems = new List<string>();

      _unloadedDataContainers = GetAndCheckUnloadedDataContainers (_objectIDs, unloadProblems);
      CheckAffectedEndPoints (_unloadedDataContainers, unloadProblems);

      _unloadProblems = unloadProblems.ToArray();

      _unloadedDomainObjects = ListAdapter.AdaptReadOnly (_unloadedDataContainers, dc => dc.DomainObject);
    }

    public DataContainer[] UnloadedDataContainers
    {
      get { return _unloadedDataContainers; }
    }

    public bool CanUnload
    {
      get { return _unloadProblems.Length == 0; }
    }

    public void EnsureCanUnload ()
    {
      if (!CanUnload)
        throw new InvalidOperationException (_unloadProblems[0]);
    }

    public void NotifyClientTransactionOfBegin ()
    {
      EnsureCanUnload();

      if (_unloadedDomainObjects.Count > 0)
        _clientTransaction.Execute (() => _clientTransaction.TransactionEventSink.ObjectsUnloading (_clientTransaction, _unloadedDomainObjects));
    }

    public void Begin ()
    {
      EnsureCanUnload();

      _clientTransaction.Execute (
          delegate
          {
            for (int i = 0; i < _unloadedDomainObjects.Count; i++)
              _unloadedDomainObjects[i].OnUnloading();
          });
    }

    public void Perform ()
    {
      EnsureCanUnload();

      // UnregisterEndPoints (_unloadedEndPoints);
      UnregisterDataContainers (_unloadedDataContainers);
    }

    public void End ()
    {
      EnsureCanUnload();

      _clientTransaction.Execute (
          delegate
          {
            for (int i = _unloadedDomainObjects.Count - 1; i >= 0; i--)
              _unloadedDomainObjects[i].OnUnloaded();
          });
    }

    public void NotifyClientTransactionOfEnd ()
    {
      EnsureCanUnload();

      if (_unloadedDomainObjects.Count > 0)
        _clientTransaction.Execute (() => _clientTransaction.TransactionEventSink.ObjectsUnloaded (_clientTransaction, _unloadedDomainObjects));
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }

    // The affected DataContainers are all DataContainers to be unloaded. The DataContainers must be unchanged.
    private DataContainer[] GetAndCheckUnloadedDataContainers (IEnumerable<ObjectID> unloadedObjectIDs, ICollection<string> problemAggregator)
    {
      var affectedDataContainers = unloadedObjectIDs.Select (id => _dataContainerMap[id]).Where (dc => dc != null).ToArray();
      var notUnchangedDataContainers = affectedDataContainers.Where (dc => dc.State != StateType.Unchanged);
      if (notUnchangedDataContainers.Any())
      {
        var message =
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + SeparatedStringBuilder.Build (", ", notUnchangedDataContainers, dc => String.Format ("'{0}' ({1})", dc.ID, dc.State))
            + ".";
        problemAggregator.Add (message);
      }

      return affectedDataContainers;
    }

    private void CheckAffectedEndPoints (IEnumerable<DataContainer> unloadedDataContainers, ICollection<string> problemAggregator)
    {
      // All end-points associated with a DataContainer to be unloaded must be unchanged, even if they are not unloaded. This is to avoid "mixed" 
      // States where an object is in state Changed and NotLoadedYet at the same time.
      var changedAssociatedEndPoints = from dataContainer in unloadedDataContainers
                                       from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                                       let associatedEndPoint = GetLoadedEndPoint (associatedEndPointID)
                                       where associatedEndPoint != null && associatedEndPoint.HasChanged
                                       select new { DataContainer = dataContainer, EndPoint = associatedEndPoint };
      
      // These are the "technically required" end-points
      var nonUnregisterableEndPoints = from dataContainer in unloadedDataContainers
                                       from endPoint in _relationEndPointMap.GetNonUnregisterableEndPointsForDataContainer (dataContainer)
                                       select new { DataContainer = dataContainer, EndPoint = endPoint };
      
      foreach (var problematicEndPoint in changedAssociatedEndPoints.Union (nonUnregisterableEndPoints))
      {
        var message = String.Format (
            "Object '{0}' cannot be unloaded because one of its relations has been changed. Only unchanged objects that are not part of changed "
            + "relations can be unloaded." + Environment.NewLine + "Changed relation: '{1}'.",
            problematicEndPoint.DataContainer.ID,
            problematicEndPoint.EndPoint.RelationDefinition.ID);
        problemAggregator.Add (message);
      }
    }

    private RelationEndPoint GetLoadedEndPoint (RelationEndPointID endPointID)
    {
      var loadedEndPoint = _relationEndPointMap[endPointID];
      Assertion.IsTrue (
          loadedEndPoint != null || endPointID.Definition.IsVirtual,
          "We can be sure that real end points always exist in the RelationEndPointMap.");
      return loadedEndPoint;
    }

    private void UnregisterDataContainers (IEnumerable<DataContainer> dataContainers)
    {
      foreach (var dataContainer in dataContainers)
      {
        _relationEndPointMap.UnregisterEndPointsForDataContainer (dataContainer);
        _dataContainerMap.Remove (dataContainer.ID);
      }
    }
  }
}