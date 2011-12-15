// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Creates <see cref="IRelationEndPoint"/> instances.
  /// </summary>
  [Serializable]
  public class RelationEndPointFactory : IRelationEndPointFactory
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ILazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _virtualObjectEndPointDataKeeperFactory;
    private readonly IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _collectionEndPointDataKeeperFactory;

    public RelationEndPointFactory (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> virtualObjectEndPointDataKeeperFactory,
        IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> collectionEndPointDataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("virtualObjectEndPointDataKeeperFactory", virtualObjectEndPointDataKeeperFactory);
      ArgumentUtility.CheckNotNull ("collectionEndPointDataKeeperFactory", collectionEndPointDataKeeperFactory);

      _clientTransaction = clientTransaction;
      _endPointProvider = endPointProvider;
      _lazyLoader = lazyLoader;
      _virtualObjectEndPointDataKeeperFactory = virtualObjectEndPointDataKeeperFactory;
      _collectionEndPointDataKeeperFactory = collectionEndPointDataKeeperFactory;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> VirtualObjectEndPointDataKeeperFactory
    {
      get { return _virtualObjectEndPointDataKeeperFactory; }
    }

    public IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> CollectionEndPointDataKeeperFactory
    {
      get { return _collectionEndPointDataKeeperFactory; }
    }

    public IRealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return new RealObjectEndPoint (_clientTransaction, endPointID, dataContainer, _endPointProvider);
    }

    public IVirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID, bool markDataComplete)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var virtualObjectEndPoint = new VirtualObjectEndPoint (
          _clientTransaction,
          endPointID,
          _lazyLoader,
          _endPointProvider,
          _virtualObjectEndPointDataKeeperFactory);
      if (markDataComplete)
        virtualObjectEndPoint.MarkDataComplete (null);
      return virtualObjectEndPoint;
    }

    public ICollectionEndPoint CreateCollectionEndPoint (RelationEndPointID endPointID, bool markDataComplete)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var manager = new CollectionEndPointCollectionManager (new AssociatedCollectionDataStrategyFactory (_endPointProvider));
      var collectionEndPoint = new CollectionEndPoint (
          _clientTransaction,
          endPointID,
          manager,
          _lazyLoader,
          _endPointProvider,
          _collectionEndPointDataKeeperFactory);
      if (markDataComplete)
        collectionEndPoint.MarkDataComplete (new DomainObject[0]);
      return collectionEndPoint;
    }

    public IVirtualEndPoint CreateVirtualEndPoint (RelationEndPointID endPointID, bool markDataComplete)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.Cardinality == CardinalityType.One)
        return CreateVirtualObjectEndPoint (endPointID, markDataComplete);
      else
        return CreateCollectionEndPoint (endPointID, markDataComplete);
    }

  }
}