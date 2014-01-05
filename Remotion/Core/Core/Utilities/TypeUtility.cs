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
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility methods for handling types.
  /// </summary>
  public static class TypeUtility
  {
    /// <summary>
    /// The implementation of <see cref="TypeUtility.ParseAbbreviatedTypeName"/>, implemented in a nested class in order to prevent unnecessary
    /// initialization of pre-compiled regular expressions.
    /// </summary>
    private static class AbbreviationParser
    {
      private static readonly Regex s_enclosedQualifiedTypeRegex;
      private static readonly Regex s_enclosedTypeRegex;
      private static readonly Regex s_typeRegex;

      private static readonly LockingCacheDecorator<string, string> s_fullTypeNames = CacheFactory.CreateWithLocking<string, string>();

      static AbbreviationParser ()
      {
        const string typeNamePattern = //  <asm>::<type>
            @"(?<asm>[^\[\]\,]+)" //       <asm> is the assembly part of the type name (before ::)
            + @"::"
            + @"(?<type>[^\[\]\,]+)"; //   <type> is the partially qualified type name (after ::)

        const string bracketPattern = //   [...] (an optional pair of matching square brackets and anything in between)
            @"(?<br> \[            " //    see "Mastering Regular Expressions" (O'Reilly) for how the construct "balancing group definition" 
            + @"  (                " //    is used to match brackets: http://www.oreilly.com/catalog/regex2/chapter/ch09.pdf
            + @"      [^\[\]]      "
            + @"    |              "
            + @"      \[ (?<d>)    " //    increment nesting counter <d>
            + @"    |              "
            + @"      \] (?<-d>)   " //    decrement <d>
            + @"  )*               "
            + @"  (?(d)(?!))       " //    ensure <d> is 0 before considering next match
            + @"\] )?              ";

        const string strongNameParts = // comma-separated list of name=value pairs
            @"(?<sn> (, \s* \w+ = [^,]+ )* )";

        const string typePattern = // <asm>::<type>[...] (square brackets are optional)
            typeNamePattern
            + bracketPattern;

        const string openUnqualifiedPattern = // requires the pattern to be preceded by [ or ,
            @"(?<= [\[,] )";
        const string closeUnqualifiedPattern = // requires the pattern to be followed by ] or ,
            @"(?= [\],] )";

        const string enclosedTypePattern = // type within argument list
            openUnqualifiedPattern
            + typePattern
            + closeUnqualifiedPattern;

        const string qualifiedTypePattern = // <asm>::<type>[...], name=val, name=val ... (square brackets are optional)
            typePattern
            + strongNameParts;

        const string openQualifiedPattern = // requires the pattern to be preceded by [[ or ,[
            @"(?<= [\[,] \[)";
        const string closeQualifiedPattern = // requires the pattern to be followed by ]] or ],
            @"(?= \] [\],] )";

        const string enclosedQualifiedTypePattern = // qualified type within argument list
            openQualifiedPattern
            + qualifiedTypePattern
            + closeQualifiedPattern;

        const RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;
        s_enclosedQualifiedTypeRegex = new Regex (enclosedQualifiedTypePattern, options);
        s_enclosedTypeRegex = new Regex (enclosedTypePattern, options);
        s_typeRegex = new Regex (typePattern, options);
      }

      public static string ParseAbbreviatedTypeNameWithCache (string abbreviatedTypeName)
      {
        if (abbreviatedTypeName == null)
          return null;

        return s_fullTypeNames.GetOrCreateValue (abbreviatedTypeName, ParseAbbreviatedTypeName);
      }

      private static string ParseAbbreviatedTypeName (string abbreviatedTypeName)
      {
        string fullTypeName = abbreviatedTypeName;
        const string replace = @"${asm}.${type}${br}, ${asm}";
        fullTypeName = ReplaceRecursive (s_enclosedQualifiedTypeRegex, fullTypeName, replace + "${sn}");
        fullTypeName = ReplaceRecursive (s_enclosedTypeRegex, fullTypeName, "[" + replace + "]");
        fullTypeName = s_typeRegex.Replace (fullTypeName, replace);

        return fullTypeName;
      }

      private static string ReplaceRecursive (Regex regex, string input, string replacement)
      {
        string result = regex.Replace (input, replacement);
        while (result != input)
        {
          input = result;
          result = regex.Replace (input, replacement);
        }
        return result;
      }
    }

    private static readonly ICache<Type, string> s_partialAssemblyQualifiedNameCache = CacheFactory.CreateWithLocking<Type, string>();

    /// <summary>
    ///   Converts abbreviated qualified type names into standard qualified type names.
    /// </summary>
    /// <remarks>
    ///   Abbreviated type names use the format <c>assemblyname::subnamespace.type</c>. For instance, the
    ///   abbreviated type name <c>"Remotion.Web::Utilities.ControlHelper"</c> would result in the standard
    ///   type name <c>"Remotion.Web.Utilities.ControlHelper, Remotion.Web"</c>.
    /// </remarks>
    /// <param name="abbreviatedTypeName"> A standard or abbreviated type name. </param>
    /// <returns> A standard type name as expected by <see cref="Type.GetType(string)"/>. </returns>
    public static string ParseAbbreviatedTypeName (string abbreviatedTypeName)
    {
      return AbbreviationParser.ParseAbbreviatedTypeNameWithCache (abbreviatedTypeName);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="ContextAwareTypeDiscoveryUtility"/>. By default, it will search all assemblies for the requested type.
    /// In the designer context, <see cref="IDesignerHost"/> is used for the lookup.
    /// </remarks>
    [CanBeNull]
    public static Type GetType (string abbreviatedTypeName)
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
    public static Type GetType (string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return ContextAwareTypeDiscoveryUtility.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    [Obsolete (
        "GetType is now designer-aware, and the designer does not support case-insensitive type lookup. If type lookup with case insensitivity "
        + "is required, use Type.GetType. To use abbreviated type names for the lookup, use ParseAbbreviatedTypeName.", true)]
    [CanBeNull]
    public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
    }

    public static string GetPartialAssemblyQualifiedName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return s_partialAssemblyQualifiedNameCache.GetOrCreateValue (type, key => type.FullName + ", " + type.Assembly.GetName ().Name);
    }

    public static Type GetDesignModeType (string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return ContextAwareTypeDiscoveryUtility.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    /// Gets the type name in abbreviated syntax (<see cref="ParseAbbreviatedTypeName"/>).
    /// </summary>
    public static string GetAbbreviatedTypeName (Type type, bool includeVersionAndCulture)
    {
      StringBuilder sb = new StringBuilder (50);
      BuildAbbreviatedTypeName (sb, type, includeVersionAndCulture, false);
      return sb.ToString();
    }

    private static void BuildAbbreviatedTypeName (StringBuilder sb, Type type, bool includeVersionAndCulture, bool isTypeParameter)
    {
      string ns = type.Namespace;
      string asm = type.Assembly.GetName().Name;
      bool canAbbreviate = ns.StartsWith (asm);

      // put type paramters in [brackets] if they include commas, so the commas cannot be confused with type parameter separators
      bool needsBrackets = isTypeParameter && (includeVersionAndCulture || ! canAbbreviate);
      if (needsBrackets)
        sb.Append ("[");

      if (canAbbreviate)
      {
        sb.Append (asm).Append ("::");

        if (ns.Length > asm.Length)
          sb.Append (ns.Substring (asm.Length + 1)).Append ('.').Append (type.Name);
        else
          sb.Append (type.Name);

        BuildAbbreviatedTypeParameters (sb, type, includeVersionAndCulture);
      }
      else
      {
        sb.Append (ns).Append (".").Append (type.Name);
        BuildAbbreviatedTypeParameters (sb, type, includeVersionAndCulture);
        sb.Append (", ").Append (asm);
      }

      if (includeVersionAndCulture)
        sb.Append (type.Assembly.FullName.Substring (asm.Length));

      if (needsBrackets)
        sb.Append ("]");
    }

    private static void BuildAbbreviatedTypeParameters (StringBuilder sb, Type type, bool includeVersionAndCulture)
    {
      if (type.ContainsGenericParameters)
      {
        sb.Append ("[");
        Type[] typeParams = type.GetGenericArguments();
        for (int i = 0; i < typeParams.Length; ++i)
        {
          if (i > 0)
            sb.Append (", ");

          Type typeParam = typeParams[i];
          BuildAbbreviatedTypeName (sb, typeParam, includeVersionAndCulture, true);
        }
        sb.Append ("]");
      }
    }
  }
}