// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

    public static CustomAttributeArguments ParseCustomAttributeArguments (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      object[] constructorArgs = new object[attributeData.ConstructorArguments.Count];
      for (int i = 0; i < constructorArgs.Length; ++i)
        constructorArgs[i] = CustomAttributeTypedArgumentUtility.Unwrap (attributeData.ConstructorArguments[i]);

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
            fieldValues.Add (CustomAttributeTypedArgumentUtility.Unwrap (namedArgument.TypedValue));
            break;
          case MemberTypes.Property:
            namedProperties.Add ((PropertyInfo) namedArgument.MemberInfo);
            propertyValues.Add (CustomAttributeTypedArgumentUtility.Unwrap (namedArgument.TypedValue));
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
