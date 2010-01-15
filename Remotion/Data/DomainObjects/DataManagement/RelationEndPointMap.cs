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
using Remotion.FunctionalProgramming;
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

    // TODO 1914: Called by DeleteDomainObjectCommand
    public void PerformDelete (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
    {
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
      ArgumentUtility.CheckNotNull ("oppositeEndPointRemoveModifications", oppositeEndPointRemoveModifications);

      if (!ClientTransaction.IsEnlisted (deletedObject))
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
          RemoveEndPoint (endPointID);
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

    public RealObjectEndPoint RegisterRealObjectEndPoint (RelationEndPointID endPointID, DataContainer foreignKeyDataContainer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("foreignKeyDataContainer", foreignKeyDataContainer);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterRealObjectEndPoint", "endPointID");

      if (endPointID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a non-virtual end point.", "endPointID");

      var objectEndPoint = new RealObjectEndPoint (_clientTransaction, endPointID, foreignKeyDataContainer);
      Add (objectEndPoint);

      return objectEndPoint;
    }

    public VirtualObjectEndPoint RegisterVirtualObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckCardinality (endPointID, CardinalityType.One, "RegisterVirtualObjectEndPoint", "endPointID");

      if (!endPointID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a virtual end point.", "endPointID");

      var objectEndPoint = new VirtualObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID);
      Add (objectEndPoint);

      return objectEndPoint;
    }

    public CollectionEndPoint RegisterCollectionEndPoint (RelationEndPointID endPointID, IEnumerable<DomainObject> initialContents)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("initialContents", initialContents);
      CheckCardinality (endPointID, CardinalityType.Many, "RegisterCollectionEndPoint", "endPointID");

      var collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, _collectionEndPointChangeDetectionStrategy, initialContents);
      Add (collectionEndPoint);

      return collectionEndPoint;
    }

    public void RegisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State == StateType.New)
        RegisterEndPointsForNewDataContainer (dataContainer);
      else
        RegisterEndPointsForExistingDataContainer (dataContainer);
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
    
    // TODO: This method probably belongs to DataManager rather than RelationEndPointMap.
    public RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.IsAnonymous)
      {
        throw new InvalidOperationException (
            "Cannot get a RelationEndPoint for an anonymous end point definition. There are no end points for the non-existing side of unidirectional "
            + "relations.");
      }

      ClientTransaction.EnsureDataAvailable (endPointID.ObjectID); // lazily load the data before retrieving its 

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
        _clientTransaction.LoadRelatedObject (endPointID);
      }
      else
      {
        var relatedObjects = _clientTransaction.LoadRelatedObjects (endPointID);
        RegisterCollectionEndPoint (endPointID, relatedObjects);
      }

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

    public void RemoveEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (!Contains (endPointID))
      {
        var message = string.Format ("End point '{0}' is not part of this map.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }

      _transactionEventSink.RelationEndPointMapUnregistering (endPointID);
      _relationEndPoints.Remove (endPointID);
    }

    public RelationEndPointID[] GetEndPointIDsForUnload (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.Unchanged)
        throw new ArgumentException ("DataContainer must be unchanged.", "dataContainer");

      var unloadedEndPointIDs = from associatedEndPointID in dataContainer.AssociatedRelationEndPointIDs
                                from unloadedEndPointID in GetEndPointIDsForUnload (associatedEndPointID)
                                select unloadedEndPointID;
      return unloadedEndPointIDs.ToArray ();
    }

    private IEnumerable<RelationEndPointID> GetEndPointIDsForUnload (RelationEndPointID endPointID)
    {
      if (!endPointID.Definition.IsVirtual && !Contains (endPointID))
        throw new InvalidOperationException ("Real end point has not been registered.");

      var maybeRealEndPointID = Maybe.ForValue (endPointID).Where (id => !id.Definition.IsVirtual);

      var maybeVirtualNullEndPointID = 
          Maybe.ForValue (endPointID)
            .Where (id => id.Definition.IsVirtual)
            .Where (id => id.Definition.Cardinality == CardinalityType.One)
            .Select (id => (ObjectEndPoint) _relationEndPoints[id])
            .Where (endPoint => endPoint.OppositeObjectID == null)
            .Select (endPoint => endPoint.ID);

      var maybeOppositeEndPointID = 
          Maybe.ForValue (endPointID)
            .Where (id => !id.Definition.IsVirtual)
            .Where (id => id.Definition.Cardinality == CardinalityType.One)
            .Select (id => (ObjectEndPoint) _relationEndPoints[id])
            .Where (endPoint => endPoint.OppositeObjectID != null)
            .Select (endPoint => new RelationEndPointID (endPoint.OppositeObjectID, endPoint.Definition.GetOppositeEndPointDefinition ()))
            .Where (Contains);
      
      return Maybe.EnumerateValues (
          maybeRealEndPointID,
          maybeVirtualNullEndPointID,
          maybeOppositeEndPointID);
    }

    public IEnumerator GetEnumerator ()
    {
      return _relationEndPoints.GetEnumerator();
    }

    private void RegisterEndPointsForNewDataContainer (DataContainer dataContainer)
    {
      foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (endPointID.Definition.Cardinality == CardinalityType.Many)
          RegisterCollectionEndPoint (endPointID, new DomainObject[0]);
        else if (endPointID.Definition.IsVirtual)
          RegisterVirtualObjectEndPoint (endPointID, null);
        else
          RegisterRealObjectEndPoint (endPointID, dataContainer);
      }
    }

    private void RegisterEndPointsForExistingDataContainer (DataContainer dataContainer)
    {
      var realObjectEndPointIDs = from endPointDefinition in dataContainer.ClassDefinition.GetRelationEndPointDefinitions ()
                                  where !endPointDefinition.IsVirtual
                                  let endPointID = new RelationEndPointID (dataContainer.ID, endPointDefinition)
                                  select endPointID;

      foreach (var realEndPointID in realObjectEndPointIDs)
      {
        var realObjectEndPoint = RegisterRealObjectEndPoint (realEndPointID, dataContainer);

        var oppositeVirtualEndPointDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
        Assertion.IsTrue (oppositeVirtualEndPointDefinition.IsVirtual);

        if (oppositeVirtualEndPointDefinition.Cardinality == CardinalityType.One && realObjectEndPoint.OppositeObjectID != null)
        {
          var oppositeVirtualEndPointID = new RelationEndPointID (realObjectEndPoint.OppositeObjectID, oppositeVirtualEndPointDefinition);
          RegisterVirtualObjectEndPoint (oppositeVirtualEndPointID, realObjectEndPoint.ObjectID);
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
