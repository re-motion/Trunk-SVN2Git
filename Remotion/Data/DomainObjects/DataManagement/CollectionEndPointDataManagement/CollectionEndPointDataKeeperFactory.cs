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
  public class CollectionEndPointDataKeeperFactory : ICollectionEndPointDataKeeperFactory
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointProvider _endPointProvider;

    public CollectionEndPointDataKeeperFactory (ClientTransaction clientTransaction, IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _clientTransaction = clientTransaction;
      _endPointProvider = endPointProvider;
    }

    public ICollectionEndPointDataKeeper Create (RelationEndPointID endPointID, IComparer<DomainObject> sortExpressionBasedComparer)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("sortExpressionBasedComparer", sortExpressionBasedComparer);

      return new CollectionEndPointDataKeeper (_clientTransaction, endPointID, sortExpressionBasedComparer, _endPointProvider);
    }
  }
}