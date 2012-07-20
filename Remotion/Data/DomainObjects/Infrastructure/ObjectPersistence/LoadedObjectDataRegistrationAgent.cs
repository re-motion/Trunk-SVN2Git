// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Takes <see cref="ILoadedObjectData"/> instances, registers all freshly loaded ones - triggering the necessary load events - and then returns
  /// the corresponding <see cref="DomainObject"/> instances.
  /// </summary>
  [Serializable]
  public class LoadedObjectDataRegistrationAgent : ILoadedObjectDataRegistrationAgent
  {
    private class RegisteredDataContainerGatheringVisitor : ILoadedObjectVisitor
    {
      private readonly List<DataContainer> _dataContainersToBeRegistered = new List<DataContainer> ();
      private readonly List<ObjectID> _notFoundObjectIDs = new List<ObjectID> ();

      public void VisitFreshlyLoadedObject (FreshlyLoadedObjectData freshlyLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull ("freshlyLoadedObjectData", freshlyLoadedObjectData);
        _dataContainersToBeRegistered.Add (freshlyLoadedObjectData.FreshlyLoadedDataContainer);
      }

      public void VisitAlreadyExistingLoadedObject (AlreadyExistingLoadedObjectData alreadyExistingLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull ("alreadyExistingLoadedObjectData", alreadyExistingLoadedObjectData);
      }

      public void VisitNullLoadedObject (NullLoadedObjectData nullLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull ("nullLoadedObjectData", nullLoadedObjectData);
      }

      public void VisitInvalidLoadedObject (InvalidLoadedObjectData invalidLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull ("invalidLoadedObjectData", invalidLoadedObjectData);
      }

      public void VisitNotFoundLoadedObject (NotFoundLoadedObjectData notFoundLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull ("notFoundLoadedObjectData", notFoundLoadedObjectData);
        _notFoundObjectIDs.Add (notFoundLoadedObjectData.ObjectID);
      }

      public void RegisterAllDataContainers (
          ClientTransaction clientTransaction, IDataManager dataManager, IClientTransactionEventSink transactionEventSink, bool throwOnNotFound)
      {
        ArgumentUtility.CheckNotNull ("dataManager", dataManager);
        ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
        ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

        if (_notFoundObjectIDs.Any ())
        {
          transactionEventSink.RaiseEvent ((tx, l) => l.ObjectsNotFound (tx, _notFoundObjectIDs.AsReadOnly()));

          if (throwOnNotFound)
            throw new ObjectsNotFoundException (_notFoundObjectIDs);
        }

        if (_dataContainersToBeRegistered.Count == 0)
          return;

        var objectIDs = ListAdapter.AdaptReadOnly (_dataContainersToBeRegistered, dc => dc.ID);
        transactionEventSink.RaiseEvent ((tx, l) => l.ObjectsLoading (tx, objectIDs));

        var loadedDomainObjects = new List<DomainObject> (_dataContainersToBeRegistered.Count);

        try
        {
          foreach (var dataContainer in _dataContainersToBeRegistered)
          {
            var domainObject = clientTransaction.GetObjectReference (dataContainer.ID);
            dataContainer.SetDomainObject (domainObject);
            dataManager.RegisterDataContainer (dataContainer);
            loadedDomainObjects.Add (domainObject);
          }
        }
        finally
        {
          var domainObjects = loadedDomainObjects.AsReadOnly();
          if (domainObjects.Count > 0)
            transactionEventSink.RaiseEvent ((tx, l) => l.ObjectsLoaded (tx, domainObjects));
        }
      }
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IDataManager _dataManager;
    private readonly IClientTransactionEventSink _transactionEventSink;

    public LoadedObjectDataRegistrationAgent (ClientTransaction clientTransaction, IDataManager dataManager, IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

      _dataManager = dataManager;
      _clientTransaction = clientTransaction;
      _transactionEventSink = transactionEventSink;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public void RegisterIfRequired (ILoadedObjectData loadedObjectData, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("loadedObjectData", loadedObjectData);

      RegisterIfRequired (new[] { loadedObjectData }, throwOnNotFound);
    }

    public void RegisterIfRequired (IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("loadedObjects", loadedObjects);

      var visitor = new RegisteredDataContainerGatheringVisitor ();
      foreach (var loadedObject in loadedObjects)
        loadedObject.Accept (visitor);

      visitor.RegisterAllDataContainers (_clientTransaction, _dataManager, _transactionEventSink, throwOnNotFound);
    }
  }
}