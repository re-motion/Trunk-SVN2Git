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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  [Serializable]
  public class IncompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    [NonSerialized] // Workaround for flattened serialization, see CollectionEndPoint.FixupLoadState
    private readonly ICollectionEndPoint _collectionEndPoint;

    private readonly IRelationEndPointLazyLoader _lazyLoader;

    public IncompleteCollectionEndPointLoadState (ICollectionEndPoint collectionEndPoint, IRelationEndPointLazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      _collectionEndPoint = collectionEndPoint;
      _lazyLoader = lazyLoader;
    }

    public ICollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public void EnsureDataComplete ()
    {
      _lazyLoader.LoadLazyCollectionEndPoint (_collectionEndPoint);
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.GetCollectionWithOriginalData();
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.GetOppositeRelationEndPoints (dataManager);
    }

    public IDataManagementCommand CreateSetOppositeCollectionCommand (IAssociatableDomainObjectCollection newOppositeCollection)
    {
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);

      _collectionEndPoint.EnsureDataComplete ();
      throw new NotImplementedException ("TODO 3732");
      // return _collectionEndPoint.CreateSetOppositeCollectionCommand (newOppositeCollection);
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.CreateRemoveCommand (removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.CreateDeleteCommand ();
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.CreateInsertCommand (insertedRelatedObject, index);
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.CreateAddCommand (addedRelatedObject);
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);
      _collectionEndPoint.EnsureDataComplete ();
      return _collectionEndPoint.CreateReplaceCommand (index, replacementObject);
    }

    public void SetValueFrom (ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      _collectionEndPoint.EnsureDataComplete ();
      _collectionEndPoint.SetValueFrom (sourceEndPoint);
    }

    public void CheckMandatory ()
    {
      _collectionEndPoint.EnsureDataComplete ();
      _collectionEndPoint.CheckMandatory ();
    }
  }
}