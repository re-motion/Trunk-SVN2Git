using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public sealed class RelationEndPointID : IFlattenedSerializable
  {
    // types

    // static members and constants

    public static RelationEndPointID[] GetAllRelationEndPointIDs (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      IRelationEndPointDefinition[] endPointDefinitions = dataContainer.ClassDefinition.GetRelationEndPointDefinitions();

      RelationEndPointID[] relationEndPointIDs = new RelationEndPointID[endPointDefinitions.Length];

      for (int i = 0; i < endPointDefinitions.Length; i++)
        relationEndPointIDs[i] = new RelationEndPointID (dataContainer.ID, endPointDefinitions[i].PropertyName);

      return relationEndPointIDs;
    }

    public static bool operator == (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      return Equals (endPointID1, endPointID2);
    }

    public static bool operator != (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      return !Equals (endPointID1, endPointID2);
    }

    public static bool Equals (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      if (object.ReferenceEquals (endPointID1, endPointID2))
        return true;
      if (object.ReferenceEquals (endPointID1, null))
        return false;

      return endPointID1.Equals (endPointID2);
    }

    // member fields

    private readonly IRelationEndPointDefinition _definition;
    private readonly ObjectID _objectID;

    // construction and disposing

    public RelationEndPointID (ObjectID objectID, IRelationEndPointDefinition definition)
        : this (objectID, definition.PropertyName)
    {
    }

    public RelationEndPointID (ObjectID objectID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _definition = objectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
      _objectID = objectID;
    }

    // methods and properties

    public override int GetHashCode ()
    {
      return _objectID.GetHashCode() ^ PropertyName.GetHashCode();
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (this.GetType() != obj.GetType())
        return false;

      RelationEndPointID other = (RelationEndPointID) obj;
      if (!object.Equals (this._objectID, other._objectID))
        return false;
      if (!object.Equals (this._definition, other._definition))
        return false;

      return true;
    }

    public override string ToString ()
    {
      return string.Format ("{0}/{1}", _objectID.ToString(), PropertyName);
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public string PropertyName
    {
      get { return _definition.PropertyName; }
    }

    public IRelationEndPointDefinition OppositeEndPointDefinition
    {
      get { return _definition.ClassDefinition.GetMandatoryOppositeEndPointDefinition (PropertyName); }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _definition.ClassDefinition.GetMandatoryRelationDefinition (PropertyName); }
    }

    public bool IsVirtual
    {
      get { return _definition.IsVirtual; }
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    #region Serialization

    private RelationEndPointID (FlattenedDeserializationInfo info)
    {
      string classDefinitionID = info.GetValueForHandle<string> ();
      string propertyName = info.GetValueForHandle<string>();
      _definition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinitionID).GetMandatoryRelationEndPointDefinition (propertyName);
      _objectID = info.GetValueForHandle<ObjectID>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_definition.ClassDefinition.ID);
      info.AddHandle (_definition.PropertyName);
      info.AddHandle (_objectID);
    }

    #endregion
  }
}