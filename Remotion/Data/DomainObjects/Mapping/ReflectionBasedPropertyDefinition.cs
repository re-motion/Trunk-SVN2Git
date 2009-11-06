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
using System.Linq;
using System.Reflection;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedPropertyDefinition: PropertyDefinition
  {
    // no members are serialized here
    [NonSerialized]
    private readonly PropertyInfo _propertyInfo;
    [NonSerialized]
    private readonly Type _propertyType;
    [NonSerialized]
    private readonly bool _isNullable;

    public ReflectionBasedPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        string propertyName,
        string columnName,
        Type propertyType,
        bool? isNullable,
        int? maxLength,
        StorageClass storageClass)
        : base (classDefinition, propertyName, columnName, maxLength, storageClass)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      if (propertyType.IsValueType && isNullable.HasValue)
      {
        throw CreateArgumentException (
            propertyName, "IsNullable parameter can only be supplied for reference types but the property is of type '{0}'.", propertyType);
      }
      //TODO: change byte[] to all arrays. Will have repurcussions in several places -> Search for byte[]
      if (propertyType != typeof (string) && propertyType != typeof (byte[]) && maxLength.HasValue)
      {
        throw CreateArgumentException (
            propertyName, "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type '{0}'.", propertyType);
      }

      _propertyInfo = propertyInfo;
      _propertyType = propertyType;
      if (propertyType.IsValueType)
        _isNullable = Nullable.GetUnderlyingType (propertyType) != null;
      else
        _isNullable = isNullable ?? true;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public override Type PropertyType
    {
      get { return _propertyType; }
    }

    public override bool IsPropertyTypeResolved
    {
      get { return true; }
    }

    //TODO: Implement DefaultValue Provider
    public override object DefaultValue
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
          return ExtensibleEnumUtility.GetDefinition (_propertyType).GetValueInfos ().First ().Value;

        return Activator.CreateInstance (_propertyType, new object[0]);
      }
    }

    public override bool IsNullable
    {
      get { return _isNullable; }
    }

    public override bool IsObjectID
    {
      get { return _propertyType == typeof (ObjectID); }
    }

    private ArgumentException CreateArgumentException (string propertyName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args) + "\r\n  Property: " + propertyName);
    }
  }
}
