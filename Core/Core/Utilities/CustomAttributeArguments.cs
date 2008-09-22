/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Reflection;

namespace Remotion.Utilities
{
  public struct CustomAttributeArguments
  {
    private readonly object[] _constructorArgs;
    private readonly PropertyInfo[] _namedProperties;
    private readonly object[] _propertyValues;
    private readonly FieldInfo[] _namedFields;
    private readonly object[] _fieldValues;

    public CustomAttributeArguments (object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields,
                                       object[] fieldValues)
    {
      _constructorArgs = constructorArgs;
      _fieldValues = fieldValues;
      _namedFields = namedFields;
      _propertyValues = propertyValues;
      _namedProperties = namedProperties;
    }

    public object[] FieldValues
    {
      get { return _fieldValues; }
    }

    public FieldInfo[] NamedFields
    {
      get { return _namedFields; }
    }

    public object[] PropertyValues
    {
      get { return _propertyValues; }
    }

    public PropertyInfo[] NamedProperties
    {
      get { return _namedProperties; }
    }

    public object[] ConstructorArgs
    {
      get { return _constructorArgs; }
    }
  }
}