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
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public class PersistenceManager : IDisposable
  {
    private bool _disposed;
    private StorageProviderManager _storageProviderManager;

    public PersistenceManager (IPersistenceListener persistenceListener)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);

      _storageProviderManager = new StorageProviderManager (persistenceListener);
    }

    #region IDisposable Members

    public void Dispose ()
    {
      if (!_disposed)
      {
        if (_storageProviderManager != null)
          _storageProviderManager.Dispose ();

        _storageProviderManager = null;

        _disposed = true;
        GC.SuppressFinalize (this);
      }
    }

    #endregion

    public StorageProviderManager StorageProviderManager
    {
      get { return _storageProviderManager; }
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      StorageProvider provider = _storageProviderManager.GetMandatory (classDefinition.StorageProviderID);
      return provider.CreateNewObjectID (classDefinition);
    }

    public void Save (DataContainerCollection dataContainers)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      if (dataContainers.Count == 0)
        return;

      string storageProviderID = dataContainers[0].ClassDefinition.StorageProviderID;
      foreach (DataContainer dataContainer in dataContainers)
      {
        if (dataContainer.ClassDefinition.StorageProviderID != storageProviderID)
          throw CreatePersistenceException ("Save does not support multiple storage providers.");
      }

      StorageProvider provider = _storageProviderManager.GetMandatory (storageProviderID);

      provider.BeginTransaction ();

      try
      {
        provider.Save (dataContainers);
        provider.SetTimestamp (dataContainers);
        provider.Commit ();
      }
      catch
      {
        try
        {
          provider.Rollback ();
        }
        catch
        {
        }

        throw;
      }
    }

    public DataContainer LoadDataContainer (ObjectID id)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("id", id);

      StorageProvider provider = _storageProviderManager.GetMandatory (id.StorageProviderID);
      DataContainer dataContainer = provider.LoadDataContainer (id);

      Exception exception = CheckLoadedDataContainer(id, dataContainer, true);
      if (exception != null)
        throw exception;

      return dataContainer;
    }

    private Exception CheckLoadedDataContainer (ObjectID id, DataContainer dataContainer, bool throwOnNotFound)
    {
      if (dataContainer != null)
      {
        if (id.ClassID != dataContainer.ID.ClassID)
        {
          return CreatePersistenceException (
              "The ClassID of the provided ObjectID '{0}' and the ClassID of the loaded DataContainer '{1}' differ.",
              id,
              dataContainer.ID);
        }
        else
          return null;
      }
      else if (throwOnNotFound)
        return new ObjectNotFoundException (id);
      else
        return null;
    }

    public DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids, bool throwOnNotFound)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("ids", ids);

      MultiDictionary<string, ObjectID> idsByProvider = GroupIDsByProvider (ids);

      List<Exception> exceptions = new List<Exception>();

      DataContainerCollection unorderedResultCollection = new DataContainerCollection();
      foreach (KeyValuePair<string, List<ObjectID>> idGroup in idsByProvider)
      {
        StorageProvider provider = _storageProviderManager.GetMandatory (idGroup.Key);
        DataContainerCollection dataContainers = provider.LoadDataContainers (idGroup.Value);
        
        foreach (ObjectID id in idGroup.Value)
        {
          DataContainer dataContainer = dataContainers[id];
          Exception exception = CheckLoadedDataContainer (id, dataContainer, throwOnNotFound);
          if (exception != null)
            exceptions.Add (exception);
          else if (dataContainer != null)
            unorderedResultCollection.Add (dataContainer);
        }
      }

      if (exceptions.Count > 0)
        throw new BulkLoadException (exceptions);

      return SortDataContainers(unorderedResultCollection, ids);
    }

    private DataContainerCollection SortDataContainers (DataContainerCollection dataContainers, IEnumerable<ObjectID> orderedIDs)
    {
      DataContainerCollection orderedResultCollection = new DataContainerCollection();
      foreach (ObjectID id in orderedIDs)
      {
        DataContainer dataContainer = dataContainers[id];
        if (dataContainer != null)
          orderedResultCollection.Add (dataContainer);
      }
      return orderedResultCollection;
    }

    private MultiDictionary<string, ObjectID> GroupIDsByProvider (IEnumerable<ObjectID> ids)
    {
      MultiDictionary<string, ObjectID> result = new MultiDictionary<string, ObjectID>();
      foreach (ObjectID id in ids)
        result[id.StorageProviderID].Add (id);
      return result;
    }

    public DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
      {
        throw CreatePersistenceException (
            "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
            + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
            relationEndPointID.Definition.RelationDefinition.ID, relationEndPointID.Definition.PropertyName);
      }

      if (relationEndPointID.Definition.Cardinality == CardinalityType.One)
      {
        throw CreatePersistenceException (
            "Cannot load multiple related data containers for one-to-one relation '{0}'.",
            relationEndPointID.Definition.RelationDefinition.ID);
      }

      var oppositeEndPointDefinition = relationEndPointID.Definition.GetOppositeEndPointDefinition();

      var oppositeProvider = _storageProviderManager.GetMandatory (oppositeEndPointDefinition.ClassDefinition.StorageProviderID);

      var oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
          oppositeEndPointDefinition.ClassDefinition,
          oppositeEndPointDefinition.PropertyName,
          relationEndPointID.ObjectID);

      if (relationEndPointID.Definition.IsMandatory && oppositeDataContainers.Count == 0)
      {
        throw CreatePersistenceException (
            "Collection for mandatory relation '{0}' (property: '{1}', object: '{2}') contains no items.",
            relationEndPointID.Definition.RelationDefinition.ID, relationEndPointID.Definition.PropertyName, relationEndPointID.ObjectID);
      }

      foreach (DataContainer oppositeDataContainer in oppositeDataContainers)
        CheckClassIDForVirtualEndPoint (relationEndPointID, oppositeDataContainer);

      return oppositeDataContainers;
    }

    public DataContainer LoadRelatedDataContainer (DataContainer dataContainer, RelationEndPointID relationEndPointID)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
        return GetOppositeDataContainerForEndPoint (dataContainer, relationEndPointID);

      return GetOppositeDataContainerForVirtualEndPoint (relationEndPointID);
    }

    private DataContainer GetOppositeDataContainerForVirtualEndPoint (RelationEndPointID relationEndPointID)
    {
      if (relationEndPointID.Definition.Cardinality == CardinalityType.Many)
      {
        throw CreatePersistenceException (
            "Cannot load a single related data container for one-to-many relation '{0}'.",
            relationEndPointID.Definition.RelationDefinition.ID);
      }

      StorageProvider oppositeProvider = _storageProviderManager.GetMandatory (
          relationEndPointID.Definition.GetOppositeEndPointDefinition ().ClassDefinition.StorageProviderID);

      DataContainerCollection oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
          relationEndPointID.Definition.GetOppositeEndPointDefinition ().ClassDefinition,
          relationEndPointID.Definition.GetOppositeEndPointDefinition ().PropertyName,
          relationEndPointID.ObjectID);

      if (oppositeDataContainers.Count > 1)
      {
        throw CreatePersistenceException (
            "Multiple related DataContainers where found for property '{0}' of DataContainer '{1}'.",
            relationEndPointID.Definition.PropertyName, relationEndPointID.ObjectID);      
      }

      if (oppositeDataContainers.Count == 0)
        return GetNullDataContainerWithRelationCheck (relationEndPointID);

      CheckClassIDForVirtualEndPoint (relationEndPointID, oppositeDataContainers[0]);
      return oppositeDataContainers[0];
    }

    private DataContainer GetOppositeDataContainerForEndPoint (DataContainer dataContainer, RelationEndPointID relationEndPointID)
    {
      var oppositeID = (ObjectID) dataContainer.PropertyValues[relationEndPointID.Definition.PropertyName].GetValueWithoutEvents (ValueAccess.Current);
      if (oppositeID == null)
        return GetNullDataContainerWithRelationCheck (relationEndPointID);

      StorageProvider oppositeProvider = _storageProviderManager.GetMandatory (oppositeID.StorageProviderID);
      DataContainer oppositeDataContainer = oppositeProvider.LoadDataContainer (oppositeID);
      if (oppositeDataContainer != null)
      {
        CheckClassIDForEndPoint (dataContainer, relationEndPointID, oppositeDataContainer);
      }
      else
      {
        throw CreatePersistenceException (
            "Property '{0}' of object '{1}' refers to non-existing object '{2}'.",
            relationEndPointID.Definition.PropertyName, relationEndPointID.ObjectID, oppositeID);
      }

      return oppositeDataContainer;
    }

    private void CheckClassIDForVirtualEndPoint (
        RelationEndPointID relationEndPointID,
        DataContainer oppositeDataContainer)
    {
      var objectID = (ObjectID) oppositeDataContainer.PropertyValues[relationEndPointID.Definition.GetOppositeEndPointDefinition ().PropertyName].GetValueWithoutEvents (ValueAccess.Current);

      if (relationEndPointID.ObjectID.ClassID != objectID.ClassID)
      {
        throw CreatePersistenceException (
            "The property '{0}' of the loaded DataContainer '{1}'"
            + " refers to ClassID '{2}', but the actual ClassID is '{3}'.",
            relationEndPointID.Definition.GetOppositeEndPointDefinition ().PropertyName,
            oppositeDataContainer.ID,
            objectID.ClassID,
            relationEndPointID.ObjectID.ClassID);
      }
    }

    private void CheckClassIDForEndPoint (
        DataContainer dataContainer,
        RelationEndPointID relationEndPointID,
        DataContainer oppositeDataContainer)
    {
      var id = (ObjectID) dataContainer.PropertyValues[relationEndPointID.Definition.PropertyName].GetValueWithoutEvents(ValueAccess.Current);
      if (id.ClassID != oppositeDataContainer.ID.ClassID)
      {
        throw CreatePersistenceException (
            "The property '{0}' of the provided DataContainer '{1}'"
            + " refers to ClassID '{2}', but the ClassID of the loaded DataContainer is '{3}'.",
            relationEndPointID.Definition.PropertyName,
            dataContainer.ID,
            id.ClassID,
            oppositeDataContainer.ID.ClassID);
      }
    }

    private DataContainer GetNullDataContainerWithRelationCheck (RelationEndPointID relationEndPointID)
    {
      if (relationEndPointID.Definition.IsMandatory)
      {
        throw CreatePersistenceException (
            "Cannot load related DataContainer of object '{0}' over mandatory relation '{1}'.",
            relationEndPointID.ObjectID, relationEndPointID.Definition.RelationDefinition.ID);
      }

      return null;
    }

    private PersistenceException CreatePersistenceException (string message, params object[] args)
    {
      return new PersistenceException (string.Format (message, args));
    }

    private void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException ("PersistenceManager", "A disposed PersistenceManager cannot be accessed.");
    }
  }
}
