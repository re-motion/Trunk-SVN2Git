// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointMap : IEnumerable, IFlattenedSerializable, ICollectionEndPointChangeDelegate
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _transactionEventSink;
    private readonly RelationEndPointCollection _relationEndPoints;
    private readonly RelationEndPointModifier _modifier;

    // construction and disposing

    public RelationEndPointMap (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;
      _transactionEventSink = clientTransaction.TransactionEventSink;
      _relationEndPoints = new RelationEndPointCollection (_clientTransaction);
      _modifier = new RelationEndPointModifier (this);
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

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public void Commit (DomainObjectCollection deletedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("deletedDomainObjects", deletedDomainObjects);

      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Commit();

      foreach (DomainObject deletedDomainObject in deletedDomainObjects)
      {
        foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(deletedDomainObject).RelationEndPointIDs)
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
        foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(newDomainObject).RelationEndPointIDs)
          Remove (endPointID);
      }
    }

    public void PerformDelete (DomainObject domainObject, RelationEndPointModificationCollection oppositeEndPointModifications)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("oppositeEndPointModifications", oppositeEndPointModifications);

      if (!domainObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        var message = string.Format ("Cannot remove DomainObject '{0}' from RelationEndPointMap, because it belongs to a different ClientTransaction.", 
            domainObject.ID);
        throw new ClientTransactionsDifferException(message);
      }

      RelationEndPointID[] relationEndPointIDs = ClientTransaction.GetDataContainer (domainObject).RelationEndPointIDs;
      _transactionEventSink.RelationEndPointMapPerformingDelete (relationEndPointIDs);

      foreach (RelationEndPointID endPointID in relationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

        oppositeEndPointModifications.Perform ();
        endPoint.PerformDelete ();

        if (domainObject.TransactionContext[ClientTransaction].State == StateType.New)
          Remove (endPointID);
      }
    }

    public RelationEndPointModificationCollection GetOppositeEndPointModificationsForDelete (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      return _modifier.GetOppositeEndPointModificationsForDelete (domainObject);
    }

    public DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "GetRelatedObject", "endPointID");

      var objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
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

      var objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
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

      var collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
      if (collectionEndPoint == null)
        return _clientTransaction.LoadRelatedObjects (endPointID);

      return collectionEndPoint.OppositeDomainObjects;
    }

    public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetOriginalRelatedObjects", "endPointID");

      var collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
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
      _modifier.SetRelatedObject (endPointID, newRelatedObject);
    }

    public void RegisterObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      var objectEndPoint = new ObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      Add (objectEndPoint);
    }

    public void RegisterCollectionEndPoint (RelationEndPointID endPointID, DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      var collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, domainObjects, this);
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
              var oppositeObjectID = (ObjectID) dataContainer.GetFieldValue (endPointDefinition.PropertyName, ValueAccess.Current);
              var endPoint = new ObjectEndPoint (dataContainer.ClientTransaction, dataContainer.ID, endPointDefinition, oppositeObjectID);
              Add (endPoint);

              if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
              {
                var oppositeEndPoint = new ObjectEndPoint (
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

      var allRelationEndPoints = new RelationEndPointCollection (_clientTransaction);

      foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(domainObject).RelationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

        if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.Many && !endPoint.OppositeEndPointDefinition.IsNull)
        {
          var objectEndPoint = (ObjectEndPoint) endPoint;
          if (objectEndPoint.OppositeObjectID != null)
          {
            var oppositeEndPointID = new RelationEndPointID (objectEndPoint.OppositeObjectID, objectEndPoint.OppositeEndPointDefinition);
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

      foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(domainObject).RelationEndPointIDs)
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
        RelationEndPoint newEndPoint = source._relationEndPoints[i].Clone (_clientTransaction);

        int position = _relationEndPoints.Add (newEndPoint);
        Assertion.IsTrue (position == i + startingPosition);
      }
    }

    public IEnumerator GetEnumerator ()
    {
      return _relationEndPoints.GetEnumerator();
    }

    private void CheckCardinality (
        RelationEndPointID endPointID,
        CardinalityType expectedCardinality,
        string methodName,
        string argumentName)
    {
      if (endPointID.Definition.Cardinality != expectedCardinality)
      {
        throw new ArgumentException (string.Format (
            "{0} can only be called for end points with a cardinality of '{1}'.",
            methodName,
            expectedCardinality), argumentName);
      }
    }

    void ICollectionEndPointChangeDelegate.PerformInsert (CollectionEndPoint endPoint, DomainObject newRelatedObject, int index)
    {
      _modifier.PerformInsert (endPoint, newRelatedObject, index);
    }

    void ICollectionEndPointChangeDelegate.PerformReplace (CollectionEndPoint endPoint, DomainObject newRelatedObject, int index)
    {
      _modifier.PerformReplace (endPoint, newRelatedObject, index);
    }

    void ICollectionEndPointChangeDelegate.PerformSelfReplace (CollectionEndPoint endPoint, DomainObject domainObject, int index)
    {
      _modifier.PerformSelfReplace (endPoint, domainObject, index);
    }

    void ICollectionEndPointChangeDelegate.PerformRemove (CollectionEndPoint endPoint, DomainObject removedRelatedObject)
    {
      _modifier.PerformRemove (endPoint, removedRelatedObject);
    }

    #region Serialization

    // Note: RelationEndPointMap should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointMap (FlattenedDeserializationInfo info)
        : this (info.GetValueForHandle<ClientTransaction>())
    {
      ArgumentUtility.CheckNotNull ("info", info);
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        RelationEndPoint[] endPointArray = info.GetArray<RelationEndPoint>();
        foreach (RelationEndPoint endPoint in endPointArray)
          _relationEndPoints.Add (endPoint);
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      var endPointArray = new RelationEndPoint[Count];
      _relationEndPoints.CopyTo (endPointArray, 0);
      info.AddArray (endPointArray);
    }

    #endregion
  }
}
