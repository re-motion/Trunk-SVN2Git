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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Remotion.Collections;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  public static class AttributeUtility
  {
    public static bool IsDefined<T> (MemberInfo element, bool inherit)
       where T : class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");

      return IsDefined (element, typeof (T), inherit);
    }

    public static bool IsDefined (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      return GetCustomAttributes (element, attributeType, inherit).Length > 0;
    }

    public static T GetCustomAttribute<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      return (T) (object) GetCustomAttribute (element, typeof (T), inherit);
    }

    public static Attribute GetCustomAttribute (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      Attribute[] attributeArray = GetCustomAttributes (element, attributeType, inherit);
      if ((attributeArray == null) || (attributeArray.Length == 0))
        return null;
      if (attributeArray.Length != 1)
        throw new AmbiguousMatchException ("Multiple custom attributes of the same type found.");
      return attributeArray[0];
    }

    public static T[] GetCustomAttributes<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      Attribute[] attributesWithMatchingType = GetCustomAttributes (element, typeof (T), inherit);
      return Array.ConvertAll<Attribute, T> (attributesWithMatchingType, delegate (Attribute attribute) { return (T) (object) attribute; });
    }

    public static Attribute[] GetCustomAttributes (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      Type elementAsType = element as Type;
      if (elementAsType != null)
        return GetCustomAttributes (elementAsType, attributeType, inherit);

      Attribute[] attributes = Attribute.GetCustomAttributes (element, typeof (Attribute), inherit);
      return Array.FindAll (attributes, delegate (Attribute attribute) { return attributeType.IsInstanceOfType (attribute); });
    }

    public static Attribute[] GetCustomAttributes (Type type, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      CheckAttributeType (attributeType, "attributeType");

      AttributeWithMetadata[] attributes = GetCustomAttributesWithMetadata (type, inherit);
      AttributeWithMetadata[] suppressAttributes = 
          EnumerableUtility.ToArray (AttributeWithMetadata.IncludeAll (attributes, typeof (SuppressAttributesAttribute)));

      IEnumerable<AttributeWithMetadata> attributesWithRightType = AttributeWithMetadata.IncludeAll (attributes, attributeType);
      IEnumerable<AttributeWithMetadata> filteredAttributes = 
          AttributeWithMetadata.ExcludeAll (attributesWithRightType, typeof (SuppressAttributesAttribute));

      IEnumerable<AttributeWithMetadata> suppressedAttributes = AttributeWithMetadata.Suppress (filteredAttributes, suppressAttributes);
      IEnumerable<Attribute> attributeInstances = AttributeWithMetadata.ExtractInstances (suppressedAttributes);

      return EnumerableUtility.ToArray (attributeInstances);
    }

    public static AttributeWithMetadata[] GetCustomAttributesWithMetadata (Type type, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      List<AttributeWithMetadata> result = new List<AttributeWithMetadata> ();

      Type currentType = type;
      do
      {
        Attribute[] attributes = Attribute.GetCustomAttributes (currentType, false); // get attributes exactly for current type
        foreach (Attribute attribute in attributes)
        {
          if (type == currentType || IsAttributeInherited (attribute.GetType()))
            result.Add (new AttributeWithMetadata (currentType, attribute));
        }
        currentType = currentType.BaseType;
      } while (inherit && currentType != null && currentType != typeof (object)); // iterate unless inherit == false, stop when typeof (object) is reached

      return result.ToArray ();
    }

    private static void CheckAttributeType (Type attributeType, string parameterName)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      if (!typeof (Attribute).IsAssignableFrom (attributeType) && !attributeType.IsInterface)
      {
        string message = "The attribute type must be assignable to System.Attribute or an interface.";
        throw new ArgumentTypeException (message, parameterName, typeof (Attribute), attributeType);
      }
    }

    public static bool IsAttributeInherited (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage.Inherited;
    }

    public static bool IsAttributeAllowMultiple (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage.AllowMultiple;
    }

    public static AttributeUsageAttribute GetAttributeUsage (Type attributeType)
    {
      AttributeUsageAttribute[] usage =
          (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true);
      if (usage.Length == 0)
        return new AttributeUsageAttribute (AttributeTargets.All);
      else
      {
        Assertion.IsTrue (usage.Length == 1, "AllowMultiple == false");
        return usage[0];
      }
    }

    public static Attribute InstantiateCustomAttributeData (CustomAttributeData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      object[] constructorArguments = new object[data.ConstructorArguments.Count];
      for (int i = 0; i < data.ConstructorArguments.Count; ++i)
        constructorArguments[i] = ExtractValueFromAttributeArgument (data.ConstructorArguments[i]);
      Attribute attribute = (Attribute) data.Constructor.Invoke (constructorArguments);

      foreach (CustomAttributeNamedArgument namedArgument in data.NamedArguments)
      {
        object value = ExtractValueFromAttributeArgument (namedArgument.TypedValue);
        FieldInfo fieldInfo = namedArgument.MemberInfo as FieldInfo;
        if (fieldInfo != null)
          fieldInfo.SetValue (attribute, value);
        else
          ((PropertyInfo) namedArgument.MemberInfo).SetValue (attribute, value, null);
      }

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
  }
}
