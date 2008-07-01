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
    private readonly StorageClass _storageClass;

    // construction and disposing

    protected PropertyDefinition (ClassDefinition classDefinition, string propertyName, string columnName, int? maxLength, StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      _classDefinition = classDefinition;
      _serializedClassDefinitionID = classDefinition.ID;
      _propertyName = propertyName;
      _storageSpecificName = columnName;
      _maxLength = maxLength;
      _storageClass = storageClass;
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
        if (StorageClass != StorageClass.Persistent)
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

    public StorageClass StorageClass
    {
      get { return _storageClass; }
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
