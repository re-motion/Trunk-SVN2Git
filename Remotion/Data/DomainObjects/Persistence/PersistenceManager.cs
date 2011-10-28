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
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public class PersistenceManager : IDisposable
  {
    private bool _disposed;
    private StorageProviderManager _storageProviderManager;

    public PersistenceManager (IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);

      _storageProviderManager = new StorageProviderManager (persistenceExtension);
    }

    #region IDisposable Members

    public void Dispose ()
    {
      if (!_disposed)
      {
        if (_storageProviderManager != null)
          _storageProviderManager.Dispose();

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
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      StorageProvider provider = _storageProviderManager.GetMandatory (classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name);
      return provider.CreateNewObjectID (classDefinition);
    }

    public void Save (DataContainerCollection dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      if (dataContainers.Count == 0)
        return;

      var providerDefinition = dataContainers[0].ClassDefinition.StorageEntityDefinition.StorageProviderDefinition;
      if (dataContainers.Any (dataContainer => dataContainer.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition != providerDefinition))
        throw CreatePersistenceException ("Save does not support multiple storage providers.");

      var provider = _storageProviderManager.GetMandatory (providerDefinition.Name);

      provider.BeginTransaction();

      try
      {
        provider.Save (dataContainers);
        provider.UpdateTimestamps (dataContainers.Where (dc => dc.State != StateType.Deleted));
        provider.Commit();
      }
      catch
      {
        try
        {
          provider.Rollback();
        }
        catch
        {
        }

        throw;
      }
    }

    public DataContainer LoadDataContainer (ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("id", id);

      var provider = _storageProviderManager.GetMandatory (id.StorageProviderDefinition.Name);
      var result = provider.LoadDataContainer (id);

      var exception = CheckLoadedDataContainer (result, true);
      if (exception != null)
        throw exception;

      return result.LocatedObject;
    }

    private Exception CheckLoadedDataContainer (ObjectLookupResult<DataContainer> lookupResult, bool throwOnNotFound)
    {
      if (lookupResult.LocatedObject != null)
      {
        if (lookupResult.ObjectID.ClassID != lookupResult.LocatedObject.ID.ClassID)
        {
          return CreatePersistenceException (
              "The ClassID of the provided ObjectID '{0}' and the ClassID of the loaded DataContainer '{1}' differ.",
              lookupResult.ObjectID,
              lookupResult.LocatedObject.ID);
        }
        else
          return null;
      }
      else if (throwOnNotFound)
        return new ObjectNotFoundException (lookupResult.ObjectID);
      else
        return null;
    }

    public DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids, bool throwOnNotFound)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("ids", ids);

      var idsByProvider = GroupIDsByProvider (ids);
      var exceptions = new List<Exception>();

      var unorderedResultCollection = new DataContainerCollection();
      foreach (var idGroup in idsByProvider)
      {
        var provider = _storageProviderManager.GetMandatory (idGroup.Key);
        foreach (var dataContainerLookupResult in provider.LoadDataContainers (idGroup.Value))
        {
          var exception = CheckLoadedDataContainer (dataContainerLookupResult, throwOnNotFound);
          if (exception != null)
            exceptions.Add (exception);
          else if (dataContainerLookupResult.LocatedObject != null)
            unorderedResultCollection.Add (dataContainerLookupResult.LocatedObject);
        }
      }

      if (exceptions.Count > 0)
        throw new BulkLoadException (exceptions);

      return SortDataContainers (unorderedResultCollection, ids);
    }

    private DataContainerCollection SortDataContainers (DataContainerCollection dataContainers, IEnumerable<ObjectID> orderedIDs)
    {
      var orderedResultCollection = new DataContainerCollection();
      foreach (ObjectID id in orderedIDs)
      {
        DataContainer dataContainer = dataContainers[id];
        if (dataContainer != null)
          orderedResultCollection.Add (dataContainer);
      }
      return orderedResultCollection;
    }

    private IEnumerable<KeyValuePair<string, List<ObjectID>>> GroupIDsByProvider (IEnumerable<ObjectID> ids)
    {
      var result = new MultiDictionary<string, ObjectID>();
      foreach (var id in ids)
        result[id.StorageProviderDefinition.Name].Add (id);
      return result;
    }

    public DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
      {
        throw CreatePersistenceException (
            "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
            + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
            relationEndPointID.Definition.RelationDefinition.ID,
            relationEndPointID.Definition.PropertyName);
      }

      var virtualEndPointDefinition = (VirtualRelationEndPointDefinition) relationEndPointID.Definition;
      if (virtualEndPointDefinition.Cardinality == CardinalityType.One)
      {
        throw CreatePersistenceException (
            "Cannot load multiple related data containers for one-to-one relation '{0}'.",
            virtualEndPointDefinition.RelationDefinition.ID);
      }

      var oppositeDataContainers = LoadOppositeDataContainers (relationEndPointID, virtualEndPointDefinition);

      if (virtualEndPointDefinition.IsMandatory && oppositeDataContainers.Count == 0)
      {
        throw CreatePersistenceException (
            "Collection for mandatory relation '{0}' (property: '{1}', object: '{2}') contains no items.",
            virtualEndPointDefinition.RelationDefinition.ID,
            virtualEndPointDefinition.PropertyName,
            relationEndPointID.ObjectID);
      }

      return oppositeDataContainers;
    }

    public DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("LoadRelatedDataContainer can only be used with virtual end points.", "relationEndPointID");

      return GetOppositeDataContainerForVirtualEndPoint (relationEndPointID);
    }

    private DataContainer GetOppositeDataContainerForVirtualEndPoint (RelationEndPointID relationEndPointID)
    {
      var virtualEndPointDefinition = (VirtualRelationEndPointDefinition) relationEndPointID.Definition;
      if (virtualEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        throw CreatePersistenceException (
            "Cannot load a single related data container for one-to-many relation '{0}'.",
            virtualEndPointDefinition.RelationDefinition.ID);
      }

      var oppositeDataContainers = LoadOppositeDataContainers (relationEndPointID, virtualEndPointDefinition);
      if (oppositeDataContainers.Count > 1)
      {
        throw CreatePersistenceException (
            "Multiple related DataContainers where found for property '{0}' of DataContainer '{1}'.",
            virtualEndPointDefinition.PropertyName,
            relationEndPointID.ObjectID);
      }

      if (oppositeDataContainers.Count == 0)
        return GetNullDataContainerWithRelationCheck (relationEndPointID);

      return oppositeDataContainers[0];
    }

    private DataContainerCollection LoadOppositeDataContainers (
        RelationEndPointID relationEndPointID, VirtualRelationEndPointDefinition virtualEndPointDefinition)
    {
      var oppositeEndPointDefinition = virtualEndPointDefinition.GetOppositeEndPointDefinition();
      var oppositeProvider =
          _storageProviderManager.GetMandatory (oppositeEndPointDefinition.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name);

      var oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) oppositeEndPointDefinition,
          virtualEndPointDefinition.GetSortExpression(),
          relationEndPointID.ObjectID);

      var oppositeDataContainerCollection = new DataContainerCollection();
      foreach (var oppositeDataContainer in oppositeDataContainers)
      {
        CheckClassIDForVirtualEndPoint (relationEndPointID, oppositeDataContainer);
        oppositeDataContainerCollection.Add (oppositeDataContainer);
      }
      return oppositeDataContainerCollection;
    }

    private void CheckClassIDForVirtualEndPoint (
        RelationEndPointID relationEndPointID,
        DataContainer oppositeDataContainer)
    {
      var objectID =
          (ObjectID)
          oppositeDataContainer.PropertyValues[relationEndPointID.Definition.GetOppositeEndPointDefinition().PropertyName].GetValueWithoutEvents (
              ValueAccess.Current);

      if (relationEndPointID.ObjectID.ClassID != objectID.ClassID)
      {
        throw CreatePersistenceException (
            "The property '{0}' of the loaded DataContainer '{1}'"
            + " refers to ClassID '{2}', but the actual ClassID is '{3}'.",
            relationEndPointID.Definition.GetOppositeEndPointDefinition().PropertyName,
            oppositeDataContainer.ID,
            objectID.ClassID,
            relationEndPointID.ObjectID.ClassID);
      }
    }

    private DataContainer GetNullDataContainerWithRelationCheck (RelationEndPointID relationEndPointID)
    {
      if (relationEndPointID.Definition.IsMandatory)
      {
        throw CreatePersistenceException (
            "Cannot load related DataContainer of object '{0}' over mandatory relation '{1}'.",
            relationEndPointID.ObjectID,
            relationEndPointID.Definition.RelationDefinition.ID);
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