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
    return TypeUtility.GetPartialAssemblyQualifiedName (type);
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
