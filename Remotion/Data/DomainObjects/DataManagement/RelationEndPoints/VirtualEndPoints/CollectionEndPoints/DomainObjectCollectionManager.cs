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
  /// Implements <see cref="IDomainObjectCollectionManager"/> by storing the active <see cref="DomainObjectCollection"/> instances for a
  /// <see cref="ClientTransaction"/> in a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  [Serializable]
  public class DomainObjectCollectionManager : IDomainObjectCollectionManager
  {
    private readonly Cache<RelationEndPointID, DomainObjectCollection> _collectionCache = new Cache<RelationEndPointID, DomainObjectCollection>();
    private readonly IAssociatedCollectionDataStrategyFactory _dataStrategyFactory;

    public DomainObjectCollectionManager (IAssociatedCollectionDataStrategyFactory dataStrategyFactory)
    {
      ArgumentUtility.CheckNotNull ("dataStrategyFactory", dataStrategyFactory);
      _dataStrategyFactory = dataStrategyFactory;
    }

    public IAssociatedCollectionDataStrategyFactory DataStrategyFactory
    {
      get { return _dataStrategyFactory; }
    }

    public DomainObjectCollection GetInitialCollection (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return _collectionCache.GetOrCreateValue (endPoint.ID, id => CreateCollection (id, _dataStrategyFactory.CreateDataStrategyForEndPoint (endPoint)));
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint endPoint, IDomainObjectCollectionData originalData)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return CreateCollection (endPoint.ID, originalData);
    }

    public void AssociateCollectionWithEndPoint (ICollectionEndPoint endPoint, DomainObjectCollection newCollection)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);

      // If the end-point's current collection is still associated with this end point, transform it to stand-alone.
      // (During rollback, the current relation might have already been associated with another end-point, we must not overwrite this!)
      var oldCollection = (IAssociatableDomainObjectCollection) endPoint.Collection;
      if (oldCollection.IsAssociatedWith (endPoint))
        oldCollection.TransformToStandAlone();

      // we must always associate the new collection with the end point, however - even during rollback phase
      ((IAssociatableDomainObjectCollection) newCollection).TransformToAssociated (endPoint, _dataStrategyFactory);
      _collectionCache.Add (endPoint.ID, newCollection);
    }

    private DomainObjectCollection CreateCollection (RelationEndPointID endPointID, IDomainObjectCollectionData dataStrategy)
    {
      return DomainObjectCollectionFactory.Instance.CreateCollection (endPointID.Definition.PropertyInfo.PropertyType, dataStrategy);
    }
  }
}