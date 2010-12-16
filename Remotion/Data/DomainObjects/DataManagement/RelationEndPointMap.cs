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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointMap : IRelationEndPointMapReadOnlyView, IFlattenedSerializable
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

    public bool Contains (RelationEndPointID id)
    {
      return this[id] != null;
    }

    public void CommitAllEndPoints ()
    {
      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Commit ();
    }

    public void RollbackAllEndPoints ()
    {
      foreach (RelationEndPoint endPoint in _relationEndPoints)
        endPoint.Rollback ();
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

    // When registering a non-virtual end-point, the opposite virtual object end-point (if any) is registered as well.
    // TODO 3401: If the opposite end-point is a collection, the end-point is registered in that collection.
    public RealObjectEndPoint RegisterRealObjectEndPoint (RelationEndPointID endPointID, DataContainer foreignKeyDataContainer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("foreignKeyDataContainer", foreignKeyDataContainer);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterRealObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, false, "RegisterRealObjectEndPoint", "endPointID");
      
      var objectEndPoint = new RealObjectEndPoint (_clientTransaction, endPointID, foreignKeyDataContainer);
      Add (objectEndPoint);

      var oppositeVirtualEndPointDefinition = objectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (objectEndPoint.OppositeObjectID != null)
      {
        var oppositeVirtualEndPointID = new RelationEndPointID (objectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
        if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One)
        {
          RegisterVirtualObjectEndPoint (oppositeVirtualEndPointID, objectEndPoint.ObjectID);
        }
        // TODO 3401
        // else if (!oppositeVirtualEndPointDefinition.IsAnonymous)
        // {
        //   "RegisterInCollectionEndPoint"
        //   var oppositeEndPoint = (CollectionEndPoint) this[oppositeVirtualEndPointID];
        //   // TODO 3401: end-point must be added if not exists:
        //   if (oppositeEndPoint == null)
        //     oppositeEndPoint = RegisterCollectionEndPoint (oppositeVirtualEndPointID, null);
        //
        //   oppositeEndPoint.RegisterOriginalObject (objectEndPoint.GetDomainObject());
        // }
      }

      return objectEndPoint;
    }

    // When unregistering a non-virtual end-point, the opposite virtual object end-point (if any) is unregistered as well.
    // If the opposite end-point is a collection, that collection is put into unloaded state. // TODO 3401: In addition, the end-point is unregistered 
    // as an item of that collection.
    public void UnregisterRealObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "UnregisterRealObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, false, "UnregisterRealObjectEndPoint", "endPointID");

      var objectEndPoint = (ObjectEndPoint) this[endPointID];
      if (objectEndPoint == null)
        throw new ArgumentException ("The given end-point is not part of this map.", "endPointID");

      CheckUnchangedForUnregister (endPointID, objectEndPoint);

      RemoveEndPoint (endPointID);

      var oppositeVirtualEndPointDefinition = objectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (objectEndPoint.OppositeObjectID != null)
      {
        var oppositeVirtualEndPointID = new RelationEndPointID (objectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
        if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One)
        {
          UnregisterVirtualObjectEndPoint (oppositeVirtualEndPointID);
        }
        else
        {
          var oppositeEndPoint = (CollectionEndPoint) this[oppositeVirtualEndPointID];
          // TODO 3401: end-point can be relied on (but add a guard clause against anonymous end-points)
          if (oppositeEndPoint != null)
          {
            Assertion.IsFalse (oppositeVirtualEndPointDefinition.IsAnonymous);
            // TODO 3401: oppositeEndPoint.UnregisterOriginalObject (objectEndPoint.ObjectID);
            oppositeEndPoint.Unload ();
          }
        }
      }
    }

    private VirtualObjectEndPoint RegisterVirtualObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterVirtualObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, true, "RegisterVirtualObjectEndPoint", "endPointID");

      var objectEndPoint = new VirtualObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      Add (objectEndPoint);

      return objectEndPoint;
    }

    public void UnregisterVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterVirtualObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, true, "UnregisterVirtualObjectEndPoint", "endPointID");

      var objectEndPoint = (ObjectEndPoint) this[endPointID];
      if (objectEndPoint == null)
        throw new ArgumentException ("The given end-point is not part of this map.", "endPointID");

      CheckUnchangedForUnregister (endPointID, objectEndPoint);

      RemoveEndPoint (endPointID);
    }

    public CollectionEndPoint RegisterCollectionEndPoint (RelationEndPointID endPointID, IEnumerable<DomainObject> initialContentsOrNull)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "RegisterCollectionEndPoint", "endPointID");

      var collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, _collectionEndPointChangeDetectionStrategy, initialContentsOrNull);
      Add (collectionEndPoint);

      return collectionEndPoint;
    }

    // When registering a DataContainer, its real end-points are always registered, too. This will indirectly register opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are registered as well.
    public void RegisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        if (!endPointID.Definition.IsVirtual)
          RegisterRealObjectEndPoint (endPointID, dataContainer);
        else if (endPointID.Definition.Cardinality == CardinalityType.One)
          RegisterVirtualObjectEndPoint (endPointID, null);
        else
          RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
      }
    }

    // When unregistering a DataContainer, its real end-points are always unregistered. This will indirectly unregister opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are unregistered as well.
    // If the DataContainer is not New, virtual object end-points with a null Original value are also unregistered because they are owned by this DataContainer.
    public void UnregisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      // TODO 3475: if (GetUnregisterableEndPointsForDataContainer (dataContainer).Any()) throw;

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        if (!endPointID.Definition.IsVirtual)
          UnregisterRealObjectEndPoint (endPointID);
        else if (endPointID.Definition.Cardinality == CardinalityType.One)
          UnregisterVirtualObjectEndPoint (endPointID);
        // TODO 3475
        //else
        //  UnregisterCollectionEndPoint (endPointID);
      }
    }

    // TODO 3475:
    //public IEnumerable<RelationEndPoint> GetUnregisterableEndPointsForDataContainer (DataContainer dataContainer)
    //{
    //  ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    //  return from endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer)
    //         let loadedEndPoint = this[endPointID]
    //         where loadedEndPoint != null && loadedEndPoint.HasChanged
    //         select loadedEndPoint;
    //}

    public RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.IsAnonymous)
      {
        throw new InvalidOperationException (
            "Cannot get a RelationEndPoint for an anonymous end point definition. There are no end points for the non-existing side of unidirectional "
            + "relations.");
      }

      if (!endPointID.Definition.IsVirtual)
        ClientTransaction.EnsureDataAvailable (endPointID.ObjectID); // to retrieve a real end-point, the data container must have been registered

      var existingEndPoint = _relationEndPoints[endPointID];
      if (existingEndPoint != null)
        return _relationEndPoints[endPointID];

      Assertion.IsTrue (endPointID.Definition.IsVirtual, 
          "EnsureDataAvailable should guarantee that the DataContainer is registered, which in turn guarantees that all non-virtual end points are "
          + "registered in the map");

      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        // loading the related object will automatically register the related's real end points via RegisterEndPointsForExistingDataContainer, 
        // which also registers the opposite virtual end point; see assertion below
        var relatedObject = _clientTransaction.LoadRelatedObject (endPointID);
        if (relatedObject == null)
          RegisterVirtualObjectEndPoint (endPointID, null);

        Assertion.IsNotNull (
            _relationEndPoints[endPointID], 
            "Loading related object should have indirectly registered this end point because that object holds the foreign key");
      }
      else
      {
        var endPoint = RegisterCollectionEndPoint (endPointID, null);
        endPoint.EnsureDataAvailable ();
      }

      var loadedEndPoint = _relationEndPoints[endPointID];
      Assertion.IsNotNull (loadedEndPoint);

      return loadedEndPoint;
    }

    private void Add (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _transactionEventSink.RelationEndPointMapRegistering (_clientTransaction, endPoint);
      _relationEndPoints.Add (endPoint);
    }

    public void RemoveEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (!Contains (endPointID))
      {
        var message = string.Format ("End point '{0}' is not part of this map.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }

      _transactionEventSink.RelationEndPointMapUnregistering (_clientTransaction, endPointID);
      _relationEndPoints.Remove (endPointID);
    }

    public IEnumerator<RelationEndPoint> GetEnumerator ()
    {
      return _relationEndPoints.Cast<RelationEndPoint>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    private IEnumerable<RelationEndPointID> GetEndPointIDsOwnedByDataContainer (DataContainer dataContainer)
    {
      var includeVirtualEndPoints = dataContainer.State == StateType.New;
      foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (!endPointID.Definition.IsVirtual || includeVirtualEndPoints)
        {
          yield return endPointID;
        }
        else if (endPointID.Definition.Cardinality == CardinalityType.One)
        {
          var loadedVirtualObjectEndPoint = (ObjectEndPoint) this[endPointID];
          if (loadedVirtualObjectEndPoint != null && loadedVirtualObjectEndPoint.OriginalOppositeObjectID == null)
            yield return endPointID;
        }
      }
    }

    private void CheckCardinality (
        RelationEndPointID endPointID,
        CardinalityType expectedCardinality,
        string methodName,
        string argumentName)
    {
      if (endPointID.Definition.Cardinality != expectedCardinality)
      {
        var message = string.Format ("{0} can only be called for end points with a cardinality of '{1}'.", methodName, expectedCardinality);
        throw new ArgumentException (message, argumentName);
      }
    }

    private void CheckVirtuality (
        RelationEndPointID endPointID,
        bool expectedVirtualness,
        string methodName,
        string argumentName)
    {
      if (endPointID.Definition.IsVirtual != expectedVirtualness)
      {
        if (expectedVirtualness)
        {
          var message = string.Format ("{0} can only be called for virtual end points.", methodName);
          throw new ArgumentException (message, argumentName);
        }
        else
        {
          var message = string.Format ("{0} can only be called for non-virtual end points.", methodName);
          throw new ArgumentException (message, argumentName);
        }
      }
    }

    private void CheckUnchangedForUnregister (RelationEndPointID endPointID, IEndPoint objectEndPoint)
    {
      if (objectEndPoint.HasChanged)
      {
        var message = string.Format (
            "Cannot remove end-point '{0}' because it has changed. End-points can only be unregistered when they are unchanged.",
            endPointID);
        throw new InvalidOperationException (message);
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
