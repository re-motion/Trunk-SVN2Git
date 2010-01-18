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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    private readonly LazyLoadableCollectionEndPointData _data; // stores the data kept by _oppositeDomainObjects and the original data for rollback

    private DomainObjectCollection _oppositeDomainObjects; // points to _data by using EndPointDelegatingCollectionData as its data strategy
    private DomainObjectCollection _originalCollectionReference; // keeps the original reference of the _oppositeDomainObjects for rollback

    private bool _hasBeenTouched;

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IEnumerable<DomainObject> initialContents)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      _data = new LazyLoadableCollectionEndPointData (clientTransaction, id, changeDetectionStrategy, initialContents);

      var factory = new DomainObjectCollectionFactory ();
      var collectionType = id.Definition.PropertyType;
      var dataStrategy = CreateDelegatingCollectionData ();
      _oppositeDomainObjects = factory.CreateCollection (collectionType, dataStrategy);

      _originalCollectionReference = _oppositeDomainObjects;
      
      _hasBeenTouched = false;
    }

    public DomainObjectCollection OppositeDomainObjects
    {
      get { return _oppositeDomainObjects; }
      set
      {
        ArgumentUtility.CheckNotNullAndType ("value", value, _oppositeDomainObjects.GetType ());

        if (value.AssociatedEndPoint != this)
        {
          throw new ArgumentException (
              "The new opposite collection must have been prepared to delegate to this end point. Use SetOppositeCollectionAndNotify instead.",
              "value");
        }

        _oppositeDomainObjects = value;
        Touch ();
      }
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsContents
    {
      get { return _data.OriginalOppositeDomainObjectsContents; }
    }

    public DomainObjectCollection OriginalCollectionReference
    {
      get { return _originalCollectionReference; }
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _data.ChangeDetectionStrategy; }
    }

    public override bool IsDataAvailable
    {
      get { return _data.IsDataAvailable; }
    }

    public override bool HasChanged
    {
      get { return _data.HasChanged (this); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    private IDomainObjectCollectionData DataStore
    {
      get { return _data.DataStore; }
    }

    public void Unload ()
    {
      _data.Unload ();
    }

    public override void EnsureDataAvailable ()
    {
      _data.EnsureDataAvailable ();
    }

    public void SetOppositeCollectionAndNotify (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);

      if (oppositeDomainObjects.AssociatedEndPoint != null && oppositeDomainObjects.AssociatedEndPoint != this)
        throw new ArgumentException ("The given collection is already associated with an end point.", "oppositeDomainObjects");

      RelationEndPointValueChecker.CheckNotDeleted (this, this.GetDomainObject ());

      var modification = oppositeDomainObjects.CreateAssociationModification (this);
      var bidirectionalModification = modification.CreateRelationModification ();
      bidirectionalModification.ExecuteAllSteps ();
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

      DataStore.ReplaceContents (sourceCollectionEndPoint.DataStore);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        OriginalOppositeDomainObjectsContents.Commit (_oppositeDomainObjects);
        _originalCollectionReference = _oppositeDomainObjects;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        var oppositeObjectsReferenceBeforeRollback = _oppositeDomainObjects;

        var modification = _originalCollectionReference.CreateAssociationModification (this);
        modification.Perform (); // no notifications, no bidirectional changes, we only change the collections' associations

        _oppositeDomainObjects = _originalCollectionReference;

        Assertion.IsTrue (_oppositeDomainObjects.AssociatedEndPoint == this);
        Assertion.IsTrue (_oppositeDomainObjects == oppositeObjectsReferenceBeforeRollback || oppositeObjectsReferenceBeforeRollback.AssociatedEndPoint != this);

        _oppositeDomainObjects.Rollback (OriginalOppositeDomainObjectsContents);
      }

      _hasBeenTouched = false;
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void CheckMandatory ()
    {
      if (_oppositeDomainObjects.Count == 0)
      {
        throw CreateMandatoryRelationNotSetException (
            this.GetDomainObject(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            PropertyName,
            ObjectID);
      }
    }

    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      var requiredItemType = Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;
      var dataStrategy = new ArgumentCheckingCollectionDataDecorator (requiredItemType, new EndPointDelegatingCollectionData (this, _data));

      return dataStrategy;
    }

    public override IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return new CollectionEndPointRemoveModification (this, removedRelatedObject, DataStore);
    }

    public virtual IRelationEndPointModification CreateInsertModification (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      return new CollectionEndPointInsertModification (this, index, insertedRelatedObject, DataStore);
    }

    public virtual IRelationEndPointModification CreateAddModification (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      return CreateInsertModification (addedRelatedObject, OppositeDomainObjects.Count);
    }

    public virtual IRelationEndPointModification CreateReplaceModification (int index, DomainObject replacementObject)
    {
      var replacedObject = OppositeDomainObjects[index];
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameModification (this, replacedObject, DataStore);
      else
        return new CollectionEndPointReplaceModification (this, replacedObject, index, replacementObject, DataStore);
    }

    public override void PerformDelete ()
    {
      Assertion.IsFalse (_oppositeDomainObjects.IsReadOnly);

      ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).BeginDelete ();
      DataStore.Clear ();
      _hasBeenTouched = true;
      ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).EndDelete ();
    }

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _oppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _originalCollectionReference = info.GetValueForHandle<DomainObjectCollection>();
      _hasBeenTouched = info.GetBoolValue();
      _data = info.GetValue<LazyLoadableCollectionEndPointData> ();

      FixupAssociatedEndPoint (_oppositeDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_oppositeDomainObjects);
      info.AddHandle (_originalCollectionReference);
      info.AddBoolValue (_hasBeenTouched);
      info.AddValue (_data);
    }

    private void FixupAssociatedEndPoint (DomainObjectCollection collection)
    {
      // The reason we need to do a fix up for associated collections is that:
      // - CollectionEndPoint is not serializable; it is /flattened/ serializable (for performance reasons); this means no reference to it can be
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
      endPointDataField.SetValue (endPointDelegatingData, _data);
    }

    #endregion
  }
}
