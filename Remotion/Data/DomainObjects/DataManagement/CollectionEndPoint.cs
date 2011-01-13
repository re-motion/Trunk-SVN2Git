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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    private readonly ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;
    private readonly ICollectionEndPointDataKeeper _dataKeeper; // stores the data kept by _oppositeDomainObjects and the original data for rollback

    private DomainObjectCollection _oppositeDomainObjects; // points to _dataKeeper by using EndPointDelegatingCollectionData as its data strategy
    private DomainObjectCollection _originalCollectionReference; // keeps the original reference of the _oppositeDomainObjects for rollback

    private bool _hasBeenTouched;

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IEnumerable<DomainObject> initialContentsOrNull)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);

      // TODO 3401: Inject DataKeeper from the outside
      _dataKeeper = CreateDataKeeper (clientTransaction, id, initialContentsOrNull);

      var collectionType = id.Definition.PropertyType;
      var dataStrategy = CreateDelegatingCollectionData ();
      _oppositeDomainObjects = DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, dataStrategy);

      _originalCollectionReference = _oppositeDomainObjects;
      
      _hasBeenTouched = false;
      _changeDetectionStrategy = changeDetectionStrategy;
    }

    public DomainObjectCollection OppositeDomainObjects
    {
      get { return _oppositeDomainObjects; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (_oppositeDomainObjects.GetType () != value.GetType ())
          throw new ArgumentTypeException ("value", _oppositeDomainObjects.GetType (), value.GetType ());

        if (!value.IsAssociatedWith (this))
        {
          throw new ArgumentException (
              "The new opposite collection must have been prepared to delegate to this end point. Use SetOppositeCollectionAndNotify instead.",
              "value");
        }

        _oppositeDomainObjects = value;
        Touch ();

        RaiseStateUpdateNotification (HasChanged);
      }
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsContents
    {
      get 
      {
        EnsureDataAvailable ();

        var collectionType = Definition.PropertyType;
        return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _dataKeeper.OriginalCollectionData);
      }
    }

    public DomainObjectCollection OriginalCollectionReference
    {
      get { return _originalCollectionReference; }
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public override bool IsDataAvailable
    {
      get { return _dataKeeper.IsDataAvailable; }
    }

    public override bool HasChanged
    {
      get { return OriginalCollectionReference != OppositeDomainObjects || _dataKeeper.HasDataChanged (_changeDetectionStrategy); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public void Unload ()
    {
      if (IsDataAvailable)
      {
        ClientTransaction.TransactionEventSink.RelationEndPointUnloading (ClientTransaction, this);
        _dataKeeper.Unload();
      }
    }

    public override void EnsureDataAvailable ()
    {
      _dataKeeper.EnsureDataAvailable ();
    }

    public void MarkDataAvailable ()
    {
      _dataKeeper.MarkDataAvailable ();
    }

    public void SetOppositeCollectionAndNotify (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      if (!oppositeDomainObjects.IsAssociatedWith (null) && !oppositeDomainObjects.IsAssociatedWith (this))
        throw new ArgumentException ("The given collection is already associated with an end point.", "oppositeDomainObjects");

      DomainObjectCheckUtility.EnsureNotDeleted (this.GetDomainObjectReference(), ClientTransaction);

      EnsureDataAvailable ();

      var command = ((IAssociatableDomainObjectCollection) oppositeDomainObjects).CreateAssociationCommand (this);
      var bidirectionalModification = command.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();
    }

    public override void SetValueFrom (RelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<CollectionEndPoint> ("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.", 
            source.ID);
        throw new ArgumentException (message, "source");
      }

      EnsureDataAvailable ();
      sourceCollectionEndPoint.EnsureDataAvailable ();

      _dataKeeper.CollectionData.ReplaceContents (sourceCollectionEndPoint._dataKeeper.CollectionData);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _dataKeeper.CommitOriginalContents ();
        _originalCollectionReference = _oppositeDomainObjects;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        var oppositeObjectsReferenceBeforeRollback = _oppositeDomainObjects;

        if (_originalCollectionReference != _oppositeDomainObjects)
        {
          var command = ((IAssociatableDomainObjectCollection) _originalCollectionReference).CreateAssociationCommand (this);
          command.Perform(); // no notifications, no bidirectional changes, we only change the collections' associations
        }

        _oppositeDomainObjects = _originalCollectionReference;

        Assertion.IsTrue (_oppositeDomainObjects.IsAssociatedWith (this));
        Assertion.IsTrue (
            _oppositeDomainObjects == oppositeObjectsReferenceBeforeRollback 
            || !oppositeObjectsReferenceBeforeRollback.IsAssociatedWith (this));

        _oppositeDomainObjects.ReplaceItemsWithoutNotifications (OriginalOppositeDomainObjectsContents.Cast<DomainObject>());
      }

      _hasBeenTouched = false;
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void CheckMandatory ()
    {
      // In order to perform the mandatory check, we need to load data. It's up to the caller to decide whether an unloaded end-point should be 
      // checked. (DataManager will not check unloaded end-points, as it also ignores not-yet-loaded end-points.)
      EnsureDataAvailable ();

      if (_dataKeeper.CollectionData.Count == 0)
      {
        throw CreateMandatoryRelationNotSetException (
            this.GetDomainObjectReference(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            PropertyName,
            ObjectID);
      }
    }

    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      var requiredItemType = Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;
      var dataStrategy = new ModificationCheckingCollectionDataDecorator (requiredItemType, new EndPointDelegatingCollectionData (this, _dataKeeper.CollectionData));

      return dataStrategy;
    }

    public void RegisterOriginalObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _dataKeeper.RegisterOriginalObject (domainObject);
    }

    public void UnregisterOriginalObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _dataKeeper.UnregisterOriginalObject (objectID);
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      EnsureDataAvailable ();
      return new CollectionEndPointRemoveCommand (this, removedRelatedObject, _dataKeeper.CollectionData);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      EnsureDataAvailable ();
      
      return new AdHocCommand
          {
            BeginHandler = () => ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).BeginDelete (),
            PerformHandler = () => { _dataKeeper.CollectionData.Clear (); Touch (); },
            EndHandler = () => ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).EndDelete ()
          };
    }

    public virtual IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      EnsureDataAvailable ();
      return new CollectionEndPointInsertCommand (this, index, insertedRelatedObject, _dataKeeper.CollectionData);
    }

    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      EnsureDataAvailable ();
      return CreateInsertCommand (addedRelatedObject, OppositeDomainObjects.Count);
    }

    public virtual IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      EnsureDataAvailable ();

      var replacedObject = OppositeDomainObjects[index];
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (this, replacedObject, _dataKeeper.CollectionData);
      else
        return new CollectionEndPointReplaceCommand (this, replacedObject, index, replacementObject, _dataKeeper.CollectionData);
    }

    public override IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      EnsureDataAvailable ();

      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in OppositeDomainObjects.Cast<DomainObject> ()
             let oppositeEndPointID = new RelationEndPointID (oppositeDomainObject.ID, oppositeEndPointDefinition)
             select dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    private void RaiseStateUpdateNotification (bool hasChanged)
    {
      ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, ID, hasChanged);
    }

    private ICollectionEndPointDataKeeper CreateDataKeeper (
        ClientTransaction clientTransaction, 
        RelationEndPointID id, 
      IEnumerable<DomainObject> initialContentsOrNull)
    {
      var sortExpression = ((VirtualRelationEndPointDefinition) id.Definition).GetSortExpression ();
      // Only root transactions use the sort expression (if any)
      var sortExpressionBasedComparer = sortExpression == null || clientTransaction.ParentTransaction != null
          ? null 
          : SortedPropertyComparer.CreateCompoundComparer (sortExpression.SortedProperties, clientTransaction.DataManager);
      return new LazyLoadingCollectionEndPointDataKeeper (clientTransaction, id, sortExpressionBasedComparer, initialContentsOrNull);
    }

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _oppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _originalCollectionReference = info.GetValueForHandle<DomainObjectCollection>();
      _hasBeenTouched = info.GetBoolValue();
      _dataKeeper = info.GetValue<ICollectionEndPointDataKeeper> ();
      _changeDetectionStrategy = info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy> ();

      FixupAssociatedEndPoint (_oppositeDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_oppositeDomainObjects);
      info.AddHandle (_originalCollectionReference);
      info.AddBoolValue (_hasBeenTouched);
      info.AddValue (_dataKeeper);
      info.AddHandle (_changeDetectionStrategy);
    }

    private void FixupAssociatedEndPoint (DomainObjectCollection collection)
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

      var wrappedDataField = typeof (DomainObjectCollectionDataDecoratorBase).GetField ("_wrappedData", BindingFlags.NonPublic | BindingFlags.Instance);
      var endPointDelegatingData = (EndPointDelegatingCollectionData) wrappedDataField.GetValue (decorator);

      var associatedEndPointField = typeof (EndPointDelegatingCollectionData).GetField ("_associatedEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
      associatedEndPointField.SetValue (endPointDelegatingData, this);

      var endPointDataField = typeof (EndPointDelegatingCollectionData).GetField ("_endPointData", BindingFlags.NonPublic | BindingFlags.Instance);
      endPointDataField.SetValue (endPointDelegatingData, _dataKeeper.CollectionData);
    }

    #endregion
  }
}
