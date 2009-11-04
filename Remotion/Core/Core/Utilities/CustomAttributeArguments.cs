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
