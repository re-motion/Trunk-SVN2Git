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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    private readonly ICollectionEndPointDataKeeper _dataKeeper;
    private readonly IRelationEndPointLazyLoader _lazyLoader;

    public IncompleteCollectionEndPointLoadState (ICollectionEndPointDataKeeper dataKeeper, IRelationEndPointLazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      _dataKeeper = dataKeeper;
      _lazyLoader = lazyLoader;
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public void EnsureDataComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      _lazyLoader.LoadLazyCollectionEndPoint (collectionEndPoint);
    }

    public IDomainObjectCollectionData GetCollectionData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetCollectionData();
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetCollectionWithOriginalData();
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (ICollectionEndPoint collectionEndPoint, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetOppositeRelationEndPoints (dataManager);
    }

    public void RegisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.RegisterOriginalObject (oppositeEndPoint.GetDomainObjectReference ());
    }

    public void UnregisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.UnregisterOriginalObject (oppositeEndPoint.ObjectID);
    }

    public IDataManagementCommand CreateSetOppositeCollectionCommand (ICollectionEndPoint collectionEndPoint, IAssociatableDomainObjectCollection newOppositeCollection)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateSetOppositeCollectionCommand (newOppositeCollection);
    }

    public IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateRemoveCommand (removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateDeleteCommand ();
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateInsertCommand (insertedRelatedObject, index);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateAddCommand (addedRelatedObject);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateReplaceCommand (index, replacementObject);
    }

    public void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      collectionEndPoint.EnsureDataComplete ();
      collectionEndPoint.SetValueFrom (sourceEndPoint);
    }

    public void CheckMandatory (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      collectionEndPoint.EnsureDataComplete ();
      collectionEndPoint.CheckMandatory ();
    }

    #region Serialization

    public IncompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader>();
      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataKeeper);
    }

    #endregion
  }
}