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
using Remotion.Utilities;

namespace Remotion.Globalization
{

[AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class ResourceIdentifiersAttribute: Attribute
{
  public static string GetResourceIdentifier (Enum enumValue)
  {
    ArgumentUtility.CheckNotNull ("enumValue", enumValue);
    Type type = enumValue.GetType();
    if (type.DeclaringType != null && IsEnumTypeNameSuppressed (type)) // if the enum is a nested type, suppress enum name
      type = type.DeclaringType;
    return type.FullName + "." + enumValue.ToString();

//    string typePath = type.FullName.Substring (0, type.FullName.Length - type.Name.Length);
//    if (typePath.EndsWith ("+"))
//      return typePath.Substring (0, typePath.Length - 1) + "." + enumValue.ToString(); // nested enum type: exclude enum type name
//    else
//      return type.FullName + "." + enumValue.ToString();
  }

  public static ResourceIdentifiersAttribute GetAttribute (Type type)
  {
    object[] attributes = type.GetCustomAttributes (typeof (ResourceIdentifiersAttribute), false);
    if (attributes == null || attributes.Length == 0)
      return null;
    else
      return (ResourceIdentifiersAttribute) attributes[0];
  }

  private static bool IsEnumTypeNameSuppressed (Type type)
  {
    ResourceIdentifiersAttribute attrib = GetAttribute (type);
    if (attrib == null)
      return false;
    else
      return attrib.SuppressTypeName;
  }

  bool _suppressTypeName;

  /// <summary> Initializes a new instance. </summary>
  /// <param name="suppressTypeName"> If true, the name of the enum type is not included in the resource identifier. Default is true. </param>
  public ResourceIdentifiersAttribute (bool suppressTypeName)
  {
    _suppressTypeName = suppressTypeName;
  }

  /// <summary> Initializes a new instance. </summary>
  public ResourceIdentifiersAttribute ()
    : this (true)
  {
  }

  public bool SuppressTypeName
  {
    get { return _suppressTypeName; }
  }
}

}
