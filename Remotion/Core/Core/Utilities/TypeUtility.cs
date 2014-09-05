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
    private class AbbreviationParser
    {
      private readonly Regex _enclosedQualifiedTypeRegex;
      private readonly Regex _enclosedTypeRegex;
      private readonly Regex _typeRegex;

      public AbbreviationParser ()
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

        // Do not use RegexOptions.Compiled because it takes 200ms to compile which is not offset by the calls made after cache lookups.
        // This is an issue in .NET up to at least version 4.5.1 in x64 mode.
        const RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;
        _enclosedQualifiedTypeRegex = new Regex (enclosedQualifiedTypePattern, options);
        _enclosedTypeRegex = new Regex (enclosedTypePattern, options);
        _typeRegex = new Regex (typePattern, options);
      }

      public string ParseAbbreviatedTypeName (string abbreviatedTypeName)
      {
        ArgumentUtility.CheckNotNull ("abbreviatedTypeName", abbreviatedTypeName);

        string fullTypeName = abbreviatedTypeName;
        const string replace = @"${asm}.${type}${br}, ${asm}";
        fullTypeName = ReplaceRecursive (_enclosedQualifiedTypeRegex, fullTypeName, replace + "${sn}");
        fullTypeName = ReplaceRecursive (_enclosedTypeRegex, fullTypeName, "[" + replace + "]");
        fullTypeName = _typeRegex.Replace (fullTypeName, replace);

        return fullTypeName;
      }

      private string ReplaceRecursive (Regex regex, string input, string replacement)
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

    private static readonly ConcurrentDictionary<string, string> s_fullTypeNames = new ConcurrentDictionary<string, string> ();
    private static readonly ConcurrentDictionary<Type, string> s_partialAssemblyQualifiedNameCache = new ConcurrentDictionary<Type, string>();

    /// <summary>The <see cref="Lazy{T}"/> protects the expensive regex-creation.</summary>
    private static readonly Lazy<AbbreviationParser> s_abbreviationParser = new Lazy<AbbreviationParser> (() => new AbbreviationParser());

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
    public static string ParseAbbreviatedTypeName ([CanBeNull]string abbreviatedTypeName)
    {
      if (abbreviatedTypeName == null)
        return null;

      return s_fullTypeNames.GetOrAdd (abbreviatedTypeName, AbbreviatedTypeNameWithoutCache);
    }

    private static string AbbreviatedTypeNameWithoutCache ([NotNull] string abbreviatedTypeName)
    {
      if (!abbreviatedTypeName.Contains ("::"))
        return abbreviatedTypeName;

      return s_abbreviationParser.Value.ParseAbbreviatedTypeName (abbreviatedTypeName);
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
    public static Type GetType ([NotNull]string abbreviatedTypeName, bool throwOnError)
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
    public static Type GetType ([NotNull]string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
    }

    public static string GetPartialAssemblyQualifiedName ([NotNull]Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return s_partialAssemblyQualifiedNameCache.GetOrAdd (type, key => key.FullName + ", " + key.Assembly.GetName ().Name);
    }

    public static Type GetDesignModeType ([NotNull]string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return ContextAwareTypeDiscoveryUtility.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    /// Gets the type name in abbreviated syntax (<see cref="ParseAbbreviatedTypeName"/>).
    /// </summary>
    public static string GetAbbreviatedTypeName ([NotNull]Type type, bool includeVersionAndCulture)
    {
      StringBuilder sb = new StringBuilder (50);
      BuildAbbreviatedTypeName (sb, type, includeVersionAndCulture, false);
      return sb.ToString();
    }

    private static void BuildAbbreviatedTypeName (StringBuilder sb, Type type, bool includeVersionAndCulture, bool isTypeParameter)
    {
      if (type.IsNested)
        throw new NotSupportedException ("Nested types are not supported with abbrivated typename construction.");

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