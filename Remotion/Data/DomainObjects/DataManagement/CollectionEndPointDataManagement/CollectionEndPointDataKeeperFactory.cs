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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// The <see cref="CollectionEndPointDataKeeperFactory"/> is responsible to create a new <see cref="ICollectionEndPointDataKeeper"/> instance.
  /// </summary>
  [Serializable]
  public class CollectionEndPointDataKeeperFactory : ICollectionEndPointDataKeeperFactory
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;

    public CollectionEndPointDataKeeperFactory (
        ClientTransaction clientTransaction, 
        IRelationEndPointProvider endPointProvider, 
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);

      _clientTransaction = clientTransaction;
      _endPointProvider = endPointProvider;
      _changeDetectionStrategy = changeDetectionStrategy;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public ICollectionEndPointDataKeeper Create (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      return new CollectionEndPointDataKeeper (
          _clientTransaction, 
          endPointID, 
          _endPointProvider, 
          _changeDetectionStrategy);
    }
  }
}