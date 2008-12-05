// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Utilities
{
  public static class CustomAttributeDataUtility
  {
    public static Attribute InstantiateCustomAttributeData (CustomAttributeData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      CustomAttributeArguments arguments = ParseCustomAttributeArguments (data);

      object[] constructorArguments = arguments.ConstructorArgs;
      Attribute attribute = (Attribute) data.Constructor.Invoke (constructorArguments);

      for (int i = 0; i < arguments.NamedFields.Length; i++)
        arguments.NamedFields[i].SetValue (attribute, arguments.FieldValues[i]);

      for (int i = 0; i < arguments.NamedProperties.Length; i++)
        arguments.NamedProperties[i].SetValue (attribute, arguments.PropertyValues[i], null);

      return attribute;
    }

    private static object ExtractValueFromAttributeArgument (CustomAttributeTypedArgument typedArgument)
    {
      if (typedArgument.ArgumentType.IsArray)
      {
        IList<CustomAttributeTypedArgument> typedArgumentValue = (IList<CustomAttributeTypedArgument>) typedArgument.Value;
        Array array = Array.CreateInstance (typedArgument.ArgumentType.GetElementType (), typedArgumentValue.Count);
        for (int i = 0; i < typedArgumentValue.Count; i++)
        {
          CustomAttributeTypedArgument arrayElement = typedArgumentValue[i];
          array.SetValue (ExtractValueFromAttributeArgument (arrayElement), i);
        }
        return array;
      }
      else if (typedArgument.ArgumentType.IsEnum)
        return Enum.ToObject (typedArgument.ArgumentType, typedArgument.Value);
      else
        return typedArgument.Value;
    }

    public static CustomAttributeArguments ParseCustomAttributeArguments (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      object[] constructorArgs = new object[attributeData.ConstructorArguments.Count];
      for (int i = 0; i < constructorArgs.Length; ++i)
        constructorArgs[i] = ExtractValueFromAttributeArgument (attributeData.ConstructorArguments[i]);

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
            fieldValues.Add (ExtractValueFromAttributeArgument (namedArgument.TypedValue));
            break;
          case MemberTypes.Property:
            namedProperties.Add ((PropertyInfo) namedArgument.MemberInfo);
            propertyValues.Add (ExtractValueFromAttributeArgument (namedArgument.TypedValue));
            break;
          default:
            Assertion.IsTrue (false);
            break;
        }
      }

      return new CustomAttributeArguments (constructorArgs, namedProperties.ToArray (), propertyValues.ToArray (), namedFields.ToArray (),
                                             fieldValues.ToArray ());
    }
  }
}
