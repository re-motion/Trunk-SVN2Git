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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents a transaction that is part of a bigger parent transaction. Any changes made within this subtransaction are not visible in
  /// the parent transaction until the subtransaction is committed, and a commit operation will only commit the changes to the parent transaction, 
  /// not to any storage providers.
  /// </summary>
  /// <remarks>The parent transaction cannot be modified while a subtransaction is active.</remarks>
  [Serializable]
  public class SubPersistenceStrategy : IPersistenceStrategy
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    public SubPersistenceStrategy (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      if (!parentTransaction.IsReadOnly)
      {
        throw new ArgumentException (
            "In order for the subtransaction persistence strategy to work correctly, the parent transaction needs to be read-only. "
            + "Use ClientTransaction.CreateSubTransaction() to create a subtransaction and automatically set the parent transaction read-only.",
            "parentTransaction");
      }

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public IInvalidDomainObjectManager ParentInvalidDomainObjectManager
    {
      get { return _parentInvalidDomainObjectManager; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return _parentTransaction.CreateNewObjectID (classDefinition);
    }

    public virtual ILoadedObjectData LoadObjectData (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        DomainObject parentObject = _parentTransaction.GetObject (id, false);
        return TransferParentObject (parentObject);
      }
    }

    public virtual IEnumerable<ILoadedObjectData> LoadObjectData (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return Enumerable.Empty<ILoadedObjectData>();

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var parentObjects = _parentTransaction.GetObjects<DomainObject> (objectIDs, throwOnNotFound).Where (obj => obj != null);
        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects.Select (parentObject => (ILoadedObjectData) TransferParentObject (parentObject)).ToArray ();
      }
    }

    public virtual ILoadedObjectData ResolveObjectRelationData (
        DataContainer originatingDataContainer, 
        RelationEndPointID relationEndPointID, 
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("ResolveObjectRelationData can only be called for virtual end points.", "relationEndPointID");

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var parentRelatedObject = _parentTransaction.GetRelatedObject (relationEndPointID);

        if (parentRelatedObject == null)
          return new NullLoadedObjectData();
        else
        {
          return TransferParentObject(parentRelatedObject, alreadyLoadedObjectDataProvider);
        }
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (
        RelationEndPointID relationEndPointID, 
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      
      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var parentObjects = _parentTransaction.GetRelatedObjects (relationEndPointID).Cast<DomainObject>();
        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects.Select (parentObject => TransferParentObject (parentObject, alreadyLoadedObjectDataProvider)).ToList ();
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var queryResult = _parentTransaction.QueryManager.GetCollection (query);
        if (queryResult == null)
          throw new InvalidOperationException ("Parent transaction returned an invalid null query result.");

        var parentObjects = queryResult.AsEnumerable();

        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects.Select (parentObject => TransferParentObject (parentObject, alreadyLoadedObjectDataProvider)).ToList ();
      }
    }

    public virtual object ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return _parentTransaction.QueryManager.GetScalar (query);
    }

    private ILoadedObjectData TransferParentObject (DomainObject parentRelatedObject, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      var existingLoadedObject = alreadyLoadedObjectDataProvider.GetLoadedObject (parentRelatedObject.ID);
      if (existingLoadedObject != null)
        return existingLoadedObject;
      else
        return TransferParentObject (parentRelatedObject);
    }

    private FreshlyLoadedObjectData TransferParentObject (DomainObject parentObject)
    {
      var parentDataContainer = _parentTransaction.DataManager.GetDataContainerWithLazyLoad (parentObject.ID);
      var dataContainer = TransferParentContainer (parentDataContainer);
      return new FreshlyLoadedObjectData (dataContainer);
    }

    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      if (parentDataContainer.State == StateType.Deleted)
      {
        var message = string.Format ("Object '{0}' is already deleted in the parent transaction.", parentDataContainer.ID);
        throw new ObjectDeletedException (message, parentDataContainer.ID);
      }

      var thisDataContainer = DataContainer.CreateNew (parentDataContainer.ID);

      thisDataContainer.SetPropertyDataFromSubTransaction (parentDataContainer);
      thisDataContainer.SetTimestamp (parentDataContainer.Timestamp);
      thisDataContainer.CommitState(); // for the new DataContainer, the current parent DC state becomes the Unchanged state

      return thisDataContainer;
    }

    public virtual void PersistData (ReadOnlyCollection<PersistableData> data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      // filter out those items whose state is only Changed due to relation changes
      var dataContainers = data.Select (item => item.DataContainer).Where (dc => dc.State != StateType.Unchanged);

      // only handle changed end-points; end-points of new and deleted objects will implicitly be handled by PersistDataContainers
      var endPoints = data.SelectMany (item => item.GetAssociatedEndPoints()).Where (ep => ep.HasChanged);
      
      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        PersistDataContainers (dataContainers);
        PersistRelationEndPoints (endPoints);
      }
    }

    private void PersistDataContainers (IEnumerable<DataContainer> dataContainers)
    {
      foreach (DataContainer dataContainer in dataContainers)
      {
        Assertion.IsFalse (
            dataContainer.IsDiscarded,
            "dataContainers cannot contain discarded DataContainers, because its items come"
            + "from DataManager.DataContainerMap, which does not contain discarded containers");
        Assertion.IsTrue (dataContainer.State != StateType.Unchanged, "dataContainers cannot contain an unchanged container");
        Assertion.IsTrue (dataContainer.State != StateType.NotLoadedYet, "dataContainers cannot contain an unloaded container");
        Assertion.IsTrue (
            dataContainer.State == StateType.New || dataContainer.State == StateType.Changed
            || dataContainer.State == StateType.Deleted,
            "Invalid dataContainer.State: " + dataContainer.State);

        switch (dataContainer.State)
        {
          case StateType.New:
            PersistNewDataContainer (dataContainer);
            break;
          case StateType.Changed:
            PersistChangedDataContainer (dataContainer);
            break;
          case StateType.Deleted:
            PersistDeletedDataContainer (dataContainer);
            break;
        }
      }
    }

    private void PersistNewDataContainer (DataContainer dataContainer)
    {
      Assertion.IsTrue (_parentInvalidDomainObjectManager.IsInvalid (dataContainer.ID));
      _parentInvalidDomainObjectManager.MarkNotInvalid (dataContainer.ID);

      Assertion.IsNull (GetParentDataContainerWithoutLoading (dataContainer.ID), "a new data container cannot be known to the parent");
      Assertion.IsFalse (dataContainer.IsDiscarded);

      var parentDataContainer = DataContainer.CreateNew (dataContainer.ID);

      parentDataContainer.SetDomainObject (dataContainer.DomainObject);

      _parentTransaction.DataManager.RegisterDataContainer (parentDataContainer);

      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);
      if (dataContainer.HasBeenMarkedChanged)
        parentDataContainer.MarkAsChanged();
      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
    }

    private void PersistChangedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a changed DataContainer must have been loaded through ParentTransaction, so the "
          + "ParentTransaction must know it");
      Assertion.IsFalse (parentDataContainer.IsDiscarded, "a changed DataContainer cannot be discarded in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.State != StateType.Deleted, "a changed DataContainer cannot be deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);

      if (dataContainer.HasBeenMarkedChanged && (parentDataContainer.State == StateType.Unchanged || parentDataContainer.State == StateType.Changed))
        parentDataContainer.MarkAsChanged();
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the ParentTransaction must know it");

      Assertion.IsTrue (
          parentDataContainer.State != StateType.Invalid && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      var deleteCommand = _parentTransaction.DataManager.CreateDeleteCommand (dataContainer.DomainObject);
      deleteCommand.Perform(); // no events, no bidirectional changes
    }

    private DataContainer GetParentDataContainerWithoutLoading (ObjectID id)
    {
      Assertion.IsFalse (_parentInvalidDomainObjectManager.IsInvalid (id), "this method is not called in situations where the ID could be invalid");
      return _parentTransaction.DataManager.GetDataContainerWithoutLoading (id);
    }

    private void PersistRelationEndPoints (IEnumerable<IRelationEndPoint> endPoints)
    {
      foreach (var endPoint in endPoints)
      {
        var parentEndPoint = _parentTransaction.DataManager.GetRelationEndPointWithoutLoading (endPoint.ID);

        // Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of both parent and child transaction now
        // contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known
        // to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.
        // Therefore, we can safely ignore end points unknown to the parent transaction.

        if (parentEndPoint != null)
          parentEndPoint.SetDataFromSubTransaction (endPoint);
      }
    }
  }
}