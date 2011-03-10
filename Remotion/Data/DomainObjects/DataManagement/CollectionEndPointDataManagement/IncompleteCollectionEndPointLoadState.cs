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
using System.Collections.ObjectModel;
using System.IO;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;
using System.Linq;

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
    private readonly ICollectionEndPointDataKeeperFactory _dataKeeperFactory;

    public IncompleteCollectionEndPointLoadState (
        ICollectionEndPointDataKeeper dataKeeper, 
        IRelationEndPointLazyLoader lazyLoader, 
        ICollectionEndPointDataKeeperFactory dataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      _dataKeeper = dataKeeper;
      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;
      
      foreach (var originalOppositeEndPoint in dataKeeper.OriginalOppositeEndPoints)
        originalOppositeEndPoint.ResetSyncState ();
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public ICollectionEndPointDataKeeperFactory DataKeeperFactory
    {
      get { return _dataKeeperFactory; }
    }

    public bool IsDataComplete ()
    {
      return false;
    }

    public void EnsureDataComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      _lazyLoader.LoadLazyCollectionEndPoint (collectionEndPoint);
    }

    public void MarkDataComplete (ICollectionEndPoint collectionEndPoint, DomainObject[] items, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      Assertion.IsFalse (
          _dataKeeper.HasDataChanged(), 
          "When it is allowed to have a changed collection in incomplete state, this algorithm must be rewritten.");

      var newDataKeeper = _dataKeeperFactory.Create (_dataKeeper.EndPointID, _dataKeeper.SortExpressionBasedComparer);
      var originalOppositeEndPoints = _dataKeeper.OriginalOppositeEndPoints.ToDictionary (ep => ep.ObjectID);

      foreach (var item in items)
      {
        IObjectEndPoint oppositeEndPoint;
        if (originalOppositeEndPoints.TryGetValue (item.ID, out oppositeEndPoint))
        {
          newDataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized();
          originalOppositeEndPoints.Remove (item.ID);
        }
        else
        {
          newDataKeeper.RegisterOriginalItemWithoutEndPoint (item);
        }
      }

      newDataKeeper.SortCurrentAndOriginalData();
      stateSetter (newDataKeeper);

      foreach (var oppositeEndPointWithoutItem in originalOppositeEndPoints.Values)
        collectionEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);
    }

    public void MarkDataIncomplete (ICollectionEndPoint collectionEndPoint, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already incomplete.");
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

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetOppositeRelationEndPointIDs ();
    }

    public void RegisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.ResetSyncState ();
    }

    public void UnregisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public bool IsSynchronized (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.IsSynchronized;
    }

    public void Synchronize (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      throw new InvalidOperationException ("Cannot synchronize a collection end-point in incomplete state.");
    }

    public ReadOnlyCollection<IObjectEndPoint> GetUnsynchronizedOppositeEndPoints ()
    {
      return Array.AsReadOnly (new IObjectEndPoint[0]);
    }

    public void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      throw new InvalidOperationException ("Cannot synchronize an opposite end-point with a collection end-point in incomplete state.");
    }

    public IDataManagementCommand CreateSetCollectionCommand (ICollectionEndPoint collectionEndPoint, DomainObjectCollection newCollection, Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateSetCollectionCommand (newCollection);
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

    public bool HasChanged ()
    {
      return _dataKeeper.HasDataChanged ();
    }

    public void Commit ()
    {
      _dataKeeper.Commit ();
    }

    #region Serialization

    public IncompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader>();
      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper> ();
      _dataKeeperFactory = info.GetValueForHandle<ICollectionEndPointDataKeeperFactory> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataKeeper);
      info.AddHandle (_dataKeeperFactory);
    }

    #endregion
  }
}