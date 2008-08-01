/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointMap : ICollectionEndPointChangeDelegate, IEnumerable, IFlattenedSerializable
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _transactionEventSink;
    private readonly RelationEndPointCollection _relationEndPoints;

    // construction and disposing

    public RelationEndPointMap (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);


      _clientTransaction = clientTransaction;
      _transactionEventSink = clientTransaction.TransactionEventSink;
      _relationEndPoints = new RelationEndPointCollection (_clientTransaction);
    }

    // methods and properties

    public RelationEndPoint this [RelationEndPointID endPointID]
    {
      get { return _relationEndPoints[endPointID]; }
    }

    public int Count
    {
      get { return _relationEndPoints.Count; }
    }

    public void Commit (DomainObjectCollection deletedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("deletedDomainObjects", deletedDomainObjects);

      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Commit();

      foreach (DomainObject deletedDomainObject in deletedDomainObjects)
      {
        foreach (RelationEndPointID endPointID in deletedDomainObject.GetDataContainerForTransaction (_clientTransaction).RelationEndPointIDs)
          Remove (endPointID);
      }
    }

    public void Rollback (DomainObjectCollection newDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("newDomainObjects", newDomainObjects);

      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Rollback();

      foreach (DomainObject newDomainObject in newDomainObjects)
      {
        foreach (RelationEndPointID endPointID in newDomainObject.GetDataContainerForTransaction (_clientTransaction).RelationEndPointIDs)
          Remove (endPointID);
      }
    }

    public void PerformDelete (DomainObject domainObject, RelationEndPointModificationCollection oppositeEndPointModifications)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransactionForDeletion (domainObject);
      RelationEndPointID[] relationEndPointIDs = domainObject.GetDataContainerForTransaction (_clientTransaction).RelationEndPointIDs;
      _transactionEventSink.RelationEndPointMapPerformingDelete (relationEndPointIDs);

      foreach (RelationEndPointID endPointID in relationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

        oppositeEndPointModifications.Perform();
        endPoint.PerformDelete();

        if (domainObject.GetStateForTransaction (_clientTransaction) == StateType.New)
          Remove (endPointID);
      }
    }

    public RelationEndPointModificationCollection GetOppositeEndPointModificationsForDelete (DomainObject domainObject)
    {
      RelationEndPointCollection allAffectedRelationEndPoints = GetAllRelationEndPointsWithLazyLoad (domainObject);
      RelationEndPointCollection allOppositeRelationEndPoints = allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);

      RelationEndPointModificationCollection modifications = new RelationEndPointModificationCollection();
      foreach (RelationEndPoint oppositeEndPoint in allOppositeRelationEndPoints)
      {
        IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
        RelationEndPoint oldEndPoint = allAffectedRelationEndPoints[new RelationEndPointID (domainObject.ID, endPointDefinition)];

        modifications.Add (oppositeEndPoint.CreateModification (oldEndPoint));
      }
      return modifications;
    }

    public DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "GetRelatedObject", "endPointID");

      ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
      if (objectEndPoint == null) // the relation hasn't been loaded yet
        return _clientTransaction.LoadRelatedObject (endPointID);
      else if (objectEndPoint.OppositeObjectID == null) // the relation points to a null value
        return null;
      else if (includeDeleted && _clientTransaction.DataManager.IsDiscarded (objectEndPoint.OppositeObjectID))
          // the relation points to a discarded value
        return _clientTransaction.DataManager.GetDiscardedDataContainer (objectEndPoint.OppositeObjectID).DomainObject;
      else // the relation points to an ordinary, known object
        return _clientTransaction.GetObject (objectEndPoint.OppositeObjectID, includeDeleted);
    }

    public DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "GetOriginalRelatedObject", "endPointID");

      ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
      if (objectEndPoint == null)
        return _clientTransaction.LoadRelatedObject (endPointID);

      if (objectEndPoint.OriginalOppositeObjectID == null)
        return null;

      return _clientTransaction.GetObject (objectEndPoint.OriginalOppositeObjectID, true);
    }

    public DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetRelatedObjects", "endPointID");

      CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
      if (collectionEndPoint == null)
        return _clientTransaction.LoadRelatedObjects (endPointID);

      return collectionEndPoint.OppositeDomainObjects;
    }

    public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetOriginalRelatedObjects", "endPointID");

      CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
      if (collectionEndPoint == null)
      {
        _clientTransaction.LoadRelatedObjects (endPointID);
        collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
      }

      return collectionEndPoint.OriginalOppositeDomainObjects;
    }

    public void SetRelatedObject (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "SetRelatedObject", "endPointID");
      CheckClientTransactionForObjectEndPoint (endPointID, newRelatedObject);
      CheckDeleted (newRelatedObject);
      CheckType (endPointID, newRelatedObject);

      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);
      CheckDeleted (endPoint);

      if (endPoint.OppositeEndPointDefinition.IsNull)
        SetRelatedObjectForUnidirectionalRelation (endPoint, newRelatedObject);
      else
        SetRelatedObjectForBidirectionalRelation (endPoint, newRelatedObject);
    }

    public void RegisterObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ObjectEndPoint objectEndPoint = new ObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      objectEndPoint.RegisterWithMap (this);
      Add (objectEndPoint);
    }

    public void RegisterCollectionEndPoint (RelationEndPointID endPointID, DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      CollectionEndPoint collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, domainObjects);
      collectionEndPoint.RegisterWithMap (this);
      Add (collectionEndPoint);
    }

    public void RegisterExistingDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      foreach (RelationDefinition relationDefinition in classDefinition.GetRelationDefinitions())
      {
        foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
        {
          if (!endPointDefinition.IsVirtual)
          {
            if (classDefinition.IsRelationEndPoint (endPointDefinition))
            {
              ObjectID oppositeObjectID = (ObjectID) dataContainer.GetFieldValue (endPointDefinition.PropertyName, ValueAccess.Current);
              ObjectEndPoint endPoint = new ObjectEndPoint (dataContainer.ClientTransaction, dataContainer.ID, endPointDefinition, oppositeObjectID);
              Add (endPoint);

              if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
              {
                ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                    _clientTransaction,
                    endPoint.OppositeObjectID,
                    endPoint.OppositeEndPointDefinition,
                    endPoint.ObjectID);

                Add (oppositeEndPoint);
              }
            }
          }
        }
      }
    }

    public void RegisterNewDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
      {
        if (endPointID.Definition.Cardinality == CardinalityType.One)
          RegisterObjectEndPoint (endPointID, null);
        else
        {
          DomainObjectCollection domainObjects = DomainObjectCollection.Create (
              endPointID.Definition.PropertyType, endPointID.OppositeEndPointDefinition.ClassDefinition.ClassType);

          RegisterCollectionEndPoint (endPointID, domainObjects);
        }
      }
    }

    public RelationEndPointCollection GetAllRelationEndPointsWithLazyLoad (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointCollection allRelationEndPoints = new RelationEndPointCollection (_clientTransaction);

      foreach (RelationEndPointID endPointID in domainObject.GetDataContainerForTransaction (_clientTransaction).RelationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

        if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.Many && !endPoint.OppositeEndPointDefinition.IsNull)
        {
          ObjectEndPoint objectEndPoint = (ObjectEndPoint) endPoint;
          if (objectEndPoint.OppositeObjectID != null)
          {
            RelationEndPointID oppositeEndPointID =
                new RelationEndPointID (objectEndPoint.OppositeObjectID, objectEndPoint.OppositeEndPointDefinition);

            GetRelatedObjects (oppositeEndPointID);
          }
        }

        allRelationEndPoints.Add (endPoint);

        allRelationEndPoints.Combine (_relationEndPoints.GetOppositeRelationEndPoints (endPoint));
      }

      return allRelationEndPoints;
    }

    public void CheckMandatoryRelations (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (RelationEndPointID endPointID in domainObject.GetDataContainerForTransaction (_clientTransaction).RelationEndPointIDs)
      {
        if (endPointID.Definition.IsMandatory)
        {
          RelationEndPoint endPoint = _relationEndPoints[endPointID];
          if (endPoint != null)
            endPoint.CheckMandatory();
        }
      }
    }

    public bool HasRelationChanged (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
      {
        RelationEndPoint endPoint = _relationEndPoints[endPointID];
        if (endPoint != null && endPoint.HasChanged)
          return true;
      }

      return false;
    }

    private void SetRelatedObjectForUnidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = GetRelatedObject (endPoint.ID, true);

      AnonymousEndPoint newRelatedEndPoint = GetAnonymousEndPoint (newRelatedObject, endPoint.RelationDefinition);
      AnonymousEndPoint oldRelatedEndPoint = GetAnonymousEndPoint (oldRelatedObject, endPoint.RelationDefinition);

      if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject(), oldRelatedEndPoint.GetDomainObject()))
        SetRelatedObjectForEqualObjects (endPoint, null);
      else
      {
        RelationEndPointModification modification = endPoint.CreateModification (oldRelatedEndPoint, newRelatedEndPoint);
        modification.ExecuteAllSteps();
      }
    }

    private void SetRelatedObjectForEqualObjects (RelationEndPoint endPoint, RelationEndPoint oppositeEndPoint)
    {
      Assertion.IsNotNull (endPoint);

      RelationEndPoint realEndPoint = endPoint.Definition.IsVirtual ? oppositeEndPoint : endPoint;
      RelationEndPoint virtualEndPoint = endPoint.Definition.IsVirtual ? endPoint : oppositeEndPoint;

      Assertion.IsNotNull (realEndPoint);
      realEndPoint.Touch();

      if (virtualEndPoint != null) // bidirectional?
        virtualEndPoint.Touch();

      // touch foreign key property
      if (!realEndPoint.IsNull) // null end points have no data container, so no foreign key needs to be touched
        realEndPoint.GetDataContainer().PropertyValues[realEndPoint.PropertyName].Touch();
    }

    private void SetRelatedObjectForBidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = GetRelatedObject (endPoint.ID, false);

      RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (newRelatedObject, endPoint.OppositeEndPointDefinition);
      RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (oldRelatedObject, newRelatedEndPoint.Definition);

      if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject(), oldRelatedEndPoint.GetDomainObject()))
        SetRelatedObjectForEqualObjects (endPoint, newRelatedEndPoint);
      else if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
        SetRelatedObjectForOneToOneRelation ((ObjectEndPoint) endPoint, (ObjectEndPoint) newRelatedEndPoint, (ObjectEndPoint) oldRelatedEndPoint);
      else
        SetRelatedObjectForOneToManyRelation ((ObjectEndPoint) endPoint, newRelatedEndPoint, oldRelatedEndPoint);
    }

    private void CheckDeleted (RelationEndPoint endPoint)
    {
      CheckDeleted (endPoint.GetDomainObject());
    }

    private void CheckDeleted (DomainObject domainObject)
    {
      if (domainObject != null && domainObject.GetStateForTransaction (_clientTransaction) == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }

    // TODO refactor: Unify SetRelatedObject*-methods to one single method => add *RelationChange-methods to IEndPoint
    private void SetRelatedObjectForOneToOneRelation (
        ObjectEndPoint endPoint,
        ObjectEndPoint newRelatedEndPoint,
        ObjectEndPoint oldRelatedEndPoint)
    {
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint)
          RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition);

      if (!newRelatedEndPoint.IsNull)
      {
        DomainObject oldRelatedObject = GetRelatedObject (newRelatedEndPoint.ID, false);
        oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) GetRelationEndPoint (oldRelatedObject, endPoint.Definition);
      }

      RelationEndPointModificationCollection modifications = new RelationEndPointModificationCollection (
          endPoint.CreateModification (oldRelatedEndPoint, newRelatedEndPoint),
          oldRelatedEndPoint.CreateModification (endPoint),
          newRelatedEndPoint.CreateModification (oldRelatedEndPointOfNewRelatedEndPoint, endPoint),
          oldRelatedEndPointOfNewRelatedEndPoint.CreateModification (newRelatedEndPoint));

      modifications.ExecuteAllSteps();
    }

    private void SetRelatedObjectForOneToManyRelation (
        ObjectEndPoint endPoint,
        RelationEndPoint newRelatedEndPoint,
        RelationEndPoint oldRelatedEndPoint)
    {
      if (!newRelatedEndPoint.IsNull)
      {
        DomainObjectCollection collection = GetRelatedObjects (newRelatedEndPoint.ID);
        collection.Add (endPoint.GetDomainObject());
      }
      else
      {
        DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint.ID);
        collection.Remove (endPoint.GetDomainObject());
      }
    }

    private AnonymousEndPoint GetAnonymousEndPoint (DomainObject domainObject, RelationDefinition relationDefinition)
    {
      if (domainObject != null)
        return new AnonymousEndPoint (_clientTransaction, domainObject, relationDefinition);
      else
        return new NullAnonymousEndPoint (relationDefinition);
    }

    private RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      if (domainObject != null)
        return GetRelationEndPointWithLazyLoad (new RelationEndPointID (domainObject.ID, definition));
      else
        return RelationEndPoint.CreateNullRelationEndPoint (definition);
    }

    public RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      if (_relationEndPoints.Contains (endPointID))
        return _relationEndPoints[endPointID];

      if (endPointID.Definition.Cardinality == CardinalityType.One)
        _clientTransaction.LoadRelatedObject (endPointID);
      else
        _clientTransaction.LoadRelatedObjects (endPointID);

      return _relationEndPoints[endPointID];
    }

    private void Add (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      _transactionEventSink.RelationEndPointMapRegistering (endPoint);
      if (endPoint.IsNull)
        throw new ArgumentNullException ("endPoint", "A NullRelationEndPoint cannot be added to a RelationEndPointMap.");

      _relationEndPoints.Add (endPoint);
    }

    private void Remove (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      _transactionEventSink.RelationEndPointMapUnregistering (endPointID);
      _relationEndPoints.Remove (endPointID);
    }

    private void CheckCardinality (
        RelationEndPointID endPointID,
        CardinalityType expectedCardinality,
        string methodName,
        string argumentName)
    {
      if (endPointID.Definition.Cardinality != expectedCardinality)
      {
        throw CreateArgumentException (
            argumentName,
            "{0} can only be called for end points with a cardinality of '{1}'.",
            methodName,
            expectedCardinality);
      }
    }

    private void CheckClientTransactionForInsertionIntoCollectionEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject, int index)
    {
      if (newRelatedObject != null && !newRelatedObject.CanBeUsedInTransaction (_clientTransaction))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot insert DomainObject '{0}' at position {1} into collection of property '{2}' of DomainObject '{3}',"
                + " because the objects do not belong to the same ClientTransaction.",
            newRelatedObject.ID,
            index,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private void CheckClientTransactionForRemovalFromCollectionEndPoint (RelationEndPointID endPointID, DomainObject relatedObject)
    {
      if (relatedObject != null && !relatedObject.CanBeUsedInTransaction (_clientTransaction))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}',"
                + " because the objects do not belong to the same ClientTransaction.",
            relatedObject.ID,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private void CheckClientTransactionForObjectEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      if (newRelatedObject != null && !newRelatedObject.CanBeUsedInTransaction (_clientTransaction))
      {
        throw CreateClientTransactionsDifferException (
            "Property '{0}' of DomainObject '{1}' cannot be set to DomainObject '{2}',"
                + " because the objects do not belong to the same ClientTransaction.",
            endPointID.PropertyName,
            endPointID.ObjectID,
            newRelatedObject.ID);
      }
    }

    private void CheckClientTransactionForReplacementInCollectionEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject, int index)
    {
      if (newRelatedObject != null && !newRelatedObject.CanBeUsedInTransaction (_clientTransaction))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot replace DomainObject at position {0} with DomainObject '{1}'"
                + " in collection of property '{2}' of DomainObject '{3}',"
                    + " because the objects do not belong to the same ClientTransaction.",
            index,
            newRelatedObject.ID,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private void CheckClientTransactionForDeletion (DomainObject domainObject)
    {
      if (!domainObject.CanBeUsedInTransaction (_clientTransaction))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot remove DomainObject '{0}' from RelationEndPointMap, because it belongs to a different ClientTransaction.",
            domainObject.ID);
      }
    }

    private void CheckType (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      if (newRelatedObject == null)
        return;

      if (!endPointID.OppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (newRelatedObject.ID.ClassDefinition))
      {
        throw CreateDataManagementException (
            "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
                + " because it is not compatible with the type of the property.",
            newRelatedObject.ID,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private DataManagementException CreateDataManagementException (string message, params object[] args)
    {
      return new DataManagementException (string.Format (message, args));
    }

    private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName);
    }

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
    {
      return new ClientTransactionsDifferException (string.Format (message, args));
    }

    #region ICollectionEndPointChangeDelegate Members

    void ICollectionEndPointChangeDelegate.PerformInsert (
        CollectionEndPoint endPoint,
        DomainObject domainObject,
        int index)
    {
      CheckClientTransactionForInsertionIntoCollectionEndPoint (endPoint.ID, domainObject, index);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      ObjectEndPoint addingEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          domainObject, endPoint.OppositeEndPointDefinition);

      CollectionEndPoint oldRelatedOfAddingEndPoint = (CollectionEndPoint) GetRelationEndPoint (
          GetRelatedObject (addingEndPoint.ID, false),
          endPoint.Definition);

      RelationEndPoint oldRelatedNullEndPoint = RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition);

      RelationEndPointModificationCollection modifications = new RelationEndPointModificationCollection (
          addingEndPoint.CreateModification (oldRelatedOfAddingEndPoint, endPoint),
          endPoint.CreateInsertModification (oldRelatedNullEndPoint, addingEndPoint, index),
          oldRelatedOfAddingEndPoint.CreateModification (addingEndPoint));

      modifications.ExecuteAllSteps();
    }

    void ICollectionEndPointChangeDelegate.PerformReplace (
        CollectionEndPoint endPoint,
        DomainObject domainObject,
        int index)
    {
      CheckClientTransactionForReplacementInCollectionEndPoint (endPoint.ID, domainObject, index);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      ObjectEndPoint newEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          domainObject, endPoint.OppositeEndPointDefinition);

      ObjectEndPoint oldEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          endPoint.OppositeDomainObjects[index], endPoint.OppositeEndPointDefinition);

      CollectionEndPoint oldEndPointOfNewEndPoint = (CollectionEndPoint) GetRelationEndPoint (
          GetRelatedObject (newEndPoint.ID, false),
          newEndPoint.OppositeEndPointDefinition);

      RelationEndPointModificationCollection modifications = new RelationEndPointModificationCollection (
          oldEndPoint.CreateModification (endPoint),
          newEndPoint.CreateModification (oldEndPointOfNewEndPoint, endPoint),
          endPoint.CreateReplaceModification (oldEndPoint, newEndPoint),
          oldEndPointOfNewEndPoint.CreateModification (newEndPoint));

      modifications.ExecuteAllSteps();
    }

    void ICollectionEndPointChangeDelegate.PerformSelfReplace (
        CollectionEndPoint endPoint,
        DomainObject domainObject,
        int index)
    {
      SetRelatedObjectForEqualObjects (endPoint, GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition));
    }

    void ICollectionEndPointChangeDelegate.PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject)
    {
      CheckClientTransactionForRemovalFromCollectionEndPoint (endPoint.ID, domainObject);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      ObjectEndPoint removingEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

      RelationEndPointModificationCollection modifications = new RelationEndPointModificationCollection (
          removingEndPoint.CreateModification (endPoint),
          endPoint.CreateModification (removingEndPoint));

      modifications.ExecuteAllSteps();
    }

    #endregion

    public void CopyFrom (RelationEndPointMap source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      if (source == this)
        throw new ArgumentException ("Source cannot be the destination RelationEndPointMap instance.", "source");

      _transactionEventSink.RelationEndPointMapCopyingFrom (source);
      source._transactionEventSink.RelationEndPointMapCopyingTo (this);

      int startingPosition = _relationEndPoints.Count;

      for (int i = 0; i < source._relationEndPoints.Count; ++i)
      {
        RelationEndPoint newEndPoint = source._relationEndPoints[i].Clone();
        newEndPoint.SetClientTransaction (_clientTransaction);

        newEndPoint.RegisterWithMap (this);

        int position = _relationEndPoints.Add (newEndPoint);
        Assertion.IsTrue (position == i + startingPosition);
      }
    }

    public IEnumerator GetEnumerator ()
    {
      return _relationEndPoints.GetEnumerator();
    }

    #region Serialization

    protected RelationEndPointMap (FlattenedDeserializationInfo info)
        : this (info.GetValueForHandle<ClientTransaction>())
    {
      ArgumentUtility.CheckNotNull ("info", info);
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        RelationEndPoint[] endPointArray = info.GetArray<RelationEndPoint>();
        foreach (RelationEndPoint endPoint in endPointArray)
        {
          _relationEndPoints.Add (endPoint);
          endPoint.RegisterWithMap (this);
        }
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      RelationEndPoint[] endPointArray = new RelationEndPoint[Count];
      _relationEndPoints.CopyTo (endPointArray, 0);
      info.AddArray (endPointArray);
    }

    #endregion
  }
}
