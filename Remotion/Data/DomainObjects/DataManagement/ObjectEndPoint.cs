// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
  public class ObjectEndPoint : RelationEndPoint
  {
    // types

    // static members and constants

    // member fields

    private ObjectID _originalOppositeObjectID;
    private ObjectID _oppositeObjectID;
    private bool _hasBeenTouched;

    // construction and disposing

    public ObjectEndPoint (
        ClientTransaction clientTransaction,
        ObjectID objectID,
        IRelationEndPointDefinition definition,
        ObjectID oppositeObjectID)
        : this (clientTransaction, objectID, definition.PropertyName, oppositeObjectID)
    {
    }

    public ObjectEndPoint (
        ClientTransaction clientTransaction,
        ObjectID objectID,
        string propertyName,
        ObjectID oppositeObjectID)
        : this (clientTransaction, new RelationEndPointID (objectID, propertyName), oppositeObjectID)
    {
    }

    public ObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ObjectID oppositeObjectID)
        : this (clientTransaction, id, oppositeObjectID, oppositeObjectID)
    {
    }

    private ObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ObjectID oppositeObjectID,
        ObjectID originalOppositeObjectID)
        : base (clientTransaction, id)
    {
      _oppositeObjectID = oppositeObjectID;
      _originalOppositeObjectID = originalOppositeObjectID;
      _hasBeenTouched = false;
    }

    protected ObjectEndPoint (IRelationEndPointDefinition definition)
        : base (definition)
    {
      _hasBeenTouched = false;
    }

    // methods and properties

    public override RelationEndPoint Clone (ClientTransaction clientTransaction)
    {
      var clone = new ObjectEndPoint (clientTransaction, ID, null);
      clone.AssumeSameState (this);
      return clone;
    }

    protected internal override void AssumeSameState (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      ObjectEndPoint sourceObjectEndPoint = (ObjectEndPoint) source;

      _oppositeObjectID = sourceObjectEndPoint._oppositeObjectID;
      _originalOppositeObjectID = sourceObjectEndPoint._originalOppositeObjectID;
      _hasBeenTouched = sourceObjectEndPoint._hasBeenTouched;
    }

    protected internal override void TakeOverCommittedData (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      ObjectEndPoint sourceObjectEndPoint = (ObjectEndPoint) source;

      _oppositeObjectID = sourceObjectEndPoint._oppositeObjectID;
      _hasBeenTouched |= sourceObjectEndPoint._hasBeenTouched || HasChanged; // true if: we have been touched/source has been touched/we have changed
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _originalOppositeObjectID = _oppositeObjectID;
        _hasBeenTouched = false;
      }
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _oppositeObjectID = _originalOppositeObjectID;
        _hasBeenTouched = false;
      }
    }

    public override bool HasChanged
    {
      get { return !object.Equals (_oppositeObjectID, _originalOppositeObjectID); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
      SetForeignKeyProperty (); // set foreign key property to the same value in order to touch it
    }

    public override void CheckMandatory ()
    {
      if (_oppositeObjectID == null)
      {
        throw CreateMandatoryRelationNotSetException (
            GetDomainObject(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' cannot be null.",
            PropertyName,
            ObjectID);
      }
    }

    public override RelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var currentRelatedObject = GetOppositeObject (true);
      if (removedRelatedObject != currentRelatedObject)
      {
        string removedID = removedRelatedObject.ID.ToString ();
        string currentID = currentRelatedObject != null ? currentRelatedObject.ID.ToString () : "<null>";

        var message = string.Format ("Cannot remove object '{0}' from object end point '{1}' - it currently holds object '{2}'.", 
            removedID, PropertyName, currentID);
        throw new InvalidOperationException (message);
      }

      return CreateSetModification (null);
    }

    public virtual RelationEndPointModification CreateSetModification (DomainObject newRelatedObject)
    {
      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (_oppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameModification (this);
      else if (OppositeEndPointDefinition.IsAnonymous)
        return new ObjectEndPointSetUnidirectionalModification (this, newRelatedObject);
      else if (OppositeEndPointDefinition.Cardinality == CardinalityType.One)
        return new ObjectEndPointSetOneOneModification (this, newRelatedObject);
      else 
        return new ObjectEndPointSetOneManyModification (this, newRelatedObject);
    }

    public override void PerformDelete ()
    {
      OppositeObjectID = null;
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { return _originalOppositeObjectID; }
    }

    public ObjectID OppositeObjectID
    {
      get { return _oppositeObjectID; }
      set
      {
        _oppositeObjectID = value;
        _hasBeenTouched = true;

        SetForeignKeyProperty();
      }
    }

    public DomainObject GetOppositeObject (bool includeDeleted)
    {
      if (OppositeObjectID == null)
        return null;
      else if (includeDeleted && ClientTransaction.DataManager.IsDiscarded (OppositeObjectID))
        return ClientTransaction.DataManager.GetDiscardedDataContainer (OppositeObjectID).DomainObject;
      else
        return ClientTransaction.GetObject (OppositeObjectID, includeDeleted);
    }

    protected virtual void SetForeignKeyProperty ()
    {
      if (!IsVirtual)
      {
        var dataContainer = ClientTransaction.GetDataContainer (GetDomainObject ());
        var foreignKeyProperty = dataContainer.PropertyValues[PropertyName];
        foreignKeyProperty.SetRelationValue (_oppositeObjectID);
      }
    }

    #region Serialization

    protected ObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _hasBeenTouched = info.GetBoolValue ();
      _oppositeObjectID = info.GetValueForHandle<ObjectID>();
      if (_hasBeenTouched)
        _originalOppositeObjectID = info.GetValueForHandle<ObjectID> ();
      else
        _originalOppositeObjectID = _oppositeObjectID;
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddBoolValue (_hasBeenTouched);
      info.AddHandle (_oppositeObjectID);
      if (_hasBeenTouched)
        info.AddHandle (_originalOppositeObjectID);
    }

    #endregion
  }
}
