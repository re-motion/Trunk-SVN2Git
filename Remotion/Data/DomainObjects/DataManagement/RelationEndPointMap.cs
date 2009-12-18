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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointMap : IEnumerable, IFlattenedSerializable
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private readonly ICollectionEndPointChangeDetectionStrategy _collectionEndPointChangeDetectionStrategy;

    private readonly IClientTransactionListener _transactionEventSink;
    private readonly RelationEndPointCollection _relationEndPoints;

    // construction and disposing

    public RelationEndPointMap (ClientTransaction clientTransaction, ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);

      _clientTransaction = clientTransaction;
      _collectionEndPointChangeDetectionStrategy = collectionEndPointChangeDetectionStrategy;

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

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ICollectionEndPointChangeDetectionStrategy CollectionEndPointChangeDetectionStrategy
    {
      get { return _collectionEndPointChangeDetectionStrategy; }
    }

    public void Commit ()
    {
      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Commit ();
    }

    public void Rollback ()
    {
      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Rollback ();
    }

    public void Commit2 (DomainObjectCollection deletedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("deletedDomainObjects", deletedDomainObjects);

      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Commit();

      foreach (DomainObject deletedDomainObject in deletedDomainObjects)
      {
        foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(deletedDomainObject).AssociatedRelationEndPointIDs)
          Discard (endPointID);
      }
    }

    public void Rollback2 (DomainObjectCollection newDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("newDomainObjects", newDomainObjects);

      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Rollback();

      foreach (DomainObject newDomainObject in newDomainObjects)
      {
        foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(newDomainObject).AssociatedRelationEndPointIDs)
          Discard (endPointID);
      }
    }

    // TODO 1914: Called by DeleteDomainObjectCommand
    public void PerformDelete2 (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
    {
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
      ArgumentUtility.CheckNotNull ("oppositeEndPointRemoveModifications", oppositeEndPointRemoveModifications);

      if (!deletedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        var message = String.Format ("Cannot remove DomainObject '{0}' from RelationEndPointMap, because it belongs to a different ClientTransaction.", 
            deletedObject.ID);
        throw new ClientTransactionsDifferException(message);
      }

      var relationEndPointIDs = ClientTransaction.GetDataContainer (deletedObject).AssociatedRelationEndPointIDs;
      _transactionEventSink.RelationEndPointMapPerformingDelete (relationEndPointIDs);

      oppositeEndPointRemoveModifications.Perform (); // remove all back-references to the deleted object
      foreach (RelationEndPointID endPointID in relationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);
        
        // this triggers a Begin/EndDelete notification on CollectionEndPoint and clears the end point's data
        endPoint.PerformDelete ();

        if (deletedObject.TransactionContext[ClientTransaction].State == StateType.New)
          Discard (endPointID);
      }
    }

    // TODO 1914: Integrated into DeleteDomainObjectCommand
    public CompositeRelationModificationWithEvents GetRemoveModificationsForOppositeEndPoints (DomainObject deletedObject)
    {
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);

      var allOppositeRelationEndPoints = OppositeRelationEndPointFinder.GetOppositeRelationEndPoints (this, deletedObject);
      var modifications = from oppositeEndPoint in allOppositeRelationEndPoints
                          select oppositeEndPoint.CreateRemoveModification (deletedObject);
      return new CompositeRelationModificationWithEvents (modifications);
    }

    public DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "GetRelatedObject", "endPointID");

      var objectEndPoint = (IObjectEndPoint) GetRelationEndPointWithLazyLoad (endPointID);
      return objectEndPoint.GetOppositeObject (includeDeleted);
    }

    public DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "GetOriginalRelatedObject", "endPointID");

      var objectEndPoint = (IObjectEndPoint) GetRelationEndPointWithLazyLoad (endPointID);
      return objectEndPoint.GetOriginalOppositeObject ();
    }

    public DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetRelatedObjects", "endPointID");

      var collectionEndPoint = (ICollectionEndPoint) GetRelationEndPointWithLazyLoad (endPointID);
      return collectionEndPoint.OppositeDomainObjects;
    }

    public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetOriginalRelatedObjects", "endPointID");

      var collectionEndPoint = (ICollectionEndPoint) GetRelationEndPointWithLazyLoad (endPointID);
      return collectionEndPoint.OriginalOppositeDomainObjectsContents;
    }

    public ObjectEndPoint RegisterObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      var objectEndPoint = new ObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      Add (objectEndPoint);
      return objectEndPoint;
    }

    public CollectionEndPoint RegisterCollectionEndPoint (RelationEndPointID endPointID, IEnumerable<DomainObject> initialContents)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("initialContents", initialContents);

      var collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, _collectionEndPointChangeDetectionStrategy, initialContents);
      Add (collectionEndPoint);

      return collectionEndPoint;
    }

    public void RegisterExistingDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      var realObjectEndPoints = from endPointDefinition in classDefinition.GetRelationEndPointDefinitions()
                                where !endPointDefinition.IsVirtual
                                let oppositeObjectID = (ObjectID) dataContainer.PropertyValues[endPointDefinition.PropertyName].GetValueWithoutEvents (ValueAccess.Current)
                                let endPointID = new RelationEndPointID (dataContainer.ID, endPointDefinition)
                                select new ObjectEndPoint (ClientTransaction, endPointID, oppositeObjectID);
      
      foreach (var realObjectEndPoint in realObjectEndPoints)
      {
        Add (realObjectEndPoint);

        var oppositeEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition();
        if (oppositeEndPointDefinition.Cardinality == CardinalityType.One && realObjectEndPoint.OppositeObjectID != null)
        {
          var oppositeEndPointID = new RelationEndPointID (realObjectEndPoint.OppositeObjectID, oppositeEndPointDefinition);
          var oppositeEndPoint = new ObjectEndPoint (ClientTransaction, oppositeEndPointID, realObjectEndPoint.ObjectID);
          Add (oppositeEndPoint);
        }
      }
    }

    public void RegisterNewDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (RelationEndPointID endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (endPointID.Definition.Cardinality == CardinalityType.One)
          RegisterObjectEndPoint (endPointID, null);
        else
        {
          RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
        }
      }
    }

    public void CheckMandatoryRelations (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(domainObject).AssociatedRelationEndPointIDs)
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

      foreach (RelationEndPointID endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        RelationEndPoint endPoint = _relationEndPoints[endPointID];
        if (endPoint != null && endPoint.HasChanged)
          return true;
      }

      return false;
    }
    
    public RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.IsAnonymous)
      {
        throw new InvalidOperationException (
            "Cannot get a RelationEndPoint for an anonymous end point definition. There are no end points for the non-existing side of unidirectional "
            + "relations.");
      }

      var existingEndPoint = _relationEndPoints[endPointID];
      if (existingEndPoint != null)
        return _relationEndPoints[endPointID];

      if (!endPointID.Definition.IsVirtual)
      {
        throw new InvalidOperationException (
            "Cannot lazily load the real part of a relation. RegisterExistingDataContainer or RegisterNewDataContainer must be called before any "
            + "non-virtual end points are retrieved.");
      }

      if (endPointID.Definition.Cardinality == CardinalityType.One)
        _clientTransaction.LoadRelatedObject (endPointID); // indirectly calls RegisterExistingDataContainer, which registers the loaded end point
      else
        _clientTransaction.LoadRelatedObjects (endPointID); // calls RegisterCollectionEndPoint, which registers the loaded end point

      var loadedEndPoint = _relationEndPoints[endPointID];
      Assertion.IsNotNull (loadedEndPoint);

      return loadedEndPoint;
    }

    private void Add (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _transactionEventSink.RelationEndPointMapRegistering (endPoint);
      _relationEndPoints.Add (endPoint);
    }

    public void Discard (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (this[endPointID] == null)
      {
        var message = string.Format ("End point '{0}' is not part of this map.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }

      _transactionEventSink.RelationEndPointMapUnregistering (endPointID);
      _relationEndPoints.Remove (endPointID);
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
        throw new ArgumentException (String.Format (
            "{0} can only be called for end points with a cardinality of '{1}'.",
            methodName,
            expectedCardinality), argumentName);
      }
    }

    #region Serialization

    // Note: RelationEndPointMap should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointMap (FlattenedDeserializationInfo info)
        : this (info.GetValueForHandle<ClientTransaction>(), info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy>())
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
      info.AddHandle (_collectionEndPointChangeDetectionStrategy);
      var endPointArray = new RelationEndPoint[Count];
      _relationEndPoints.CopyTo (endPointArray, 0);
      info.AddArray (endPointArray);
    }

    #endregion
  }
}
