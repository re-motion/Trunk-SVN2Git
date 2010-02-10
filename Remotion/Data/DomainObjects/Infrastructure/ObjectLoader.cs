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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements the mechanisms for loading a set of <see cref="DomainObject"/> objects into a <see cref="ClientTransaction"/>.
  /// This class should only be used by <see cref="ClientTransaction"/> and its subclasses.
  /// </summary>
  [Serializable]
  public class ObjectLoader : IObjectLoader
  {
    private readonly IDataSource _dataSource;
    private readonly ClientTransaction _clientTransaction
      ;
    private readonly IClientTransactionListener _eventSink;

    public ObjectLoader (ClientTransaction clientTransaction, IDataSource dataSource, IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      _dataSource = dataSource;
      _clientTransaction = clientTransaction;
      _eventSink = eventSink;
    }

    public DomainObject LoadObject (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var dataContainer = _dataSource.LoadDataContainer (id);
      RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (new[] { id }));

      InitializeLoadedDataContainer (dataContainer);

      var loadedDomainObject = dataContainer.DomainObject;
      RaiseLoadedNotifications (new ReadOnlyCollection<DomainObject> (new[] { loadedDomainObject }));

      return loadedDomainObject;
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);

      var dataContainers = _dataSource.LoadDataContainers (idsToBeLoaded, throwOnNotFound);
      RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (idsToBeLoaded));

      foreach (DataContainer dataContainer in dataContainers)
        InitializeLoadedDataContainer (dataContainer);

      var loadedDomainObjectsWithoutNulls = dataContainers.Cast<DataContainer> ().Select (dc => dc.DomainObject).ToList ();
      RaiseLoadedNotifications (new ReadOnlyCollection<DomainObject> (loadedDomainObjectsWithoutNulls));

      var loadedDomainObjects = (from id in idsToBeLoaded
                                 let dataContainer = dataContainers[id]
                                 select dataContainer != null ? dataContainer.DomainObject : null).ToArray ();
      return loadedDomainObjects;
    }

    public DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("LoadRelatedObject can only be used with virtual end points.", "relationEndPointID");

      DataContainer relatedDataContainer = _dataSource.LoadRelatedDataContainer (relationEndPointID);

      if (relatedDataContainer != null)
      {
        RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (new[] { relatedDataContainer.ID }));

        InitializeLoadedDataContainer (relatedDataContainer);

        var loadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { relatedDataContainer.DomainObject });
        RaiseLoadedNotifications (loadedDomainObjects);

        return relatedDataContainer.DomainObject;
      }
      else
      {
        _clientTransaction.DataManager.RelationEndPointMap.RegisterVirtualObjectEndPoint (relationEndPointID, null);
        return null;
      }
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      var relatedDataContainers = _dataSource.LoadRelatedDataContainers (relationEndPointID).Cast<DataContainer> ();

      FindNewDataContainersAndInitialize (relatedDataContainers);

      var relatedObjects = from loadedDataContainer in relatedDataContainers
                           let relatedID = loadedDataContainer.ID
                           let registeredDataContainer = Assertion.IsNotNull (_clientTransaction.DataManager.DataContainerMap[relatedID])
                           select registeredDataContainer.DomainObject;
      return relatedObjects.ToArray ();
    }

    private void RaiseLoadingNotificiations (ReadOnlyCollection<ObjectID> objectIDs)
    {
      if (objectIDs.Count != 0)
        _eventSink.ObjectsLoading (objectIDs);
    }

    private void RaiseLoadedNotifications (ReadOnlyCollection<DomainObject> loadedObjects)
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        if (loadedObjects.Count != 0)
        {
          foreach (var loadedDomainObject in loadedObjects)
            loadedDomainObject.OnLoaded ();

          _eventSink.ObjectsLoaded (loadedObjects);
          _clientTransaction.OnLoaded (new ClientTransactionEventArgs (loadedObjects));
        }
      }
    }

    public void FindNewDataContainersAndInitialize (IEnumerable<DataContainer> dataContainers)
    {
      var newlyLoadedDataContainers = (from dataContainer in dataContainers
                                       where dataContainer != null && _clientTransaction.DataManager.DataContainerMap[dataContainer.ID] == null
                                       select dataContainer).ToList ();

      RaiseLoadingNotificiations (newlyLoadedDataContainers.Select (dc => dc.ID).ToList ().AsReadOnly ());

      foreach (var dataContainer in newlyLoadedDataContainers)
        InitializeLoadedDataContainer (dataContainer);

      var newlyLoadedDomainObjects = from dataContainer in newlyLoadedDataContainers
                                     select dataContainer.DomainObject;
      RaiseLoadedNotifications (newlyLoadedDomainObjects.ToList ().AsReadOnly ());
    }

    private void InitializeLoadedDataContainer (DataContainer dataContainer)
    {
      var domainObjectReference = _clientTransaction.GetObjectReference (dataContainer.ID);

      dataContainer.RegisterWithTransaction (_clientTransaction);
      dataContainer.SetDomainObject (domainObjectReference);

      Assertion.IsTrue (dataContainer.DomainObject.ID == dataContainer.ID);
      Assertion.IsTrue (dataContainer.ClientTransaction == _clientTransaction);
      Assertion.IsTrue (_clientTransaction.DataManager.DataContainerMap[dataContainer.ID] == dataContainer);
    }
  }
}