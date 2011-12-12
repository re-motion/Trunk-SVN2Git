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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements <see cref="IDomainObjectCollectionProvider"/> by storing the active <see cref="DomainObjectCollection"/> instances for a
  /// <see cref="ClientTransaction"/> in a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  public class DomainObjectCollectionProvider : IDomainObjectCollectionProvider
  {
    private readonly Cache<RelationEndPointID, DomainObjectCollection> _collectionCache = new Cache<RelationEndPointID, DomainObjectCollection>();

    public DomainObjectCollection GetCollection (RelationEndPointID endPointID, Func<IDomainObjectCollectionData> dataStrategyProvider)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("dataStrategyProvider", dataStrategyProvider);

      return _collectionCache.GetOrCreateValue (endPointID, id => CreateCollection (id, dataStrategyProvider()));
    }

    public DomainObjectCollection GetCollectionWithOriginalData (RelationEndPointID endPointID, Func<IDomainObjectCollectionData> originalDataStrategyProvider)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("originalDataStrategyProvider", originalDataStrategyProvider);

      return CreateCollection (endPointID, originalDataStrategyProvider());
    }

    public void RegisterCollection (RelationEndPointID endPointID, DomainObjectCollection domainObjectCollection)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("domainObjectCollection", domainObjectCollection);

      _collectionCache.Add (endPointID, domainObjectCollection);
    }

    private DomainObjectCollection CreateCollection (RelationEndPointID id, IDomainObjectCollectionData dataStrategy)
    {
      return DomainObjectCollectionFactory.Instance.CreateCollection (id.Definition.PropertyInfo.PropertyType, dataStrategy);
    }
  }
}