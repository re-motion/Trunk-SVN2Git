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
        bool isPersistent)
        : base (classDefinition, propertyName, columnName, maxLength, isPersistent)
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
