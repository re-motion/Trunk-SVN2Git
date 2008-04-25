using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}")]
  public abstract class PropertyDefinition : SerializableMappingObject
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: PropertyDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _propertyName;
    private readonly string _serializedClassDefinitionID;

    // nonserialized member fields

    [NonSerialized]
    private readonly ClassDefinition _classDefinition;
    [NonSerialized]
    private readonly string _storageSpecificName;
    [NonSerialized]
    private readonly int? _maxLength;
    [NonSerialized]
    private readonly bool _isPersistent;

    // construction and disposing

    protected PropertyDefinition (ClassDefinition classDefinition, string propertyName, string columnName, int? maxLength, bool isPersistent)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      _classDefinition = classDefinition;
      _serializedClassDefinitionID = classDefinition.ID;
      _propertyName = propertyName;
      _storageSpecificName = columnName;
      _maxLength = maxLength;
      _isPersistent = isPersistent;
    }

    // methods and properties

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public string StorageSpecificName
    {
      get
      {
        if (!_isPersistent)
          throw new InvalidOperationException ("Cannot access property 'StorageSpecificName' for non-persistent property definitions.");
        return _storageSpecificName;
      }
    }

    public abstract Type PropertyType { get; }

    public abstract bool IsPropertyTypeResolved
    {
      get; }

    public abstract bool IsNullable
    {
      get; }

    public abstract object DefaultValue { get; }

    public abstract bool IsObjectID { get; }

    public int? MaxLength
    {
      get { return _maxLength; }
    }

    public bool IsPersistent
    {
      get { return _isPersistent; }
    }

    public override string ToString ()
    {
      return GetType ().FullName + ": " + _propertyName;
    }

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      // Note: A PropertyDefinition must know its ClassDefinition to correctly deserialize itself and a 
      // ClassDefinition knows its PropertyDefintions. For bi-directional relationships
      // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
      // Therefore the member _classDefinition cannot be used here, because it could point to the wrong instance. 
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_serializedClassDefinitionID)[_propertyName];
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return PropertyName; }
    }
    #endregion
  }
}