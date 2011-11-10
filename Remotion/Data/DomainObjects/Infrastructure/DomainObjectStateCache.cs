// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IClientTransactionListener"/> in order to cache the values of <see cref="DomainObject.State"/>.
  /// </summary>
  [Serializable]
  public class DomainObjectStateCache
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly Dictionary<ObjectID, StateType> _stateCache = new Dictionary<ObjectID, StateType> ();

    [Serializable]
    private class StateUpdateListener : ClientTransactionListenerBase
    {
      private readonly DomainObjectStateCache _cache;

      public StateUpdateListener (DomainObjectStateCache cache)
      {
        ArgumentUtility.CheckNotNull ("cache", cache);
        _cache = cache;
      }

      public override void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer dataContainer, StateType newDataContainerState)
      {
        _cache.HandleStateUpdate (dataContainer.ID);
      }

      public override void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
      {
        _cache.HandleStateUpdate (endPointID.ObjectID);
      }

      public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
      {
        _cache.HandleStateUpdate (container.ID);
      }

      public override void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
      {
        _cache.HandleStateUpdate (container.ID);
      }
    }

    public DomainObjectStateCache (ClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
      _clientTransaction.AddListener (new StateUpdateListener (this));
    }

    /// <summary>
    /// Gets the <see cref="StateType"/> value for the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> from the cache; recalculating 
    /// it if the cache does not have an up-to-date <see cref="StateType"/> value.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> whose state to get.</param>
    /// <returns>The state of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/>. If no such object has been loaded, 
    /// <see cref="StateType.NotLoadedYet"/> is returned.</returns>
    public StateType GetState (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      
      StateType state;
      if (_stateCache.TryGetValue (objectID, out state))
        return state;

      state = CalculateState (objectID);
      _stateCache.Add (objectID, state);
      return state;
    }

    private void HandleStateUpdate (ObjectID objectID)
    {
      _stateCache.Remove (objectID);
    }

    private StateType CalculateState (ObjectID objectID)
    {
      if (_clientTransaction.IsInvalid (objectID))
        return StateType.Invalid;

      var dataContainer = _clientTransaction.DataManager.DataContainers[objectID];
      if (dataContainer == null)
        return StateType.NotLoadedYet;

      if (dataContainer.State == StateType.Unchanged)
        return _clientTransaction.DataManager.HasRelationChanged (dataContainer) ? StateType.Changed : StateType.Unchanged;

      return dataContainer.State;
    }
  }
}