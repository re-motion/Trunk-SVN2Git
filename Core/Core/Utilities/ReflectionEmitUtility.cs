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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Text;

namespace Remotion.Utilities
{
  public static class ReflectionEmitUtility
  {
    public struct CustomAttributeBuilderData
    {
      private readonly object[] _constructorArgs;
      private readonly PropertyInfo[] _namedProperties;
      private readonly object[] _propertyValues;
      private readonly FieldInfo[] _namedFields;
      private readonly object[] _fieldValues;

      public CustomAttributeBuilderData (object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields,
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

    public static CustomAttributeBuilder CreateAttributeBuilderFromData (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilderData builderData = GetCustomAttributeBuilderData (attributeData);
      return new CustomAttributeBuilder (attributeData.Constructor, builderData.ConstructorArgs, builderData.NamedProperties,
          builderData.PropertyValues, builderData.NamedFields, builderData.FieldValues);
    }

    public static CustomAttributeBuilderData GetCustomAttributeBuilderData (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CheckForPublicFields (attributeData.Constructor.DeclaringType);

      object[] constructorArgs = new object[attributeData.ConstructorArguments.Count];
      for (int i = 0; i < constructorArgs.Length; ++i)
        constructorArgs[i] = GetRealCustomAttributeArgumentValue (attributeData.ConstructorArguments[i]);

      List<PropertyInfo> namedProperties = new List<PropertyInfo> ();
      List<object> propertyValues = new List<object> ();
      List<FieldInfo> namedFields = new List<FieldInfo> ();
      List<object> fieldValues = new List<object> ();

      foreach (CustomAttributeNamedArgument namedArgument in attributeData.NamedArguments)
      {
        switch (namedArgument.MemberInfo.MemberType)
        {
          case MemberTypes.Field:
            namedFields.Add ((FieldInfo) namedArgument.MemberInfo);
            fieldValues.Add (GetRealCustomAttributeArgumentValue (namedArgument.TypedValue));
            break;
          case MemberTypes.Property:
            namedProperties.Add ((PropertyInfo) namedArgument.MemberInfo);
            propertyValues.Add (GetRealCustomAttributeArgumentValue (namedArgument.TypedValue));
            break;
          default:
            Assertion.IsTrue (false);
            break;
        }
      }

      return new CustomAttributeBuilderData (constructorArgs, namedProperties.ToArray (), propertyValues.ToArray (), namedFields.ToArray (),
          fieldValues.ToArray ());
    }

    // This is due to a bug in CustomAttributeData.GetCustomAttributes, where CustomAttributeData misinterprets named arguments if public fields
    // are present on a type, see ReflectionEmitUtilityTests.CreateAttributeBuilderFromData_WithCtorArgumentsNamedArgumentsAndNamedFields_Throws.
    // This check can be removed when the bug has been fixed with a .NET 2.0 framework service pack.
    private static void CheckForPublicFields (Type type)
    {
      FieldInfo[] fields = type.GetFields (BindingFlags.Instance | BindingFlags.Public);
      if (fields.Length > 0)
      {
        string message = string.Format ("Type {0} declares public fields: {1}. Due to a bug in CustomAttributeData.GetCustomAttributes, attributes "
            + "with public fields are currently not supported.", type.FullName,
            SeparatedStringBuilder.Build (", ", fields, delegate (FieldInfo field) { return field.Name; }));
        throw new NotSupportedException (message);
      }
    }

    private static object GetRealCustomAttributeArgumentValue (CustomAttributeTypedArgument typedArgument)
    {
      if (typedArgument.ArgumentType.IsArray)
      {
        IList<CustomAttributeTypedArgument> elements = (IList<CustomAttributeTypedArgument>) typedArgument.Value;
        IList realValueArray = Array.CreateInstance (typedArgument.ArgumentType.GetElementType(), elements.Count);
        for (int i = 0; i < elements.Count; ++i)
          realValueArray[i] = GetRealCustomAttributeArgumentValue (elements[i]);
        return realValueArray;
      }
      else
        return typedArgument.Value;
    }
  }
}
