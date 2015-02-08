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
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using JetBrains.Annotations;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility methods for handling types.
  /// </summary>
  public static partial class TypeUtility
  {
    #region Obsolete

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    [Obsolete (
        "GetType is now designer-aware, and the designer does not support case-insensitive type lookup. If type lookup with case insensitivity "
        + "is required, use Type.GetType. To use abbreviated type names for the lookup, use ParseAbbreviatedTypeName.", true)]
    [CanBeNull]
    public static Type GetType ([NotNull]string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
    }

    [Obsolete ("Use GetType (string, bool) instead. (Version 1.15.30.0)")]
    public static Type GetDesignModeType ([NotNull]string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return GetType (abbreviatedTypeName, throwOnError);
    }

    #endregion

    private static readonly ConcurrentDictionary<string, string> s_fullTypeNames = new ConcurrentDictionary<string, string> ();
    private static readonly ConcurrentDictionary<Type, string> s_partialAssemblyQualifiedNameCache = new ConcurrentDictionary<Type, string>();

    /// <summary>The <see cref="Lazy{T}"/> protects the expensive regex-creation.</summary>
    private static readonly Lazy<AbbreviationParser> s_abbreviationParser = new Lazy<AbbreviationParser> (() => new AbbreviationParser());

    private static readonly AbbreviationBuilder s_abbreviationBuilder = new AbbreviationBuilder();

    /// <summary>
    ///   Converts abbreviated qualified type names into standard qualified type names.
    /// </summary>
    /// <remarks>
    ///   Abbreviated type names use the format <c>assemblyname::subnamespace.type</c>. For instance, the
    ///   abbreviated type name <c>"Remotion.Web::Utilities.ControlHelper"</c> would result in the standard
    ///   type name <c>"Remotion.Web.Utilities.ControlHelper, Remotion.Web"</c>.
    /// </remarks>
    /// <param name="typeName"> A standard or abbreviated type name. </param>
    /// <returns> A standard type name as expected by <see cref="Type.GetType(string)"/>. </returns>
    [CanBeNull]
    [ContractAnnotation ("typeName:notnull => notnull;typeName:null => null")]
    public static string ParseAbbreviatedTypeName ([CanBeNull]string typeName)
    {
      if (typeName == null)
        return null;

      return s_fullTypeNames.GetOrAdd (typeName, ParseAbbreviatedTypeNameWithoutCache);
    }

    private static string ParseAbbreviatedTypeNameWithoutCache ([NotNull] string typeName)
    {
      // Optimization to prevent instantiating the AbbreviationParser unless necessary.
      if (!AbbreviationParser.IsAbbreviatedTypeName (typeName))
        return typeName;

      return s_abbreviationParser.Value.ParseAbbreviatedTypeName (typeName);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="ContextAwareTypeDiscoveryUtility"/>. By default, it will search all assemblies for the requested type.
    /// In the designer context, <see cref="IDesignerHost"/> is used for the lookup.
    /// </remarks>
    [CanBeNull]
    public static Type GetType ([NotNull]string abbreviatedTypeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return GetType (abbreviatedTypeName, false);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="ContextAwareTypeDiscoveryUtility"/>. By default, it will search all assemblies for the requested type.
    /// In the designer context, <see cref="IDesignerHost"/> is used for the lookup.
    /// </remarks>
    [CanBeNull]
    [ContractAnnotation ("throwOnError:true => notnull")]
    public static Type GetType ([NotNull]string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return ContextAwareTypeDiscoveryUtility.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    /// Gets the type and assembly name without the version, culture, and public key token.
    /// </summary>
    [NotNull]
    public static string GetPartialAssemblyQualifiedName ([NotNull]Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return s_partialAssemblyQualifiedNameCache.GetOrAdd (type, key => key.FullName + ", " + key.Assembly.GetName ().Name);
    }

    /// <summary>
    /// Gets the type name in abbreviated syntax (<see cref="ParseAbbreviatedTypeName"/>).
    /// </summary>
    [NotNull]
    public static string GetAbbreviatedTypeName ([NotNull]Type type, bool includeVersionAndCulture)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return s_abbreviationBuilder.BuildAbbreviatedTypeName (type, includeVersionAndCulture);
    }
  }
}