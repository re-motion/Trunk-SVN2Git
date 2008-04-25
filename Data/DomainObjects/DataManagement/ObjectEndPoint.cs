using System;
using Remotion.Data.DomainObjects.Infrastructure;
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

    public override RelationEndPoint Clone ()
    {
      ObjectEndPoint clone = new ObjectEndPoint (ClientTransaction, ID, null);
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

    protected internal override void RegisterWithMap (RelationEndPointMap map)
    {
      // nothing to do here
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

    protected internal override void Touch ()
    {
      _hasBeenTouched = true;
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

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new ObjectEndPointModification (this, oldEndPoint, newEndPoint);
    }

    public virtual void PerformRelationChange (ObjectEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);
      PerformRelationChange (modification.NewEndPoint);
    }

    private void PerformRelationChange (IEndPoint newEndPoint)
    {
      OppositeObjectID = newEndPoint.ObjectID;

      if (!IsVirtual)
      {
        DataContainer dataContainer = GetDataContainer();
        dataContainer.PropertyValues[PropertyName].SetRelationValue (newEndPoint.ObjectID);
      }
    }

    public override void PerformDelete ()
    {
      PerformRelationChange (RelationEndPoint.CreateNullRelationEndPoint (OppositeEndPointDefinition));
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