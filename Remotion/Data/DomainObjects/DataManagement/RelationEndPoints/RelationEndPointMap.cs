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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Text;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
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

    private readonly RelationEndPointMap2 _relationEndPoints;
    private readonly IRelationEndPointRegistrationAgent _registrationAgent;

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

      _relationEndPoints = new RelationEndPointMap2 (_clientTransaction);
      _registrationAgent = new RelationEndPointRegistrationAgent (_endPointProvider, _relationEndPoints, _clientTransaction);
    }

    // TODO 3642: To method: GetRelationEndPointWithoutLoading
    public IRelationEndPoint this[RelationEndPointID endPointID]
    {
      get { return _relationEndPoints[endPointID]; }
    }

    // TODO 3642: Remove
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
      return _relationEndPoints[id] != null;
    }

    public void CommitAllEndPoints ()
    {
      _relationEndPoints.CommitAllEndPoints();
    }

    public void RollbackAllEndPoints ()
    {
      _relationEndPoints.RollbackAllEndPoints();
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

    // When registering a DataContainer, its real end-points are always registered, too. This may indirectly register opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are registered as well.
    public void RegisterEndPointsForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        IRelationEndPoint endPoint;
        if (!endPointID.Definition.IsVirtual)
          endPoint = CreateRealObjectEndPoint (endPointID, dataContainer);
        else
          endPoint = CreateVirtualEndPoint (endPointID, true);

        _registrationAgent.RegisterEndPoint (endPoint);
      }
    }

    // When unregistering a DataContainer, its real end-points are always unregistered. This may indirectly unregister opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are unregistered as well.
    public IDataManagementCommand GetUnregisterCommandForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var loadedEndPoints = new List<IRelationEndPoint>();
      var notUnregisterableEndPoints = new List<IRelationEndPoint>();

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        var endPoint = this[endPointID];
        if (endPoint != null)
        {
          loadedEndPoints.Add (endPoint);

          if (!_registrationAgent.IsUnregisterable (endPoint))
            notUnregisterableEndPoints.Add (endPoint);
        }
      }

      if (notUnregisterableEndPoints.Count > 0)
      {
        var message = string.Format (
            "Object '{0}' cannot be unloaded because its relations have been changed. Only unchanged objects that are not part of changed "
            + "relations can be unloaded."
            + Environment.NewLine
            + "Changed relations: {1}.",
            dataContainer.ID,
            SeparatedStringBuilder.Build (", ", notUnregisterableEndPoints.Select (endPoint => "'" + endPoint.Definition.PropertyName + "'")));
        return new ExceptionCommand (new InvalidOperationException (message));
      }
      else
      {
        return new UnregisterEndPointsCommand (loadedEndPoints, _registrationAgent);
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

    public IEnumerator<IRelationEndPoint> GetEnumerator ()
    {
      return _relationEndPoints.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    // TODO: Remove
    public void RemoveEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      _relationEndPoints.RemoveEndPoint (endPointID);
    }

    public IRelationEndPoint GetOppositeEndPointWithLazyLoad (IObjectEndPoint objectEndPoint, ObjectID oppositeObjectID)
    {
      ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

      if (this[objectEndPoint.ID] != objectEndPoint)
        throw new ArgumentException ("The end-point is not registered in this map.", "objectEndPoint");

      var oppositeEndPointDefinition = objectEndPoint.Definition.GetMandatoryOppositeEndPointDefinition();
      if (oppositeEndPointDefinition.IsAnonymous)
        throw new ArgumentException ("The end-point is not part of a bidirectional relation.", "objectEndPoint");

      if (oppositeObjectID == null)
        return CreateNullEndPoint (_clientTransaction, oppositeEndPointDefinition);

      var oppositeEndPointID = RelationEndPointID.Create (oppositeObjectID, oppositeEndPointDefinition);
      return GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    private IVirtualEndPoint GetVirtualEndPointOrRegisterEmpty (RelationEndPointID endPointID)
    {
      return (IVirtualEndPoint) this[endPointID] ?? RegisterVirtualEndPoint (endPointID);
    }

    private IVirtualEndPoint RegisterVirtualEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = CreateVirtualEndPoint (endPointID, false);
      _registrationAgent.RegisterEndPoint (endPoint);
      return endPoint;
    }

    private IRelationEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      return new RealObjectEndPoint (_clientTransaction, endPointID, dataContainer, _lazyLoader, _endPointProvider);
    }

    private IVirtualEndPoint CreateVirtualEndPoint (RelationEndPointID endPointID, bool markComplete)
    {
      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        var virtualObjectEndPoint = CreateVirtualObjectEndPoint (endPointID);
        if (markComplete)
          virtualObjectEndPoint.MarkDataComplete (null);
        return virtualObjectEndPoint;
      }
      else
      {
        var collectionEndPoint = CreateCollectionEndPoint (endPointID);
        if (markComplete)
          collectionEndPoint.MarkDataComplete (new DomainObject[0]);
        return collectionEndPoint;
      }
    }

    private VirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      return new VirtualObjectEndPoint (_clientTransaction,
                                        endPointID, _lazyLoader, _endPointProvider, _virtualObjectEndPointDataKeeperFactory);
    }

    private CollectionEndPoint CreateCollectionEndPoint (RelationEndPointID endPointID)
    {
      return new CollectionEndPoint (_clientTransaction,
                                     endPointID, _lazyLoader, _endPointProvider, _collectionEndPointDataKeeperFactory);
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

    private void CheckNotAnonymous (RelationEndPointID endPointID, string methodName, string argumentName)
    {
      if (endPointID.Definition.IsAnonymous)
      {
        var message = string.Format ("{0} cannot be called for anonymous end points.", methodName);
        throw new ArgumentException (message, argumentName);
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
      _relationEndPoints = info.GetValue<RelationEndPointMap2> ();
      _registrationAgent = new RelationEndPointRegistrationAgent (_endPointProvider, _relationEndPoints, _clientTransaction);
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
      info.AddValue (_relationEndPoints);
    }

    #endregion
  }
}
