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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
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
    public static IRelationEndPoint CreateNullEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      if (endPointDefinition.Cardinality == CardinalityType.Many)
        return new NullCollectionEndPoint (clientTransaction, endPointDefinition);
      else if (endPointDefinition.IsVirtual)
        return new NullVirtualObjectEndPoint (clientTransaction, endPointDefinition);
      else
        return new NullObjectEndPoint (clientTransaction, endPointDefinition);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IObjectLoader _objectLoader;
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _collectionEndPointDataKeeperFactory;
    private readonly IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _virtualObjectEndPointDataKeeperFactory;

    private readonly IClientTransactionListener _transactionEventSink;
    private readonly Dictionary<RelationEndPointID, IRelationEndPoint> _relationEndPoints;

    public RelationEndPointMap (
        ClientTransaction clientTransaction,
        IObjectLoader objectLoader,
        IRelationEndPointLazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> collectionEndPointDataKeeperFactory,
        IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> virtualObjectEndPointDataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("collectionEndPointDataKeeperFactory", collectionEndPointDataKeeperFactory);
      ArgumentUtility.CheckNotNull ("virtualObjectEndPointDataKeeperFactory", virtualObjectEndPointDataKeeperFactory);

      _clientTransaction = clientTransaction;
      _objectLoader = objectLoader;
      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _collectionEndPointDataKeeperFactory = collectionEndPointDataKeeperFactory;
      _virtualObjectEndPointDataKeeperFactory = virtualObjectEndPointDataKeeperFactory;

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

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IObjectLoader ObjectLoader
    {
      get { return _objectLoader; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> CollectionEndPointDataKeeperFactory
    {
      get { return _collectionEndPointDataKeeperFactory; }
    }

    public IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> VirtualObjectEndPointDataKeeperFactory
    {
      get { return _virtualObjectEndPointDataKeeperFactory; }
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
      
      var objectEndPoint = new RealObjectEndPoint (_clientTransaction, endPointID, foreignKeyDataContainer, _lazyLoader, _endPointProvider);
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

      var objectEndPoint = (IRealObjectEndPoint) this[endPointID];
      if (objectEndPoint == null)
        throw new ArgumentException ("The given end-point is not part of this map.", "endPointID");

      CheckUnchangedForUnregister (endPointID, objectEndPoint);

      RemoveEndPoint (endPointID);

      UnregisterOppositeForRealObjectEndPoint(objectEndPoint);
    }

    private VirtualObjectEndPoint RegisterVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      var objectEndPoint = new VirtualObjectEndPoint (
          _clientTransaction,
          endPointID,
          _lazyLoader,
          _endPointProvider,
          _virtualObjectEndPointDataKeeperFactory);
      
      Add (objectEndPoint);
      
      return objectEndPoint;
    }

    public CollectionEndPoint RegisterCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.Many, "RegisterCollectionEndPoint", "endPointID");
      
      var collectionEndPoint = new CollectionEndPoint (
          _clientTransaction, 
          endPointID, 
          _lazyLoader,
          _endPointProvider,
          _collectionEndPointDataKeeperFactory);
      Add (collectionEndPoint);

      return collectionEndPoint;
    }

    private IVirtualEndPoint RegisterVirtualEndPoint (RelationEndPointID endPointID)
    {
      IVirtualEndPoint endPoint;
      if (endPointID.Definition.Cardinality == CardinalityType.One)
        endPoint = RegisterVirtualObjectEndPoint (endPointID);
      else
        endPoint = RegisterCollectionEndPoint (endPointID);
      return endPoint;
    }

    private IVirtualEndPoint GetVirtualEndPointOrRegisterEmpty (RelationEndPointID endPointID)
    {
      return (IVirtualEndPoint) this[endPointID] ?? RegisterVirtualEndPoint (endPointID);
    }

    // When registering a DataContainer, its real end-points are always registered, too. This may indirectly register opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are registered as well.
    public void RegisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        if (!endPointID.Definition.IsVirtual)
        {
          RegisterRealObjectEndPoint (endPointID, dataContainer);
        }
        else if (endPointID.Definition.Cardinality == CardinalityType.One)
        {
          var endPoint = RegisterVirtualObjectEndPoint (endPointID);
          endPoint.MarkDataComplete (null);
        }
        else
        {
          var endPoint = RegisterCollectionEndPoint (endPointID);
          endPoint.MarkDataComplete (new DomainObject[0]);
        }
      }
    }

    // When unregistering a DataContainer, its real end-points are always unregistered. This may indirectly unregister opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are unregistered as well.
    public IDataManagementCommand GetUnregisterCommandForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var loadedEndPoints = GetEndPointIDsOwnedByDataContainer (dataContainer).Select (id => this[id]).Where (endPoint => endPoint != null).ToArray();
      var endPointIDs = new List<RelationEndPointID>();
      var notUnregisterableEndPointIDs = new List<RelationEndPointID> ();

      foreach (var endPoint in loadedEndPoints)
      {
        endPointIDs.Add (endPoint.ID);

        if (!IsUnregisterable (endPoint))
          notUnregisterableEndPointIDs.Add (endPoint.ID);
      }

      if (notUnregisterableEndPointIDs.Count > 0)
      {
        var message = string.Format (
            "Object '{0}' cannot be unloaded because its relations have been changed. Only unchanged objects that are not part of changed "
            + "relations can be unloaded."
            + Environment.NewLine
            + "Changed relations: {1}.",
            dataContainer.ID,
            SeparatedStringBuilder.Build (", ", notUnregisterableEndPointIDs.Select (id => "'" + id.Definition.PropertyName + "'")));
        return new ExceptionCommand (new InvalidOperationException (message));
      }
      else
      {
        return new UnregisterEndPointsCommand (endPointIDs, this);
      }
    }

    public void MarkCollectionEndPointComplete (RelationEndPointID endPointID, DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("items", items);
      CheckCardinality (endPointID, CardinalityType.Many, "MarkCollectionEndPointComplete", "endPointID");
      CheckNotAnonymous (endPointID, "MarkCollectionEndPointComplete", "endPointID");

      var endPoint = (ICollectionEndPoint) GetVirtualEndPointOrRegisterEmpty (endPointID);
      endPoint.MarkDataComplete (items);
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckNotAnonymous (endPointID, "GetRelationEndPointWithLazyLoad", "endPointID");

      if (endPointID.ObjectID == null)
        return CreateNullEndPoint(ClientTransaction, endPointID.Definition);

      var existingEndPoint = this[endPointID];
      if (existingEndPoint != null)
        return _relationEndPoints[endPointID];

      if (endPointID.Definition.IsVirtual)
      {
        IVirtualEndPoint endPoint = RegisterVirtualEndPoint (endPointID);
        endPoint.EnsureDataComplete();

        Assertion.IsTrue (_relationEndPoints.ContainsKey (endPointID));
        return endPoint;
      }
      else
      {
        ClientTransaction.EnsureDataAvailable (endPointID.ObjectID); // to retrieve a real end-point, the data container must have been registered
        return Assertion.IsNotNull (this[endPointID], "Non-virtual end-points are registered when the DataContainer is loaded.");
      }
    }

    public IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID)
    {
      var existingEndPoint = this[endPointID];
      if (existingEndPoint != null)
        return existingEndPoint;
      else if (endPointID.Definition.IsVirtual)
        return GetVirtualEndPointOrRegisterEmpty (endPointID);
      else
      {
        ClientTransaction.EnsureDataAvailable (endPointID.ObjectID);
        return Assertion.IsNotNull (this[endPointID], "Non-virtual end-points are registered when the DataContainer is loaded.");
      }
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

    public IRelationEndPoint GetOppositeEndPointWithLazyLoad (IObjectEndPoint objectEndPoint, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

      if (this[objectEndPoint.ID] != objectEndPoint)
        throw new ArgumentException ("The end-point is not registered in this map.", "objectEndPoint");

      var oppositeEndPointDefinition = objectEndPoint.Definition.GetMandatoryOppositeEndPointDefinition();
      if (oppositeEndPointDefinition.IsAnonymous)
        throw new ArgumentException ("The end-point is not part of a bidirectional relation.", "objectEndPoint");

      var oppositeEndPointID = RelationEndPointID.Create (oppositeObjectID, oppositeEndPointDefinition);
      if (oppositeEndPointID.ObjectID == null)
        return CreateNullEndPoint (_clientTransaction, oppositeEndPointID.Definition);

      return GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    private IEnumerable<RelationEndPointID> GetEndPointIDsOwnedByDataContainer (DataContainer dataContainer)
    {
      // DataContainers usually own all non-virtual end-points. New DataContainers also own all virtual end-points.
      var includeVirtualEndPoints = dataContainer.State == StateType.New;
      return dataContainer.AssociatedRelationEndPointIDs.Where (endPointID => !endPointID.Definition.IsVirtual || includeVirtualEndPoints);
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
              .Select (ep => ep as IRealObjectEndPoint)
              .Where (ep => ep.OppositeObjectID != null)
              .Select (ep => RelationEndPointID.Create (ep.OppositeObjectID, ep.Definition.GetOppositeEndPointDefinition ()))
              .Select (oppositeID => this[oppositeID]);
      if (maybeOppositeEndPoint.Where (oppositeEndPoint => oppositeEndPoint.HasChanged).HasValue)
        return false;

      return true;
    }

    private void RegisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (realObjectEndPoint.OppositeObjectID == null || oppositeVirtualEndPointDefinition.IsAnonymous)
      {
        realObjectEndPoint.MarkSynchronized ();
        return;
      }

      var oppositeVirtualEndPointID = RelationEndPointID.Create (realObjectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
      var oppositeEndPoint = GetVirtualEndPointOrRegisterEmpty(oppositeVirtualEndPointID);
      oppositeEndPoint.RegisterOriginalOppositeEndPoint (realObjectEndPoint);

      // Optimization for 1:1 relations: to avoid a database query, we'll mark the virtual end-point complete when the first opposite foreign key
      // is registered with it. We can only do this in root transactions; in sub-transactions we need the query to occur so that we get the same
      // relation state in the sub-transaction as in the root transaction even in the case of unsynchronized end-points.

      var oppositeVirtualObjectEndPoint = oppositeEndPoint as IVirtualObjectEndPoint;
      if (_clientTransaction.ParentTransaction == null && oppositeVirtualObjectEndPoint != null && !oppositeVirtualObjectEndPoint.IsDataComplete)
        oppositeVirtualObjectEndPoint.MarkDataComplete (realObjectEndPoint.GetDomainObjectReference());
    }

    private void UnregisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      Assertion.IsFalse (realObjectEndPoint.HasChanged, "Deregistration currently only works for unchanged end-points");

      var oppositeVirtualEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
      Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

      if (realObjectEndPoint.OppositeObjectID != null)
      {
        var oppositeVirtualEndPointID = RelationEndPointID.Create(realObjectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
        var oppositeEndPoint = (IVirtualEndPoint) this[oppositeVirtualEndPointID];
        if (oppositeEndPoint != null)
        {
          Assertion.IsFalse (oppositeVirtualEndPointDefinition.IsAnonymous);
          oppositeEndPoint.UnregisterOriginalOppositeEndPoint (realObjectEndPoint);
          if (oppositeEndPoint.CanBeCollected)
          {
            Assertion.IsTrue (IsUnregisterable (oppositeEndPoint), "Caller checks that this end-point is unregisterable.");
            RemoveEndPoint (oppositeEndPoint.ID);
          }
        }
      }
    }

    #region Serialization

    // Note: RelationEndPointMap should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointMap (FlattenedDeserializationInfo info)
        : this (
            info.GetValueForHandle<ClientTransaction>(),
            info.GetValueForHandle<IObjectLoader>(),
            info.GetValueForHandle<IRelationEndPointLazyLoader>(),
            info.GetValueForHandle<IRelationEndPointProvider>(),
            info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>>(),
            info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>>())
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
      info.AddHandle (_objectLoader);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_collectionEndPointDataKeeperFactory);
      info.AddHandle (_virtualObjectEndPointDataKeeperFactory);

      var endPointArray = new IRelationEndPoint[Count];
      _relationEndPoints.Values.CopyTo (endPointArray, 0);
      info.AddArray (endPointArray);
    }

    #endregion
  }
}
