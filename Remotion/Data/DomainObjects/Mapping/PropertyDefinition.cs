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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}")]
  public class PropertyDefinition : SerializableMappingObject
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
    private readonly int? _maxLength;
    [NonSerialized]
    private readonly StorageClass _storageClass;
    
    [NonSerialized]
    private IStoragePropertyDefinition _storagePropertyDefinition;

    [NonSerialized]
    private readonly PropertyInfo _propertyInfo;

    [NonSerialized]
    private readonly Type _propertyType;

    [NonSerialized]
    private readonly bool _isNullable;

    // construction and disposing

    public PropertyDefinition (
        ClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        string propertyName,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      _classDefinition = classDefinition;
      _serializedClassDefinitionID = classDefinition.ID;
      _propertyName = propertyName;
      _maxLength = maxLength;
      _storageClass = storageClass;

      if (propertyType.IsValueType && Nullable.GetUnderlyingType (propertyType) == null && isNullable)
        throw CreateArgumentException (propertyName, "Properties cannot be nullable when they have a non-nullable value type.");

      //TODO: change byte[] to all arrays. Will have repurcussions in several places -> Search for byte[]
      if (propertyType != typeof (string) && propertyType != typeof (byte[]) && maxLength.HasValue)
      {
        throw CreateArgumentException (
            propertyName, "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type '{0}'.", propertyType);
      }

      _propertyInfo = propertyInfo;
      _propertyType = propertyType;
      _isNullable = isNullable;
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

    public IStoragePropertyDefinition StoragePropertyDefinition
    {
      get
      {
        if (StorageClass != StorageClass.Persistent)
          throw new InvalidOperationException ("Cannot access property 'storagePropertyDefinition' for non-persistent property definitions.");

        return _storagePropertyDefinition;
      }
    }

    public int? MaxLength
    {
      get { return _maxLength; }
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }

    public void SetStorageProperty (IStoragePropertyDefinition storagePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("storagePropertyDefinition", storagePropertyDefinition);

      _storagePropertyDefinition = storagePropertyDefinition;
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

    public virtual PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public virtual bool IsPropertyInfoResolved
    {
      get { return true; }
    }

    public virtual Type PropertyType
    {
      get { return _propertyType; }
    }

    public virtual bool IsPropertyTypeResolved
    {
      get { return true; }
    }

    public virtual object DefaultValue
    {
      get
      {
        if (_isNullable)
          return null;

        if (_propertyType.IsArray)
          return Array.CreateInstance (_propertyType.GetElementType(), 0);

        if (_propertyType == typeof (string))
          return string.Empty;

        if (_propertyType.IsEnum)
          return EnumUtility.GetEnumMetadata (_propertyType).OrderedValues[0];

        if (ExtensibleEnumUtility.IsExtensibleEnumType (_propertyType))
          return ExtensibleEnumUtility.GetDefinition (_propertyType).GetValueInfos().First().Value;

        return Activator.CreateInstance (_propertyType, new object[0]);
      }
    }

    public virtual bool IsNullable
    {
      get { return _isNullable; }
    }

    public virtual bool IsObjectID
    {
      get { return _propertyType == typeof (ObjectID); }
    }

    #endregion

    private ArgumentException CreateArgumentException (string propertyName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args) + "\r\n  Property: " + propertyName);
    }
  }
}
