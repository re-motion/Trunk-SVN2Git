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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Takes <see cref="ILoadedObject"/> instances, registers all freshly loaded ones - triggering the necessary load events - and then returns
  /// the corresponding <see cref="DomainObject"/> instances.
  /// </summary>
  public class LoadedObjectRegistrationAgent : ILoadedObjectRegistrationAgent
  {
    private class RegisteredDataContainerGatheringVisitor : ILoadedObjectVisitor
    {
      private readonly List<Func<DomainObject>> _domainObjectAccessors = new List<Func<DomainObject>> ();
      private readonly List<DataContainer> _dataContainersToBeRegistered = new List<DataContainer> ();

      public void VisitFreshlyLoadedObject (FreshlyLoadedObject freshlyLoadedObject)
      {
        ArgumentUtility.CheckNotNull ("freshlyLoadedObject", freshlyLoadedObject);
      
        _dataContainersToBeRegistered.Add (freshlyLoadedObject.FreshlyLoadedDataContainer);
        _domainObjectAccessors.Add (() => freshlyLoadedObject.FreshlyLoadedDataContainer.DomainObject);
      }

      public void VisitAlreadyExistingLoadedObject (AlreadyExistingLoadedObject alreadyExistingLoadedObject)
      {
        ArgumentUtility.CheckNotNull ("alreadyExistingLoadedObject", alreadyExistingLoadedObject);
        _domainObjectAccessors.Add (() => alreadyExistingLoadedObject.ExistingDataContainer.DomainObject);
      }

      public void VisitNullLoadedObject (NullLoadedObject nullLoadedObject)
      {
        ArgumentUtility.CheckNotNull ("nullLoadedObject", nullLoadedObject);
        _domainObjectAccessors.Add (() => null);
      }

      public void VisitInvalidLoadedObject (InvalidLoadedObject invalidLoadedObject)
      {
        ArgumentUtility.CheckNotNull ("invalidLoadedObject", invalidLoadedObject);
        _domainObjectAccessors.Add (() => invalidLoadedObject.InvalidObjectReference);
      }

      public void RegisterAllDataContainers (IDataManager dataManager, ClientTransaction clientTransaction, IClientTransactionListener transactionEventSink)
      {
        ArgumentUtility.CheckNotNull ("dataManager", dataManager);

        if (_dataContainersToBeRegistered.Count == 0)
          return;

        var objectIDs = ListAdapter.AdaptReadOnly (_dataContainersToBeRegistered, dc => dc.ID);
        transactionEventSink.ObjectsLoading (clientTransaction, objectIDs);

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
          using (clientTransaction.EnterNonDiscardingScope())
          {
            var domainObjects = loadedDomainObjects.AsReadOnly();
            foreach (var domainObject in domainObjects)
              domainObject.OnLoaded();

            transactionEventSink.ObjectsLoaded (clientTransaction, domainObjects);
            clientTransaction.OnLoaded (new ClientTransactionEventArgs (domainObjects));
          }
        }
      }

      public IEnumerable<DomainObject> GetAllDomainObjects ()
      {
        return _domainObjectAccessors.Select(accessor => accessor());
      }
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _transactionEventSink;
    private readonly IDataManager _dataManager;

    public LoadedObjectRegistrationAgent (ClientTransaction clientTransaction, IClientTransactionListener transactionEventSink, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _clientTransaction = clientTransaction;
      _transactionEventSink = transactionEventSink;
      _dataManager = dataManager;
    }

    public DomainObject RegisterIfRequired (ILoadedObject loadedObject)
    {
      ArgumentUtility.CheckNotNull ("loadedObject", loadedObject);

      return RegisterIfRequired (new[] { loadedObject }).Single();
    }

    public IEnumerable<DomainObject> RegisterIfRequired (IEnumerable<ILoadedObject> loadedObjects)
    {
      ArgumentUtility.CheckNotNull ("loadedObjects", loadedObjects);

      var visitor = new RegisteredDataContainerGatheringVisitor ();
      foreach (var loadedObject in loadedObjects)
        loadedObject.Accept (visitor);

      visitor.RegisterAllDataContainers (_dataManager, _clientTransaction, _transactionEventSink);
      return visitor.GetAllDomainObjects ();
    }
  }
}