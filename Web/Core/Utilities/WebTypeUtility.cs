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
