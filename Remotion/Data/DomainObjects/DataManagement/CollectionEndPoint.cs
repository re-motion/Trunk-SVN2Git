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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a collection-valued relation end-point in the <see cref="RelationEndPointMap"/>.
  /// </summary>
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    private readonly ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly ICollectionEndPointDataKeeper _dataKeeper; // stores the data kept by _collection and the original data for rollback

    private DomainObjectCollection _collection; // points to _dataKeeper by using EndPointDelegatingCollectionData as its data strategy
    private DomainObjectCollection _originalCollection; // keeps the original reference of the _collection for rollback
    private ICollectionEndPointLoadState _loadState; // keeps track of whether this end-point has been completely loaded or not

    private bool _hasBeenTouched;

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IRelationEndPointLazyLoader lazyLoader,
        IEnumerable<DomainObject> initialContentsOrNull)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      // TODO 3653: Inject DataKeeper from the outside
      _dataKeeper = CreateDataKeeper (clientTransaction, id, initialContentsOrNull ?? Enumerable.Empty<DomainObject> ());

      var collectionType = id.Definition.PropertyType;
      var dataStrategy = CreateDelegatingCollectionData();
      _collection = DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, dataStrategy);

      _originalCollection = _collection;

      _hasBeenTouched = false;
      _changeDetectionStrategy = changeDetectionStrategy;
      _lazyLoader = lazyLoader;
      
      if (initialContentsOrNull != null)
        SetCompleteLoadState();
      else
        SetIncompleteLoadState();
    }

    public DomainObjectCollection Collection
    {
      get { return _collection; }
      private set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _collection = value;
        Touch();

        RaiseStateUpdateNotification (HasChanged);
      }
    }

    public DomainObjectCollection OriginalCollection
    {
      get { return _originalCollection; }
    }

    public IDomainObjectCollectionData GetCollectionData ()
    {
      return _loadState.GetCollectionData (this);
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      return _loadState.GetCollectionWithOriginalData(this);
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public override bool HasChanged
    {
      get { return OriginalCollection != Collection || _dataKeeper.HasDataChanged (_changeDetectionStrategy); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void EnsureDataComplete ()
    {
      _loadState.EnsureDataComplete(this);
    }

    public void MarkDataComplete ()
    {
      _loadState.MarkDataComplete (this, SetCompleteLoadState);
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete (this, SetIncompleteLoadState);
    }

    public override void SetValueFrom (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<ICollectionEndPoint> ("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      _loadState.SetValueFrom (this, sourceCollectionEndPoint);
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _dataKeeper.CommitOriginalContents();
        _originalCollection = _collection;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        var oppositeObjectsReferenceBeforeRollback = _collection;

        if (_originalCollection != _collection)
        {
          var command = CreateSetCollectionCommand (_originalCollection);
          command.Perform(); // no notifications, no bidirectional changes, we only change the collections' associations
        }

        _collection = _originalCollection;

        Assertion.IsTrue (_collection.IsAssociatedWith (this));
        Assertion.IsTrue (
            _collection == oppositeObjectsReferenceBeforeRollback
            || !oppositeObjectsReferenceBeforeRollback.IsAssociatedWith (this));

        _collection.ReplaceItemsWithoutNotifications (GetCollectionWithOriginalData().Cast<DomainObject>());
      }

      _hasBeenTouched = false;
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void CheckMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an incomplete end-point should be 
      // checked. (DataManager will not check incomplete end-points, as it also ignores not-yet-loaded end-points.)
      _loadState.CheckMandatory(this);
    }

    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      var requiredItemType = Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;
      return new ModificationCheckingCollectionDataDecorator (requiredItemType, new EndPointDelegatingCollectionData (this));
    }

    public void RegisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.RegisterOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.UnregisterOppositeEndPoint (this, oppositeEndPoint);
    }

    public override bool IsSynchronized
    {
      get { return !GetUnsynchronizedOppositeEndPoints().Any(); }
    }

    public ReadOnlyCollection<IObjectEndPoint> GetUnsynchronizedOppositeEndPoints ()
    {
      return _loadState.GetUnsynchronizedOppositeEndPoints();
    }

    public override void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint (oppositeEndPoint);
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
      return _loadState.GetOppositeRelationEndPointIDs (this);
    }

    private void RaiseStateUpdateNotification (bool hasChanged)
    {
      ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, ID, hasChanged);
    }

    private ICollectionEndPointDataKeeper CreateDataKeeper (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        IEnumerable<DomainObject> initialContents)
    {
      var sortExpression = ((VirtualRelationEndPointDefinition) id.Definition).GetSortExpression();
      // Only root transactions use the sort expression (if any)
      var sortExpressionBasedComparer = sortExpression == null || clientTransaction.ParentTransaction != null
                                            ? null
                                            : SortedPropertyComparer.CreateCompoundComparer (
                                                sortExpression.SortedProperties, clientTransaction.DataManager);
      return new CollectionEndPointDataKeeper (clientTransaction, id, sortExpressionBasedComparer, initialContents);
    }

    private void SetCompleteLoadState ()
    {
      _loadState = new CompleteCollectionEndPointLoadState (_dataKeeper, ClientTransaction);
    }

    private void SetIncompleteLoadState ()
    {
      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeper, _lazyLoader);
    }

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _collection = info.GetValueForHandle<DomainObjectCollection>();
      _originalCollection = info.GetValueForHandle<DomainObjectCollection> ();
      _hasBeenTouched = info.GetBoolValue();
      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper>();
      _changeDetectionStrategy = info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy>();
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader>();
      _loadState = info.GetValue<ICollectionEndPointLoadState>();

      FixupDomainObjectCollection (_collection);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_collection);
      info.AddHandle (_originalCollection);
      info.AddBoolValue (_hasBeenTouched);
      info.AddHandle (_dataKeeper);
      info.AddHandle (_changeDetectionStrategy);
      info.AddHandle (_lazyLoader);
      info.AddValue (_loadState);
    }

    private void FixupDomainObjectCollection (DomainObjectCollection collection)
    {
      // The reason we need to do a fix up for associated collections is that:
      // - CollectionEndPoint is not serializable; it is /flattened/ serializable (for performance reasons); this means no reference to it can be held
      //   by a serializable object.
      // - DomainObjectCollection is serializable, not flattened serializable.
      // - Therefore, EndPointDelegatingCollectionData can only be serializable, not flattened serializable.
      // - Therefore, EndPointDelegatingCollectionData's back-reference to CollectionEndPoint cannot be serialized. (It is marked as [NonSerializable].)
      // - Therefore, it needs to be fixed up manually when the end point is restored.

      // Fixups could be avoided if DomainObjectCollection and all IDomainObjectCollectionData implementations were made flattened serializable, 
      // but that would be complex and it would impose the details of flattened serialization to re-store's users. Won't happen.
      // Fixups could also be avoided if the end points stop being flattened serializable. For that, however, they must lose any references they 
      // currently have to RelationEndPointMap. Will probably happen in the future.
      // If it doesn't happen, fixups can be made prettier by adding an IAssociatedEndPointFixup interface to DomainObjectCollection and all 
      // IDomainObjectCollectionData implementors.

      var dataField = typeof (DomainObjectCollection).GetField ("_dataStrategy", BindingFlags.NonPublic | BindingFlags.Instance);
      var decorator = dataField.GetValue (collection);

      var wrappedDataField = typeof (DomainObjectCollectionDataDecoratorBase).GetField (
          "_wrappedData", BindingFlags.NonPublic | BindingFlags.Instance);
      var endPointDelegatingData = (EndPointDelegatingCollectionData) wrappedDataField.GetValue (decorator);

      var associatedEndPointField = typeof (EndPointDelegatingCollectionData).GetField (
          "_associatedEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
      associatedEndPointField.SetValue (endPointDelegatingData, this);

    }

    #endregion
  }
}