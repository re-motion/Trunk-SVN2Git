/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionChangeDelegate
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
        DomainObjectCollection oppositeDomainObjects)
        : this (clientTransaction, id, oppositeDomainObjects, CloneDomainObjectCollection (oppositeDomainObjects, true))
    {
    }

    private CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        DomainObjectCollection oppositeDomainObjects,
        DomainObjectCollection originalOppositeDomainObjects)
        : base (clientTransaction, id)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      ArgumentUtility.CheckNotNull ("originalOppositeDomainObjects", originalOppositeDomainObjects);

      _originalOppositeDomainObjects = originalOppositeDomainObjects;
      _oppositeDomainObjects = oppositeDomainObjects;
      _oppositeDomainObjects.ChangeDelegate = this;
      _originalOppositeDomainObjectsReference = oppositeDomainObjects;
      _hasBeenTouched = false;
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

      CheckNewOppositeCollection(oppositeDomainObjects);

      DomainObjectCollection oldOpposites = _oppositeDomainObjects;
      oldOpposites.ChangeDelegate = null;

      // temporarily set a clone of the old collection; that way, we can keep the old collection unmodified while synchronizing
      _oppositeDomainObjects = oldOpposites.Clone (false);
      _oppositeDomainObjects.ChangeDelegate = this;
      SynchronizeWithNewOppositeObjects (oppositeDomainObjects);

      _oppositeDomainObjects = oppositeDomainObjects;
      _oppositeDomainObjects.ChangeDelegate = this;
      _hasBeenTouched = true;
    }

    private void CheckNewOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      if (oppositeDomainObjects.ChangeDelegate != null)
        throw new InvalidOperationException ("The new opposite collection is already associated with another relation property.");

      if (_oppositeDomainObjects.GetType () != oppositeDomainObjects.GetType ())
      {
        string message = string.Format ("The new opposite collection must have the same type as the old collection ('{0}'), but its type is '{1}'.", 
            _oppositeDomainObjects.GetType ().FullName, oppositeDomainObjects.GetType ().FullName);
        throw new InvalidOperationException (message);
      }
    }

    private void SynchronizeWithNewOppositeObjects (DomainObjectCollection newOppositeObjects)
    {
      _oppositeDomainObjects.Clear ();
      foreach (DomainObject opposite in newOppositeObjects)
        _oppositeDomainObjects.Add (opposite);
    }

    public override RelationEndPoint Clone ()
    {
      CollectionEndPoint clone = new CollectionEndPoint (
          ClientTransaction, ID, DomainObjectCollection.Create (_oppositeDomainObjects.GetType(), _oppositeDomainObjects.RequiredItemType));
      clone.AssumeSameState (this);
      clone.ChangeDelegate = ChangeDelegate;

      return clone;
    }

    protected internal override void AssumeSameState (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

      _oppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._oppositeDomainObjects);
      _originalOppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._originalOppositeDomainObjects);
      _hasBeenTouched = sourceCollectionEndPoint._hasBeenTouched;
    }

    protected internal override void TakeOverCommittedData (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

      _oppositeDomainObjects.TakeOverCommittedData (sourceCollectionEndPoint._oppositeDomainObjects);
      _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched || HasChanged;
    }

    protected internal override void RegisterWithMap (RelationEndPointMap map)
    {
      ChangeDelegate = map;
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
        ReplaceOppositeCollection (_originalOppositeDomainObjectsReference);
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

    protected internal override void Touch ()
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

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForAddOrRemove (_oppositeDomainObjects, oldEndPoint, newEndPoint));
    }

    public virtual RelationEndPointModification CreateInsertModification (IEndPoint oldEndPoint, IEndPoint newEndPoint, int index)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForInsert (_oppositeDomainObjects, oldEndPoint, newEndPoint, index));
    }

    public virtual RelationEndPointModification CreateReplaceModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForReplace (
              _oppositeDomainObjects, oldEndPoint, newEndPoint, _oppositeDomainObjects.IndexOf (oldEndPoint.GetDomainObject())));
    }

    public virtual void PerformRelationChange (CollectionEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);

      modification.ChangeAgent.PerformRelationChange();
      _hasBeenTouched = true;
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
      set { _changeDelegate = value; }
    }

    #region ICollectionChangeDelegate Members

    void ICollectionChangeDelegate.PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index)
    {
      if (_changeDelegate == null)
        throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

      _changeDelegate.PerformInsert (this, domainObject, index);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
    {
      if (_changeDelegate == null)
        throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

      _changeDelegate.PerformReplace (this, domainObject, index);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformSelfReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
    {
      if (_changeDelegate == null)
        throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

      _changeDelegate.PerformSelfReplace (this, domainObject, index);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
    {
      if (_changeDelegate == null)
        throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

      _changeDelegate.PerformRemove (this, domainObject);
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
      Type collectionType = info.GetValueForHandle<Type>();
      _originalOppositeDomainObjects = (DomainObjectCollection) 
          TypesafeActivator.CreateInstance (collectionType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With();
      _originalOppositeDomainObjects.DeserializeFromFlatStructure (info);
      _originalOppositeDomainObjects.ChangeDelegate = this;

      _oppositeDomainObjects = (DomainObjectCollection)
          TypesafeActivator.CreateInstance (collectionType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With ();
      _oppositeDomainObjects.DeserializeFromFlatStructure (info);
      _oppositeDomainObjects.ChangeDelegate = this;

      _hasBeenTouched = info.GetBoolValue ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_originalOppositeDomainObjects.GetType());
      _originalOppositeDomainObjects.SerializeIntoFlatStructure (info);
      _oppositeDomainObjects.SerializeIntoFlatStructure (info);
      info.AddBoolValue (_hasBeenTouched);
    }

    #endregion
  }
}
