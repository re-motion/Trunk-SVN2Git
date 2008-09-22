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
using System.ComponentModel;
using System.Reflection;
using Remotion.Collections;
using Remotion.Text;

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

      object[] attributeArray = GetCustomAttributes (element, attributeType, inherit);
      if ((attributeArray == null) || (attributeArray.Length == 0))
        return null;
      if (attributeArray.Length != 1)
        throw new AmbiguousMatchException ("Multiple custom attributes of the same type found.");
      return (Attribute)attributeArray[0];
    }

    public static T[] GetCustomAttributes<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      return (T[])GetCustomAttributes (element, typeof (T), inherit);
    }

    public static object[] GetCustomAttributes (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      Type elementAsType = element as Type;
      if (elementAsType != null)
        return GetCustomAttributes (elementAsType, attributeType, inherit);

      Attribute[] attributes = Attribute.GetCustomAttributes (element, typeof (Attribute), inherit);
      Attribute[] filteredAttributes = Array.FindAll (attributes, delegate (Attribute attribute) { return attributeType.IsInstanceOfType (attribute); });
      return (object[]) ArrayUtility.Convert (filteredAttributes, attributeType);
    }

    public static object[] GetCustomAttributes (Type type, Type attributeType, bool inherit)
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

      return (object[]) ArrayUtility.Convert (EnumerableUtility.ToArray (attributeInstances), attributeType);
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

    private static readonly InterlockedCache<Type, AttributeUsageAttribute> s_cache = new InterlockedCache<Type, AttributeUsageAttribute>();

    public static AttributeUsageAttribute GetAttributeUsage (Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      AttributeUsageAttribute cachedInstance = s_cache.GetOrCreateValue (
          attributeType,
          delegate (Type type)
          {
            AttributeUsageAttribute[] usage =
                (AttributeUsageAttribute[]) type.GetCustomAttributes (typeof (AttributeUsageAttribute), true);
            if (usage.Length == 0)
              return new AttributeUsageAttribute (AttributeTargets.All);
            else
            {
              Assertion.IsTrue (usage.Length == 1, "AllowMultiple == false");
              return usage[0];
            }
          });
      
      AttributeUsageAttribute newInstance = new AttributeUsageAttribute (cachedInstance.ValidOn);
      newInstance.AllowMultiple = cachedInstance.AllowMultiple;
      newInstance.Inherited = cachedInstance.Inherited;
      return newInstance;
    }
  }
}
