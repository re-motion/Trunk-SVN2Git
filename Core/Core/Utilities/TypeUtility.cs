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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using Remotion.Collections;
using R = System.Text.RegularExpressions;

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
      public static readonly Regex s_enclosedQualifiedTypeRegex;
      public static readonly Regex s_enclosedTypeRegex;
      public static readonly Regex s_typeRegex;

      public static InterlockedCache<string, string> s_fullTypeNames = new InterlockedCache<string, string>();

      static AbbreviationParser ()
      {
        string typeNamePattern =                // <asm>::<type>
              @"(?<asm>[^\[\]\,]+)"             //    <asm> is the assembly part of the type name (before ::)
            + @"::"
            + @"(?<type>[^\[\]\,]+)";           //    <type> is the partially qualified type name (after ::)

        string bracketPattern =                 // [...] (an optional pair of matching square brackets and anything in between)
              @"(?<br> \[          "            //    see "Mastering Regular Expressions" (O'Reilly) for how the construct "balancing group definition" 
            + @"  (                "            //    is used to match brackets: http://www.oreilly.com/catalog/regex2/chapter/ch09.pdf
            + @"      [^\[\]]      "
            + @"    |              "
            + @"      \[ (?<d>)    "            //    increment nesting counter <d>
            + @"    |              "
            + @"      \] (?<-d>)   "            //    decrement <d>
            + @"  )*               "
            + @"  (?(d)(?!))       "            //    ensure <d> is 0 before considering next match
            + @"\] )?              ";

        string strongNameParts =                // comma-separated list of name=value pairs
              @"(?<sn> (, \s* \w+ = [^,]+ )* )";

        string typePattern =                    // <asm>::<type>[...] (square brackets are optional)
              typeNamePattern
            + bracketPattern;

        string openUnqualifiedPattern =         // requires the pattern to be preceded by [ or ,
              @"(?<= [\[,] )";
        string closeUnqualifiedPattern =        // requires the pattern to be followed by ] or ,
              @"(?= [\],] )";

        string enclosedTypePattern =            // type within argument list
              openUnqualifiedPattern
            + typePattern
            + closeUnqualifiedPattern;

        string qualifiedTypePattern =           // <asm>::<type>[...], name=val, name=val ... (square brackets are optional)
              typePattern
            + strongNameParts;

        string openQualifiedPattern =           // requires the pattern to be preceded by [[ or ,[
              @"(?<= [\[,] \[)";
        string closeQualifiedPattern =          // requires the pattern to be followed by ]] or ],
              @"(?= \] [\],] )";

        string enclosedQualifiedTypePattern =   // qualified type within argument list
              openQualifiedPattern
            + qualifiedTypePattern
            + closeQualifiedPattern;

        RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;
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

      private static string ParseAbbreviatedTypeName  (string abbreviatedTypeName)
      {
        string fullTypeName = abbreviatedTypeName;
        string replace = @"${asm}.${type}${br}, ${asm}";
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

    /// <summary>
    ///   Converts abbreviated qualified type names into standard qualified type names.
    /// </summary>
    /// <remarks>
    ///   Abbreviated type names use the format <c>assemblyname::subnamespace.type</c>. For instance, the
    ///   abbreviated type name <c>"Remotion.Web::Utilities.ControlHelper"</c> would result in the standard
    ///   type name <c>"Remotion.Web.Utilities.ControlHelper, Remotion.Web"</c>.
    /// </remarks>
    /// <param name="abbreviatedTypeName"> A standard or abbreviated type name. </param>
    /// <returns> A standard type name as expected by <see cref="Type.GetType"/>. </returns>
    public static string ParseAbbreviatedTypeName (string abbreviatedTypeName)
    {
      return AbbreviationParser.ParseAbbreviatedTypeNameWithCache (abbreviatedTypeName);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName));
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
    }

    public static string GetPartialAssemblyQualifiedName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return type.FullName + ", " + type.Assembly.GetName().Name;
    }

    public static Type GetDesignModeType (string abbreviatedTypeName, ISite site, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
      ArgumentUtility.CheckNotNull ("site", site);

      IDesignerHost designerHost = (IDesignerHost) site.GetService (typeof (IDesignerHost));
      Assertion.IsNotNull (designerHost, "No IDesignerHost found.");
      Type type = designerHost.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName));
      if (type == null && throwOnError)
        throw new TypeLoadException (string.Format ("Could not load type '{0}'.", ParseAbbreviatedTypeName (abbreviatedTypeName)));
      return type;
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
        sb.Append (asm).Append ("::").Append (ns.Substring (asm.Length)).Append (type.Name);
        BuildAbbreviatedTypeParameters (sb, type, includeVersionAndCulture);
      }
      else
      {
        sb.Append (ns).Append (type.Name);
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
