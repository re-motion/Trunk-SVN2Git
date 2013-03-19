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
using System.Collections.ObjectModel;
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

      public ReadOnlyCollection<ObjectID> NotFoundObjectIDs
      {
        get { return _notFoundObjectIDs.AsReadOnly(); }
      }

      public ReadOnlyCollection<DataContainer> DataContainersToBeRegistered
      {
        get { return _dataContainersToBeRegistered.AsReadOnly (); }
      }

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
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IDataManager _dataManager;
    private readonly ILoadedObjectDataRegistrationListener _registrationListener;

    public LoadedObjectDataRegistrationAgent (
        ClientTransaction clientTransaction,
        IDataManager dataManager,
        ILoadedObjectDataRegistrationListener registrationListener)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("registrationListener", registrationListener);

      _dataManager = dataManager;
      _clientTransaction = clientTransaction;
      _registrationListener = registrationListener;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public ILoadedObjectDataRegistrationListener RegistrationListener
    {
      get { return _registrationListener; }
    }

    public void RegisterIfRequired (IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("loadedObjects", loadedObjects);

      var visitor = new RegisteredDataContainerGatheringVisitor ();
      foreach (var loadedObject in loadedObjects)
        loadedObject.Accept (visitor);

      if (visitor.NotFoundObjectIDs.Any ())
      {
        _registrationListener.OnObjectsNotFound (visitor.NotFoundObjectIDs);

        if (throwOnNotFound)
          throw new ObjectsNotFoundException (visitor.NotFoundObjectIDs);
      }

      PrepareDataContainers (visitor.DataContainersToBeRegistered);

      // TODO 5397: Split operation here.
      RegisterPreparedDataContainers (visitor.DataContainersToBeRegistered);
    }

    private void PrepareDataContainers (IList<DataContainer> dataContainersToBeRegistered)
    {
      if (dataContainersToBeRegistered.Count == 0)
        return;
      
      // Note: After this event, OnAfterObjectRegistration _must_ be raised for the same ObjectIDs! Otherwise, we'll leak "objects currently loading".
      var objectIDs = ListAdapter.AdaptReadOnly (dataContainersToBeRegistered, dc => dc.ID);
      _registrationListener.OnBeforeObjectRegistration (objectIDs);

      foreach (var dataContainer in dataContainersToBeRegistered)
      {
        var domainObject = _clientTransaction.GetObjectReference (dataContainer.ID);
        dataContainer.SetDomainObject (domainObject);
      }
    }

    private void RegisterPreparedDataContainers (IList<DataContainer> dataContainersToBeRegistered)
    {
      if (dataContainersToBeRegistered.Count == 0)
        return;

      var objectIDs = ListAdapter.AdaptReadOnly (dataContainersToBeRegistered, dc => dc.ID);
      var loadedDomainObjects = new List<DomainObject> (dataContainersToBeRegistered.Count);

      try
      {
        foreach (var dataContainer in dataContainersToBeRegistered)
        {
          Assertion.IsTrue (dataContainer.HasDomainObject);

          _dataManager.RegisterDataContainer (dataContainer);
          loadedDomainObjects.Add (dataContainer.DomainObject);
        }
      }
      finally
      {
        _registrationListener.OnAfterObjectRegistration (objectIDs, loadedDomainObjects.AsReadOnly ());
      }
    }
  }
}