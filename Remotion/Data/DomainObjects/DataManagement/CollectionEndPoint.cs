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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    // types

    // static members and constants

    private static DomainObjectCollection CloneDomainObjectCollection (DomainObjectCollection domainObjects, bool makeReadOnly)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      return domainObjects.Clone (makeReadOnly);
    }

    // member fields

    // this field is not serialized via IFlattenedSerializable.SerializeIntoFlatStructure
    private ICollectionEndPointChangeDelegate _changeDelegate = null;

    private readonly DomainObjectCollection _originalOppositeDomainObjects;
    private DomainObjectCollection _originalOppositeDomainObjectsReference;
    private DomainObjectCollection _oppositeDomainObjects;

    private bool _hasBeenTouched;

    // construction and disposing

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        DomainObjectCollection oppositeDomainObjects,
        ICollectionEndPointChangeDelegate changeDelegate)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      ArgumentUtility.CheckNotNull ("changeDelegate", changeDelegate);

      _originalOppositeDomainObjects = CloneDomainObjectCollection (oppositeDomainObjects, true);
      PerformReplaceOppositeCollection (oppositeDomainObjects);
      _originalOppositeDomainObjectsReference = oppositeDomainObjects;
      _hasBeenTouched = false;
      _changeDelegate = changeDelegate;
    }

    protected CollectionEndPoint (IRelationEndPointDefinition definition)
        : base (definition)
    {
      _hasBeenTouched = false;
    }

    // methods and properties

    public void ReplaceOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      if (oppositeDomainObjects == _oppositeDomainObjects)
      {
        _hasBeenTouched = true;
        return;
      }

      CheckNewOppositeCollection (oppositeDomainObjects);

      DomainObjectCollection oldOpposites = _oppositeDomainObjects;
      oldOpposites.ChangeDelegate = null;

      // temporarily set a clone of the old collection; that way, we can keep the old collection unmodified while synchronizing
      _oppositeDomainObjects = oldOpposites.Clone (false);
      _oppositeDomainObjects.ChangeDelegate = this;

      SynchronizeWithNewOppositeObjects (oppositeDomainObjects);

      PerformReplaceOppositeCollection (oppositeDomainObjects);
      _hasBeenTouched = true;
    }

    private void CheckNewOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      if (oppositeDomainObjects.ChangeDelegate != null)
        throw new InvalidOperationException ("The new opposite collection is already associated with another relation property.");

      if (_oppositeDomainObjects.GetType() != oppositeDomainObjects.GetType())
      {
        string message = string.Format (
            "The new opposite collection must have the same type as the old collection ('{0}'), but its type is '{1}'.",
            _oppositeDomainObjects.GetType().FullName,
            oppositeDomainObjects.GetType().FullName);
        throw new InvalidOperationException (message);
      }
    }

    private void PerformReplaceOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      _oppositeDomainObjects = oppositeDomainObjects;
      _oppositeDomainObjects.ChangeDelegate = this;
    }

    private void SynchronizeWithNewOppositeObjects (DomainObjectCollection newOppositeObjects)
    {
      _oppositeDomainObjects.Clear();
      foreach (DomainObject opposite in newOppositeObjects)
        _oppositeDomainObjects.Add (opposite);
    }

    public override RelationEndPoint Clone (ClientTransaction clientTransaction)
    {
      var cloneOppositeDomainObjects = DomainObjectCollection.Create (_oppositeDomainObjects.GetType(), _oppositeDomainObjects.RequiredItemType);
      var clone = new CollectionEndPoint (clientTransaction, ID, cloneOppositeDomainObjects, clientTransaction.DataManager.RelationEndPointMap);

      clone._oppositeDomainObjects.AssumeSameState (_oppositeDomainObjects);
      clone._originalOppositeDomainObjects.AssumeSameState (_originalOppositeDomainObjects);
      clone._hasBeenTouched = _hasBeenTouched;
      return clone;
    }

    protected internal override void TakeOverCommittedData (RelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<CollectionEndPoint> ("source", source);
      Assertion.IsTrue (Definition == sourceCollectionEndPoint.Definition);
      
      _oppositeDomainObjects.TakeOverCommittedData (sourceCollectionEndPoint._oppositeDomainObjects);
      _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched || HasChanged;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _originalOppositeDomainObjects.Commit (_oppositeDomainObjects);
        _originalOppositeDomainObjectsReference = _oppositeDomainObjects;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _oppositeDomainObjects.ChangeDelegate = null;
        PerformReplaceOppositeCollection (_originalOppositeDomainObjectsReference);
        _oppositeDomainObjects.Rollback (_originalOppositeDomainObjects);
      }

      _hasBeenTouched = false;
    }

    public override bool HasChanged
    {
      get
      {
        return OppositeDomainObjects != OriginalOppositeDomainObjectsReference
               || ClientTransaction.HasCollectionEndPointDataChanged (OppositeDomainObjects, OriginalOppositeDomainObjects);
      }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
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
            GetDomainObject(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            PropertyName,
            ObjectID);
      }
    }

    public override IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return new CollectionEndPointRemoveModification (this, removedRelatedObject, _oppositeDomainObjects._data);
    }

    public override IRelationEndPointModification CreateSelfReplaceModification (DomainObject selfReplaceRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("selfReplaceRelatedObject", selfReplaceRelatedObject);
      return new CollectionEndPointSelfReplaceModification (this, selfReplaceRelatedObject, _oppositeDomainObjects._data);
    }

    public virtual IRelationEndPointModification CreateInsertModification (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      return new CollectionEndPointInsertModification (this, insertedRelatedObject, index, _oppositeDomainObjects._data);
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
        return new CollectionEndPointSelfReplaceModification (this, replacedObject, _oppositeDomainObjects._data);
      else
        return new CollectionEndPointReplaceModification (this, replacedObject, index, replacementObject, _oppositeDomainObjects._data);
    }

    public override void PerformDelete ()
    {
      _oppositeDomainObjects.PerformDelete();
      _hasBeenTouched = true;
    }

    public DomainObjectCollection OriginalOppositeDomainObjects
    {
      get { return _originalOppositeDomainObjects; }
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsReference
    {
      get { return _originalOppositeDomainObjectsReference; }
    }

    public DomainObjectCollection OppositeDomainObjects
    {
      get { return _oppositeDomainObjects; }
    }

    public ICollectionEndPointChangeDelegate ChangeDelegate
    {
      get { return _changeDelegate; }
    }

    #region ICollectionChangeDelegate Members

    void ICollectionChangeDelegate.PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index)
    {
      ChangeDelegate.PerformInsert (this, domainObject, index);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject newDomainObject, int index)
    {
      ChangeDelegate.PerformReplace (this, index, newDomainObject);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
    {
      ChangeDelegate.PerformRemove (this, domainObject);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.MarkAsTouched ()
    {
      _hasBeenTouched = true;
    }

    #endregion

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _originalOppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _originalOppositeDomainObjects.ChangeDelegate = this;
      _oppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _oppositeDomainObjects.ChangeDelegate = this;
      _originalOppositeDomainObjectsReference = info.GetValueForHandle<DomainObjectCollection>();
      _hasBeenTouched = info.GetBoolValue();
      _changeDelegate = info.GetValueForHandle<ICollectionEndPointChangeDelegate>();

      // if _changeDelegate == null, we didn't serialize the back-reference to the RelationEndPointMap, so get it from the ClientTransaction
      // only do that after deserialization has finished
      if (_changeDelegate == null)
        info.DeserializationFinished += ((sender, args) => _changeDelegate = ClientTransaction.DataManager.RelationEndPointMap);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_originalOppositeDomainObjects);
      info.AddHandle (_oppositeDomainObjects);
      info.AddHandle (_originalOppositeDomainObjectsReference);
      info.AddBoolValue (_hasBeenTouched);

      // cannot serialize back-references, therefore, we only add the ChangeDelegate if it is not the (default) back-reference to 
      // ClientTransaction.DataManager.RelationEndPointMap
      info.AddHandle (_changeDelegate == ClientTransaction.DataManager.RelationEndPointMap ? null : _changeDelegate);
    }

    #endregion
  }
}
