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
using System.Reflection;
using System.Web.Compilation;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{

/// <summary> Utility methods for handling types in web projects. </summary>
public sealed class WebTypeUtility
{
  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
    return BuildManager.GetType (typeName, true);
  }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
    return BuildManager.GetType (typeName, throwOnError);
    }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
    return BuildManager.GetType (typeName, throwOnError, ignoreCase);
  }

  public static string GetQualifiedName (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    if (IsCompiledType (type))
      return type.FullName;
    return type.FullName + "," + type.Assembly.GetName().Name;
  }

  public static bool IsCompiledType (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    IList codeAssemblies = BuildManager.CodeAssemblies;
    if (codeAssemblies == null)
      return false;

    foreach (Assembly assembly in codeAssemblies)
    {
      if (assembly == type.Assembly)
        return true;
    }
    return false;
  }

	private WebTypeUtility()
	{
  }
}

}
