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
using System.Collections;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;
using System.Collections.Generic;
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class RelationEndPointMap : IRelationEndPointMapReadOnlyView, IFlattenedSerializable
  {
    public static IRelationEndPoint CreateNullEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.Cardinality == CardinalityType.One)
        return new NullObjectEndPoint (clientTransaction, endPointID.Definition);
      else
        return new NullCollectionEndPoint (clientTransaction, endPointID.Definition);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly ICollectionEndPointChangeDetectionStrategy _collectionEndPointChangeDetectionStrategy;
    private readonly IObjectLoader _objectLoader;
    private readonly IRelationEndPointLazyLoader _lazyLoader;

    private readonly IClientTransactionListener _transactionEventSink;
    private readonly Dictionary<RelationEndPointID, IRelationEndPoint> _relationEndPoints;

    public RelationEndPointMap (
        ClientTransaction clientTransaction,
        ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy,
        IObjectLoader objectLoader,
        IRelationEndPointLazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      _clientTransaction = clientTransaction;
      _collectionEndPointChangeDetectionStrategy = collectionEndPointChangeDetectionStrategy;
      _objectLoader = objectLoader;
      _lazyLoader = lazyLoader;

      _transactionEventSink = clientTransaction.TransactionEventSink;
      _relationEndPoints = new Dictionary<RelationEndPointID, IRelationEndPoint> ();
    }

    public IRelationEndPoint this[RelationEndPointID endPointID]
    {
      get { return _relationEndPoints.GetValueOrDefault (endPointID); }
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
      foreach (IRelationEndPoint endPoint in _relationEndPoints.Values)
        endPoint.Commit ();
    }

    public void RollbackAllEndPoints ()
    {
      foreach (IRelationEndPoint endPoint in _relationEndPoints.Values)
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
      return collectionEndPoint.Collection;
    }

    public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "GetOriginalRelatedObjects", "endPointID");

      var collectionEndPoint = (ICollectionEndPoint) GetRelationEndPointWithLazyLoad (endPointID);
      return collectionEndPoint.GetCollectionWithOriginalData();
    }

    // When registering a non-virtual end-point, the opposite virtual object end-point (if any) is registered as well.
    public RealObjectEndPoint RegisterRealObjectEndPoint (RelationEndPointID endPointID, DataContainer foreignKeyDataContainer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("foreignKeyDataContainer", foreignKeyDataContainer);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterRealObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, false, "RegisterRealObjectEndPoint", "endPointID");
      
      var objectEndPoint = new RealObjectEndPoint (_clientTransaction, endPointID, foreignKeyDataContainer);
      Add (objectEndPoint);

      RegisterOppositeForRealObjectEndPoint (objectEndPoint);
      return objectEndPoint;
    }

    // When unregistering a non-virtual end-point, the opposite virtual object end-point (if any) is unregistered as well.
    // If the opposite end-point is a collection, that collection is put into incomplete state.
    public void UnregisterRealObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "UnregisterRealObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, false, "UnregisterRealObjectEndPoint", "endPointID");

      var objectEndPoint = (IObjectEndPoint) this[endPointID];
      if (objectEndPoint == null)
        throw new ArgumentException ("The given end-point is not part of this map.", "endPointID");

      CheckUnchangedForUnregister (endPointID, objectEndPoint);

      RemoveEndPoint (endPointID);

      UnregisterOppositeForRealObjectEndPoint(objectEndPoint);
    }

    public VirtualObjectEndPoint RegisterVirtualObjectEndPointWithNullOpposite (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterVirtualObjectEndPoint", "endPointID");
      CheckVirtuality (endPointID, true, "RegisterVirtualObjectEndPoint", "endPointID");

      return RegisterVirtualObjectEndPoint (endPointID, null);
    }

    private VirtualObjectEndPoint RegisterVirtualObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      var objectEndPoint = new VirtualObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      Add (objectEndPoint);

      objectEndPoint.MarkSynchronized();

      return objectEndPoint;
    }

    private void UnregisterVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      var objectEndPoint = (IObjectEndPoint) this[endPointID];
      Assertion.IsNotNull (objectEndPoint, "This method is only called in situations where the end-point has been registered.");

      CheckUnchangedForUnregister (endPointID, objectEndPoint);

      RemoveEndPoint (endPointID);
    }

    public CollectionEndPoint RegisterCollectionEndPoint (RelationEndPointID endPointID, IEnumerable<DomainObject> initialContentsOrNull)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "RegisterCollectionEndPoint", "endPointID");

      var collectionEndPoint = new CollectionEndPoint (
          _clientTransaction, 
          endPointID, 
          _collectionEndPointChangeDetectionStrategy,
          _lazyLoader,
          initialContentsOrNull);
      Add (collectionEndPoint);

      return collectionEndPoint;
    }

    public void UnregisterCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "UnregisterCollectionEndPoint", "endPointID");

      var collectionEndPoint = (ICollectionEndPoint) this[endPointID];
      if (collectionEndPoint == null)
        throw new ArgumentException ("The given end-point is not part of this map.", "endPointID");

      CheckUnchangedForUnregister (endPointID, collectionEndPoint);
      
      RemoveEndPoint (endPointID);
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

      var unregisterableEndPoints = GetNonUnregisterableEndPointsForDataContainer (dataContainer);
      if (unregisterableEndPoints.Length > 0)
      {
        var message = string.Format (
            "Cannot unregister the following relation end-points: {0}. Relation end-points can only be removed when they are unchanged.",
            SeparatedStringBuilder.Build (", ", unregisterableEndPoints, endPoint => "'" + endPoint + "'"));
        throw new InvalidOperationException (message);
      }

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        if (!endPointID.Definition.IsVirtual)
          UnregisterRealObjectEndPoint (endPointID);
        else if (endPointID.Definition.Cardinality == CardinalityType.One)
          UnregisterVirtualObjectEndPoint (endPointID);
        else
          UnregisterCollectionEndPoint (endPointID);
      }
    }

    public IRelationEndPoint[] GetNonUnregisterableEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return (from endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer)
              let loadedEndPoint = this[endPointID]
              where loadedEndPoint != null && !IsUnregisterable (loadedEndPoint)
              select loadedEndPoint).ToArray();
    }

    public void CheckForConflictingForeignKeys (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        if (!endPointID.Definition.IsVirtual)
        {
          var oppositeVirtualEndPointDefinition = endPointID.Definition.GetOppositeEndPointDefinition ();
          if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One)
            CheckForConflictingForeignKey (dataContainer, endPointID.Definition, oppositeVirtualEndPointDefinition);
        }
      }
    }

    public void MarkCollectionEndPointComplete (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "MarkCollectionEndPointComplete", "endPointID");
      CheckNotAnonymous (endPointID, "MarkCollectionEndPointComplete", "endPointID");

      var endPoint = GetCollectionEndPointOrRegisterEmpty (endPointID);
      endPoint.MarkDataComplete ();
    }
    
    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckNotAnonymous (endPointID, "GetRelationEndPointWithLazyLoad", "endPointID");

      if (endPointID.ObjectID == null)
        return CreateNullEndPoint(ClientTransaction, endPointID);

      if (!endPointID.Definition.IsVirtual)
        ClientTransaction.EnsureDataAvailable (endPointID.ObjectID); // to retrieve a real end-point, the data container must have been registered

      var existingEndPoint = _relationEndPoints.GetValueOrDefault (endPointID);
      if (existingEndPoint != null)
        return _relationEndPoints[endPointID];

      Assertion.IsTrue (endPointID.Definition.IsVirtual,
          "EnsureDataAvailable should guarantee that the DataContainer is registered, which in turn guarantees that all non-virtual end points are "
          + "registered in the map");

      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        // loading the related object will automatically register the related's real end points via RegisterEndPointsForExistingDataContainer, 
        // which also registers the opposite virtual end point; see assertion below
        var relatedObject = _objectLoader.LoadRelatedObject (endPointID, ClientTransaction.DataManager);
        if (relatedObject == null)
          RegisterVirtualObjectEndPoint (endPointID, null);

        Assertion.IsTrue (
            _relationEndPoints.ContainsKey (endPointID), 
            "Loading related object should have indirectly registered this end point because that object holds the foreign key");
      }
      else
      {
        var endPoint = RegisterCollectionEndPoint (endPointID, null);
        endPoint.EnsureDataComplete ();
      }

      Assertion.IsTrue (_relationEndPoints.ContainsKey (endPointID));
      return _relationEndPoints[endPointID];
    }

    private void Add (IRelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _transactionEventSink.RelationEndPointMapRegistering (_clientTransaction, endPoint);
      _relationEndPoints.Add (endPoint.ID, endPoint);
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

    public IEnumerator<IRelationEndPoint> GetEnumerator ()
    {
      return _relationEndPoints.Values.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public IRelationEndPoint GetOppositeEndPoint (IObjectEndPoint objectEndPoint)
    {
      ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

      if (this[objectEndPoint.ID] != objectEndPoint)
        throw new ArgumentException ("The end-point is not registered in this map.", "objectEndPoint");

      var oppositeRelationEndPointID = objectEndPoint.GetOppositeRelationEndPointID ();
      if (oppositeRelationEndPointID == null)
        throw new ArgumentException ("The end-point is not part of a bidirectional relation.", "objectEndPoint");

      if (oppositeRelationEndPointID.ObjectID == null)
        return CreateNullEndPoint (_clientTransaction, oppositeRelationEndPointID);

      var oppositeEndPoint = this[oppositeRelationEndPointID];
      Assertion.IsNotNull (
          oppositeEndPoint,
          "The implementation of RelationEndPointMap guarantees that when an object end-point is registered, the opposite is registered, too.");

      return oppositeEndPoint;
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
          var loadedVirtualObjectEndPoint = (IObjectEndPoint) this[endPointID];
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

    private void CheckNotAnonymous (RelationEndPointID endPointID, string methodName, string argumentName)
    {
      if (endPointID.Definition.IsAnonymous)
      {
        var message = string.Format ("{0} cannot be called for anonymous end points.", methodName);
        throw new ArgumentException (message, argumentName);
      }
    }

    private void CheckUnchangedForUnregister (RelationEndPointID endPointID, IRelationEndPoint endPoint)
    {
      if (!IsUnregisterable(endPoint))
      {
        var message = string.Format (
            "Cannot remove end-point '{0}' because it has changed. End-points can only be unregistered when they are unchanged.",
            endPointID);
        throw new InvalidOperationException (message);
      }
    }

    private bool IsUnregisterable (IRelationEndPoint endPoint)
    {
      // An end-point must be unchanged to be unregisterable.
      if (endPoint.HasChanged)
        return false;

      // If it is a real object end-point pointing to a non-null object, and the opposite end-point is loaded, the opposite (virtual) end-point 
      // must be unchanged. Virtual end-points cannot exist in changed state without their opposite real end-points.
      // (This only affects 1:n relations: for those, the opposite virtual end-point can be changed although the (one of many) real end-point is 
      // unchanged. For 1:1 relations, the real and virtual end-points always have an equal HasChanged flag.)
      var maybeOppositeEndPoint =
          Maybe.ForValue (endPoint)
              .Where (ep => !ep.Definition.IsVirtual)
              .Select (ep => ep as IObjectEndPoint)
              .Where (ep => ep.OppositeObjectID != null)
              .Select (ep => RelationEndPointID.Create(ep.OppositeObjectID, ep.Definition.GetOppositeEndPointDefinition ()))
              .Select (oppositeID => this[oppositeID]);
      if (maybeOppositeEndPoint.Where (oppositeEndPoint => oppositeEndPoint.HasChanged).HasValue)
        return false;

      return true;
    }

    private void RegisterOppositeForRealObjectEndPoint (IObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (realObjectEndPoint.OppositeObjectID == null || oppositeVirtualEndPointDefinition.IsAnonymous)
      {
        realObjectEndPoint.MarkSynchronized ();
        return;
      }

      var oppositeVirtualEndPointID = RelationEndPointID.Create(realObjectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
      if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One)
      {
        RegisterVirtualObjectEndPoint (oppositeVirtualEndPointID, realObjectEndPoint.ObjectID);
        realObjectEndPoint.MarkSynchronized();
      }
      else
      {
        var oppositeEndPoint = GetCollectionEndPointOrRegisterEmpty (oppositeVirtualEndPointID);
        oppositeEndPoint.RegisterOppositeEndPoint (realObjectEndPoint); // calls MarkSynchronized/MarkUnsynchronized
      }
    }

    private void UnregisterOppositeForRealObjectEndPoint (IObjectEndPoint realObjectEndPoint)
    {
      Assertion.IsFalse (realObjectEndPoint.HasChanged, "Deregistration currently only works for unchanged end-points");

      var oppositeVirtualEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (realObjectEndPoint.OppositeObjectID != null)
      {
        var oppositeVirtualEndPointID = RelationEndPointID.Create(realObjectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
        if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One)
        {
          UnregisterVirtualObjectEndPoint (oppositeVirtualEndPointID);
        }
        else
        {
          var oppositeEndPoint = (ICollectionEndPoint) this[oppositeVirtualEndPointID];
          if (oppositeEndPoint != null)
          {
            Assertion.IsFalse (oppositeVirtualEndPointDefinition.IsAnonymous);
            oppositeEndPoint.UnregisterOppositeEndPoint (realObjectEndPoint);
          }
        }
      }
    }

    private ICollectionEndPoint GetCollectionEndPointOrRegisterEmpty (RelationEndPointID endPointID)
    {
      return (ICollectionEndPoint) this[endPointID] ?? RegisterCollectionEndPoint (endPointID, null);
    }

    // Check whether the given dataContainer contains a conflicting foreign key for the given definition. A foreign key is conflicting if it
    // is non-null and points to an object that already points back to another object.
    private void CheckForConflictingForeignKey (
        DataContainer dataContainer,
        IRelationEndPointDefinition foreignKeyEndPointDefinition,
        IRelationEndPointDefinition oppositeVirtualObjectEndPointDefinition)
    {
      Assertion.IsTrue (foreignKeyEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (dataContainer.ClassDefinition));
      Assertion.IsFalse (foreignKeyEndPointDefinition.IsVirtual);
      Assertion.IsTrue (oppositeVirtualObjectEndPointDefinition.IsVirtual);
      Assertion.IsTrue (oppositeVirtualObjectEndPointDefinition.Cardinality == CardinalityType.One);

      var foreignKeyValue =
          (ObjectID) dataContainer.PropertyValues[foreignKeyEndPointDefinition.PropertyName].GetValueWithoutEvents (ValueAccess.Current);
      if (foreignKeyValue == null) // null is never a conflicting foreign key value
        return;

      var oppositeVirtualEndPointID = RelationEndPointID.Create(foreignKeyValue, oppositeVirtualObjectEndPointDefinition);
      var existingOppositeVirtualEndPoint = (IObjectEndPoint) this[oppositeVirtualEndPointID];
      if (existingOppositeVirtualEndPoint == null) // if the opposite end point does not exist, this is not a conflicting foreign key value
        return;

      var existingConflictingObjectID = existingOppositeVirtualEndPoint.OriginalOppositeObjectID;
      var message =
            string.Format (
                "The data of object '{0}' conflicts with existing data: It has a foreign key "
                + "property '{1}' which points to object '{2}'. However, that object has previously been determined to point back to object '{3}'. "
                + "These two pieces of information contradict each other.",
                dataContainer.ID,
                foreignKeyEndPointDefinition.PropertyName,
                foreignKeyValue,
                existingConflictingObjectID != null ? existingConflictingObjectID.ToString() : "<null>");
      throw new InvalidOperationException (message);
    }

    #region Serialization

    // Note: RelationEndPointMap should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointMap (FlattenedDeserializationInfo info)
        : this (
            info.GetValueForHandle<ClientTransaction>(),
            info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy>(),
            info.GetValueForHandle<IObjectLoader>(),
            info.GetValueForHandle<IRelationEndPointLazyLoader>())
    {
      ArgumentUtility.CheckNotNull ("info", info);
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        IRelationEndPoint[] endPointArray = info.GetArray<IRelationEndPoint> ();
        foreach (IRelationEndPoint endPoint in endPointArray)
          _relationEndPoints.Add (endPoint.ID, endPoint);
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      info.AddHandle (_collectionEndPointChangeDetectionStrategy);
      info.AddHandle (_objectLoader);
      info.AddHandle (_lazyLoader);
      var endPointArray = new IRelationEndPoint[Count];
      _relationEndPoints.Values.CopyTo (endPointArray, 0);
      info.AddArray (endPointArray);
    }

    #endregion
  }
}
