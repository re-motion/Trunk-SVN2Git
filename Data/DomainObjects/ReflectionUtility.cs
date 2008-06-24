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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Utility class for methods using reflection.
  /// </summary>
  public static class ReflectionUtility
  {
    // types

    // static members and constants

    /// <summary>
    /// Returns the directory of the current executing assembly.
    /// </summary>
    /// <returns>The path of the current executing assembly</returns>
    public static string GetConfigFileDirectory()
    {
      return GetAssemblyDirectory (typeof (DomainObject).Assembly);
    }

    /// <summary>
    /// Gets the directory containing the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly whose directory to retrieve.</param>
    /// <returns>The directory holding the given assembly as a local path. If the assembly has been shadow-copied, this returns the directory before the
    /// shadow-copying.</returns>
    /// <exception cref="InvalidOperationException">The assembly's code base is not a local path.</exception>
    public static string GetAssemblyDirectory (Assembly assembly)
    {
      return GetAssemblyDirectory ((_Assembly) assembly);
    }

    /// <summary>
    /// Gets the directory containing the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly whose directory to retrieve.</param>
    /// <returns>The directory holding the given assembly as a local path. If the assembly has been shadow-copied, this returns the directory before the
    /// shadow-copying.</returns>
    /// <exception cref="InvalidOperationException">The assembly's code base is not a local path.</exception>
    [CLSCompliant (false)]
    public static string GetAssemblyDirectory (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      Uri codeBaseUri = new Uri (assembly.EscapedCodeBase);
      if (!codeBaseUri.IsFile)
        throw new InvalidOperationException (string.Format ("The assembly's code base '{0}' is not a local path.", codeBaseUri.OriginalString));
      return Path.GetDirectoryName (codeBaseUri.LocalPath);
    }

    /// <summary>
    /// Creates an object of a given type.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> of the object to instantiate. Must not be <see langword="null"/>.</param>
    /// <param name="constructorParameters">The parameters for the constructor of the object.</param>
    /// <returns>The object that has been created.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException">Type <paramref name="type"/> has no suitable constructor for the given <paramref name="constructorParameters"/>.</exception>
    public static object CreateObject (Type type, params object[] constructorParameters)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Type[] constructorParameterTypes = new Type[constructorParameters.Length];
      for (int i = 0; i < constructorParameterTypes.Length; i++)
        constructorParameterTypes[i] = constructorParameters[i].GetType();

      ConstructorInfo constructor = type.GetConstructor (
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          null,
          constructorParameterTypes,
          null);

      if (constructor != null)
        return constructor.Invoke (constructorParameters);
      else
      {
        throw new ArgumentException (
            string.Format (
                "Type '{0}' has no suitable constructor. Parameter types: ({1})",
                type,
                GetTypeListAsString (constructorParameterTypes)));
      }
    }

    internal static string GetTypeListAsString (Type[] types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      string result = string.Empty;
      foreach (Type type in types)
      {
        if (result != string.Empty)
          result += ", ";

        if (type != null)
          result += type.ToString();
        else
          result += "<any reference type>";
      }

      return result;
    }

    public static string GetSignatureForArguments (object[] args)
    {
      Type[] argumentTypes = GetTypesForArgs (args);
      return GetTypeListAsString (argumentTypes);
    }

    public static Type[] GetTypesForArgs (object[] args)
    {
      Type[] types = new Type[args.Length];
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i] == null)
          types[i] = null;
        else
          types[i] = args[i].GetType();
      }
      return types;
    }

    /// <summary>
    /// Returns the reflection based property identifier for a given property member.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The property identifier for the given property.</returns>
    /// <remarks>
    /// Currently, the identifier is defined to be the full name of the property's declaring type, suffixed with a dot (".") and the
    /// property's name (e.g. MyNamespace.MyType.MyProperty). However, this might change in the future, so this API should be used whenever the
    /// identifier must be retrieved programmatically.
    /// </remarks>
    [Obsolete ("Use MappingConfiguration.NameResolver.GetPropertyName.")]
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Type originalDeclaringType = Remotion.Utilities.ReflectionUtility.GetOriginalDeclaringType (propertyInfo);
      if (originalDeclaringType.IsGenericType)
        return GetPropertyName (originalDeclaringType.GetGenericTypeDefinition (), propertyInfo.Name);
      else
        return GetPropertyName (originalDeclaringType, propertyInfo.Name);
    }

    /// <summary>Returns the property name scoped for a specific <paramref name="originalDeclaringType"/>.</summary>
    [Obsolete ("Use MappingConfiguration.NameResolver.GetPropertyName.")]
    public static string GetPropertyName (Type originalDeclaringType, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", originalDeclaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return originalDeclaringType.FullName + "." + propertyName;
    }

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> is an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </returns>
    public static bool IsObjectList (Type type)
    {
      return Remotion.Utilities.ReflectionUtility.CanAscribe (type, typeof (ObjectList<>));
    }

    /// <summary>
    /// Returns the type parameter of the <see cref="ObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Type"/> if the <paramref name="type"/> is a closed <see cref="ObjectList{T}"/> or <see langword="null"/> if the generic 
    /// <see cref="ObjectList{T}"/> is open.
    /// </returns>
    /// <exception cref="ArgumentTypeException">
    /// Thrown if the type is not an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </exception>
    public static Type GetObjectListTypeParameter (Type type)
    {
      Type[] typeParameters = Remotion.Utilities.ReflectionUtility.GetAscribedGenericArguments (type, typeof (ObjectList<>));
      if (typeParameters == null)
        return null;
      return typeParameters[0];
    }
  }
}
