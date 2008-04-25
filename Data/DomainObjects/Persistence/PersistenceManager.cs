using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public class PersistenceManager : IDisposable
  {
    // types

    // static members and constants

    // member fields

    private bool _disposed = false;
    private StorageProviderManager _storageProviderManager;

    // construction and disposing

    public PersistenceManager ()
    {
      _storageProviderManager = new StorageProviderManager ();
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

    // methods and properties

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

      if (!relationEndPointID.IsVirtual)
      {
        throw CreatePersistenceException (
            "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
            + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
            relationEndPointID.RelationDefinition.ID, relationEndPointID.PropertyName);
      }

      if (relationEndPointID.Definition.Cardinality == CardinalityType.One)
      {
        throw CreatePersistenceException (
            "Cannot load multiple related data containers for one-to-one relation '{0}'.",
            relationEndPointID.RelationDefinition.ID);
      }

      IRelationEndPointDefinition oppositeEndPointDefinition = relationEndPointID.OppositeEndPointDefinition;

      StorageProvider oppositeProvider = _storageProviderManager.GetMandatory (
          oppositeEndPointDefinition.ClassDefinition.StorageProviderID);

      DataContainerCollection oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
          oppositeEndPointDefinition.ClassDefinition,
          oppositeEndPointDefinition.PropertyName,
          relationEndPointID.ObjectID);

      if (relationEndPointID.Definition.IsMandatory && oppositeDataContainers.Count == 0)
      {
        throw CreatePersistenceException (
            "Collection for mandatory relation '{0}' (property: '{1}', object: '{2}') contains no items.",
            relationEndPointID.RelationDefinition.ID, relationEndPointID.PropertyName, relationEndPointID.ObjectID);
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

      if (!relationEndPointID.IsVirtual)
        return GetOppositeDataContainerForEndPoint (dataContainer, relationEndPointID);

      return GetOppositeDataContainerForVirtualEndPoint (relationEndPointID);
    }

    private DataContainer GetOppositeDataContainerForVirtualEndPoint (RelationEndPointID relationEndPointID)
    {
      if (relationEndPointID.Definition.Cardinality == CardinalityType.Many)
      {
        throw CreatePersistenceException (
            "Cannot load a single related data container for one-to-many relation '{0}'.",
            relationEndPointID.RelationDefinition.ID);
      }

      StorageProvider oppositeProvider = _storageProviderManager.GetMandatory (
          relationEndPointID.OppositeEndPointDefinition.ClassDefinition.StorageProviderID);

      DataContainerCollection oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
          relationEndPointID.OppositeEndPointDefinition.ClassDefinition,
          relationEndPointID.OppositeEndPointDefinition.PropertyName,
          relationEndPointID.ObjectID);

      if (oppositeDataContainers.Count > 1)
      {
        throw CreatePersistenceException (
            "Multiple related DataContainers where found for property '{0}' of DataContainer '{1}'.",
            relationEndPointID.PropertyName, relationEndPointID.ObjectID);      
      }

      if (oppositeDataContainers.Count == 0)
        return GetNullDataContainerWithRelationCheck (relationEndPointID);

      CheckClassIDForVirtualEndPoint (relationEndPointID, oppositeDataContainers[0]);
      return oppositeDataContainers[0];
    }

    private DataContainer GetOppositeDataContainerForEndPoint (DataContainer dataContainer, RelationEndPointID relationEndPointID)
    {
      ObjectID oppositeID = (ObjectID) dataContainer.GetFieldValue (relationEndPointID.PropertyName, ValueAccess.Current);
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
            relationEndPointID.PropertyName, relationEndPointID.ObjectID, oppositeID);
      }

      return oppositeDataContainer;
    }

    private void CheckClassIDForVirtualEndPoint (
        RelationEndPointID relationEndPointID,
        DataContainer oppositeDataContainer)
    {
      ObjectID objectID = (ObjectID) oppositeDataContainer.GetFieldValue (relationEndPointID.OppositeEndPointDefinition.PropertyName, ValueAccess.Current);

      if (relationEndPointID.ObjectID.ClassID != objectID.ClassID)
      {
        throw CreatePersistenceException (
            "The property '{0}' of the loaded DataContainer '{1}'"
            + " refers to ClassID '{2}', but the actual ClassID is '{3}'.",
            relationEndPointID.OppositeEndPointDefinition.PropertyName,
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
      ObjectID id = (ObjectID) dataContainer.GetFieldValue (relationEndPointID.PropertyName, ValueAccess.Current);
      if (id.ClassID != oppositeDataContainer.ID.ClassID)
      {
        throw CreatePersistenceException (
            "The property '{0}' of the provided DataContainer '{1}'"
            + " refers to ClassID '{2}', but the ClassID of the loaded DataContainer is '{3}'.",
            relationEndPointID.PropertyName,
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
            relationEndPointID.ObjectID, relationEndPointID.RelationDefinition.ID);
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
