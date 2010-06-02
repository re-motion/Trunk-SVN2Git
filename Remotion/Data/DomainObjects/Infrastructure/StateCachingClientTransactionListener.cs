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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IClientTransactionListener"/> in order to cache the values of <see cref="DomainObject.State"/>.
  /// </summary>
  public class StateCachingClientTransactionListener : ClientTransactionListenerBase
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly Dictionary<ObjectID, StateType> _stateCache = new Dictionary<ObjectID, StateType> ();

    public StateCachingClientTransactionListener (ClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
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

    private StateType CalculateState (ObjectID objectID)
    {
      if (_clientTransaction.DataManager.IsInvalid (objectID))
        return StateType.Invalid;

      var dataContainer = _clientTransaction.DataManager.DataContainerMap[objectID];
      if (dataContainer == null)
        return StateType.NotLoadedYet;

      if (dataContainer.State == StateType.Unchanged)
      {
        // TODO 2806: Inline this, don't use DomainObject but ObjectID
        return _clientTransaction.HasRelationChanged (dataContainer.DomainObject) ? StateType.Changed : StateType.Unchanged;
      }

      return dataContainer.State;
    }

    public override void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer dataContainer, StateType newDataContainerState)
    {
      HandleStateUpdate (dataContainer.ID);
    }

    public override void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      HandleStateUpdate (endPointID.ObjectID);
    }

    public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      HandleStateUpdate (container.ID);
    }

    public override void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      HandleStateUpdate (container.ID);
    }

    private void HandleStateUpdate (ObjectID objectID)
    {
      _stateCache.Remove (objectID);
    }
  }
}