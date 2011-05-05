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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Manages the <see cref="IRelationEndPoint"/> instances loaded into a <see cref="ClientTransaction"/>, encapsulating the details of registration
  /// and loading.
  /// </summary>
  public class RelationEndPointManager : IRelationEndPointManager
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
        return new NullRealObjectEndPoint (clientTransaction, endPointDefinition);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _collectionEndPointDataKeeperFactory;
    private readonly IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _virtualObjectEndPointDataKeeperFactory;

    private readonly RelationEndPointMap _map;
    private readonly IRelationEndPointRegistrationAgent _registrationAgent;

    public RelationEndPointManager (
        ClientTransaction clientTransaction,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> collectionEndPointDataKeeperFactory,
        IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> virtualObjectEndPointDataKeeperFactory,
        IRelationEndPointRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("collectionEndPointDataKeeperFactory", collectionEndPointDataKeeperFactory);
      ArgumentUtility.CheckNotNull ("virtualObjectEndPointDataKeeperFactory", virtualObjectEndPointDataKeeperFactory);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);

      _clientTransaction = clientTransaction;
      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _collectionEndPointDataKeeperFactory = collectionEndPointDataKeeperFactory;
      _virtualObjectEndPointDataKeeperFactory = virtualObjectEndPointDataKeeperFactory;
      _registrationAgent = registrationAgent;

      _map = new RelationEndPointMap (_clientTransaction);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
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

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { return _map; }
    }

    public IRelationEndPointRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
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

        _registrationAgent.RegisterEndPoint (endPoint, _map);
      }
    }

    // When unregistering a DataContainer, its real end-points are always unregistered. This may indirectly unregister opposite virtual end-points.
    // If the DataContainer is New, the virtual end-points are unregistered as well.
    public IDataManagementCommand CreateUnregisterCommandForDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var loadedEndPoints = new List<IRelationEndPoint> ();
      var notUnregisterableEndPoints = new List<IRelationEndPoint> ();

      foreach (var endPointID in GetEndPointIDsOwnedByDataContainer (dataContainer))
      {
        var endPoint = GetRelationEndPointWithoutLoading (endPointID);
        if (endPoint != null)
        {
          loadedEndPoints.Add (endPoint);

          if (!IsUnregisterable (endPoint))
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
        return new UnregisterEndPointsCommand (loadedEndPoints, _registrationAgent, _map);
      }
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.ObjectID == null)
        return CreateNullEndPoint (_clientTransaction, endPointID.Definition);

      return _map[endPointID];
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckNotAnonymous (endPointID, "GetRelationEndPointWithLazyLoad", "endPointID");

      var existingEndPoint = GetRelationEndPointWithoutLoading (endPointID);
      if (existingEndPoint != null)
        return existingEndPoint;

      var endPoint = GetRelationEndPointWithMinimumLoading (endPointID);
      endPoint.EnsureDataComplete ();
      return endPoint;
    }

    public IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      CheckNotAnonymous (endPointID, "GetRelationEndPointWithMinimumLoading", "endPointID");

      var existingEndPoint = GetRelationEndPointWithoutLoading (endPointID);
      if (existingEndPoint != null)
      {
        return existingEndPoint;
      }
      else if (endPointID.Definition.IsVirtual)
      {
        return GetVirtualEndPointOrRegisterEmpty (endPointID);
      }
      else
      {
        _lazyLoader.LoadLazyDataContainer (endPointID.ObjectID); // will trigger indirect call to RegisterEndPointsForDataContainer
        return Assertion.IsNotNull (_map[endPointID], "Non-virtual end-points are registered when the DataContainer is loaded.");
      }
    }
    
    public void CommitAllEndPoints ()
    {
      _map.CommitAllEndPoints();
    }

    public void RollbackAllEndPoints ()
    {
      _map.RollbackAllEndPoints();
    }

    public bool TrySetCollectionEndPointData (RelationEndPointID endPointID, DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("items", items);
      CheckCardinality (endPointID, CardinalityType.Many, "SetCollectionEndPointData", "endPointID");
      CheckNotAnonymous (endPointID, "SetCollectionEndPointData", "endPointID");

      var endPoint = (ICollectionEndPoint) GetVirtualEndPointOrRegisterEmpty (endPointID);
      if (endPoint.IsDataComplete)
        return false;

      endPoint.MarkDataComplete (items);
      return true;
    }

    // TODO 3634: Remove
    public void RemoveEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      _map.RemoveEndPoint (endPointID);
    }

    private IVirtualEndPoint GetVirtualEndPointOrRegisterEmpty (RelationEndPointID endPointID)
    {
      return (IVirtualEndPoint) GetRelationEndPointWithoutLoading (endPointID) ?? RegisterVirtualEndPoint (endPointID);
    }

    private IVirtualEndPoint RegisterVirtualEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = CreateVirtualEndPoint (endPointID, false);
      _registrationAgent.RegisterEndPoint (endPoint, _map);
      return endPoint;
    }

    private IRelationEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      return new RealObjectEndPoint (_clientTransaction, endPointID, dataContainer, _endPointProvider);
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
          Maybe
            .ForValue (endPoint as IRealObjectEndPoint)
            .Select (ep => RelationEndPointID.CreateOpposite (ep.Definition, ep.OppositeObjectID))
            .Where (oppositeEndPointID => !oppositeEndPointID.Definition.IsAnonymous)
            .Select (GetRelationEndPointWithoutLoading);
      if (maybeOppositeEndPoint.Where (ep => ep.HasChanged).HasValue)
        return false;

      return true;
    }

    #region Serialization

    // Note: RelationEndPointManager should never be serialized on its own; always start from the DataManager.
    protected RelationEndPointManager (FlattenedDeserializationInfo info)
        : this (
            info.GetValueForHandle<ClientTransaction>(),
            info.GetValueForHandle<ILazyLoader>(),
            info.GetValueForHandle<IRelationEndPointProvider>(),
            info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>>(),
            info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>>(),
            info.GetValueForHandle<IRelationEndPointRegistrationAgent> ())
    {
      _map = info.GetValue<RelationEndPointMap> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_collectionEndPointDataKeeperFactory);
      info.AddHandle (_virtualObjectEndPointDataKeeperFactory);
      info.AddHandle (_registrationAgent);
      info.AddValue (_map);
    }

    #endregion
  }
}
