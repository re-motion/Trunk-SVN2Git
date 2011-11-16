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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents a collection-valued relation end-point in the <see cref="RelationEndPointManager"/>.
  /// </summary>
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    private readonly ILazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _dataKeeperFactory;
    
    private DomainObjectCollection _collection; // points to _dataKeeper by using EndPointDelegatingCollectionData as its data strategy
    private DomainObjectCollection _originalCollection; // keeps the original reference of the _collection for rollback
    private ICollectionEndPointLoadState _loadState; // keeps track of whether this end-point has been completely loaded or not

    private bool _hasBeenTouched;

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> dataKeeperFactory)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      _hasBeenTouched = false;
      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
      _dataKeeperFactory = dataKeeperFactory;

      _collection = CreateCollection(CreateDelegatingCollectionData ());

      _originalCollection = _collection;

      SetIncompleteLoadState ();
    }

    public DomainObjectCollection Collection
    {
      get { return _collection; }
      private set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _collection = value;
        Touch();

        ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, ID, null);
      }
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> DataKeeperFactory
    {
      get { return _dataKeeperFactory; }
    }

    public DomainObjectCollection OriginalCollection
    {
      get { return _originalCollection; }
    }

    public ReadOnlyCollectionDataDecorator GetData ()
    {
      return _loadState.GetData (this);
    }

    public ReadOnlyCollectionDataDecorator GetOriginalData ()
    {
      return _loadState.GetOriginalData (this);
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
      return Collection;
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      return CreateCollection ( _loadState.GetOriginalData (this));
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public bool CanBeCollected
    {
      get { return _loadState.CanEndPointBeCollected (this); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return _loadState.CanDataBeMarkedIncomplete (this); }
    }

    public override bool HasChanged
    {
      get { return OriginalCollection != Collection || _loadState.HasChanged(); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void EnsureDataComplete ()
    {
      _loadState.EnsureDataComplete(this);
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("items", items);
      _loadState.MarkDataComplete (this, items, SetCompleteLoadState);
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete (this, SetIncompleteLoadState);
    }

    public override void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<CollectionEndPoint> ("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      _loadState.SetDataFromSubTransaction (this, sourceCollectionEndPoint._loadState);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _loadState.Commit(this);
        _originalCollection = _collection;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        if (_collection != _originalCollection)
        {
          var command = CreateSetCollectionCommand (_originalCollection);
          command.Perform(); // no notifications, no bidirectional changes, we only change the collections' associations
        }
        Assertion.IsTrue (_collection == _originalCollection);

        //_collection.ReplaceItemsWithoutNotifications (GetOriginalData());

        _loadState.Rollback(this);
      }

      _hasBeenTouched = false;
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void ValidateMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an incomplete end-point should be 
      // checked. (DataManager will not check incomplete end-points, as it also ignores not-yet-loaded end-points.)

      if (_loadState.GetData (this).Count == 0)
      {
        var objectReference = GetDomainObjectReference ();
        var message = String.Format (
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            Definition.PropertyName,
            ObjectID);
        throw new MandatoryRelationNotSetException (objectReference, Definition.PropertyName, message);
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("comparison", comparison);
      _loadState.SortCurrentData (this, comparison);
    }

    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      var requiredItemType = Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;
      return new ModificationCheckingCollectionDataDecorator (requiredItemType, new EndPointDelegatingCollectionData (ID, EndPointProvider));
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public override bool? IsSynchronized
    {
      get { return _loadState.IsSynchronized (this); }
    }

    public override void Synchronize ()
    {
      _loadState.Synchronize (this);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint (this, oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);
      return _loadState.CreateSetCollectionCommand (this, newCollection, collection => Collection = collection);
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return _loadState.CreateRemoveCommand (this, removedRelatedObject);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _loadState.CreateDeleteCommand(this);
    }

    public virtual IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      return _loadState.CreateInsertCommand (this, insertedRelatedObject, index);
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      return _loadState.CreateAddCommand (this, addedRelatedObject);
    }

    public virtual IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);
      return _loadState.CreateReplaceCommand (this, index, replacementObject);
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _loadState.GetData (this)
             select RelationEndPointID.Create (oppositeDomainObject.ID, oppositeEndPointDefinition);
    }

    private void SetCompleteLoadState (ICollectionEndPointDataKeeper dataKeeper)
    {
      _loadState = new CompleteCollectionEndPointLoadState (dataKeeper, _endPointProvider, ClientTransaction);
    }

    private void SetIncompleteLoadState ()
    {
      _loadState = new IncompleteCollectionEndPointLoadState (_lazyLoader, _dataKeeperFactory);
    }

    private DomainObjectCollection CreateCollection (IDomainObjectCollectionData dataStrategy)
    {
      return DomainObjectCollectionFactory.Instance.CreateCollection (Definition.PropertyInfo.PropertyType, dataStrategy);
    }

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _collection = info.GetValueForHandle<DomainObjectCollection>();
      _originalCollection = info.GetValueForHandle<DomainObjectCollection> ();
      _hasBeenTouched = info.GetBoolValue();
      _lazyLoader = info.GetValueForHandle<ILazyLoader>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();
      _loadState = info.GetValue<ICollectionEndPointLoadState>();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_collection);
      info.AddHandle (_originalCollection);
      info.AddBoolValue (_hasBeenTouched);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_dataKeeperFactory);
      info.AddValue (_loadState);
    }

    #endregion
  }
}