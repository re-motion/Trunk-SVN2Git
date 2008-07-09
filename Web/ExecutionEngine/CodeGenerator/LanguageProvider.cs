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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public enum CodeLineType
  {
    Other,
    LineComment,
    NamespaceImport,
    NamespaceDeclaration,
    ClassDeclaration
  }

  public abstract class LanguageProvider
  {
    public abstract CodeLineType ParseLine (string line, out string argument);
    public abstract string ConvertTypeName (string type);
  }

  public abstract class RegexProvider: LanguageProvider
  {
    public abstract string LineComment { get; }
    public abstract Regex ImportNamespaceExpr { get; }
    public abstract Regex ClassDeclarationExpr { get; }
    public abstract Regex NamespaceDeclarationExpr { get; }
    public abstract Dictionary<string, string> BuiltInTypes { get; } 
    public abstract Regex TypeNameExpr { get; }

    public override CodeLineType ParseLine (string line, out string argument)
    {
      string trimmedLine = line.TrimStart ();
      Match match;
      if (trimmedLine.StartsWith (LineComment))
      {
        argument = trimmedLine.Substring (LineComment.Length);
        return CodeLineType.LineComment;
      }
      else if ((match = ImportNamespaceExpr.Match (line)).Success)
      {
        argument = match.Groups["namespace"].Value;
        return CodeLineType.NamespaceImport;
      }
      else if ((match = NamespaceDeclarationExpr.Match (line)).Success)
      {
        argument = match.Groups["namespace"].Value;
        return CodeLineType.NamespaceDeclaration;
      }
      else if ((match = ClassDeclarationExpr.Match (line)).Success)
      {
        argument = match.Groups["class"].Value;
        return CodeLineType.ClassDeclaration;
      }
      else
      {
        argument = null;
        return CodeLineType.Other;
      }
    }

    public override string ConvertTypeName (string type)
    {
      type = TypeNameExpr.Replace (
          type,
          delegate (Match match)
          {
            string basetype = match.Groups["basetype"].Value;
            string result;
            if (BuiltInTypes.TryGetValue (basetype, out result))
              return result;
            else
              return basetype;
          });

      return type.Replace ('{', '<').Replace ('}', '>');
    }
  }

  public class CSharpProvider : RegexProvider
  {
    private const string c_prefix = "//";
    private const RegexOptions c_regexOptions = RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

    private static Regex s_importNamespaceExpr = new Regex (@"^ \s* using \s+ (?<namespace> [\w._]+) ; \s* (//|$)", c_regexOptions);
    private static Regex s_classDeclarationExpr = new Regex (@"^ \s* ((new|public|private|protected|internal|abstract|partial) \s+)* class \s+ (?<class> [\w_]+)", c_regexOptions);    
    private static Regex s_namespaceDeclarationExpr = new Regex (@"^ \s* namespace \s+ (?<namespace> [\w_.]+) \s* (//|{|$)", c_regexOptions);                             
    private static Dictionary<string, string> s_builtInTypes = new Dictionary<string, string> ();

    private static Regex s_typeNameExpr = new Regex (@"(?<basetype> @?\w+)", c_regexOptions | RegexOptions.IgnoreCase);

    static CSharpProvider ()
    {
      s_builtInTypes.Add ("bool", "System.Boolean");
      s_builtInTypes.Add ("byte", "System.Byte");
      s_builtInTypes.Add ("sbyte", "System.SByte");
      s_builtInTypes.Add ("char", "System.Char");
      s_builtInTypes.Add ("decimal", "System.Decimal");
      s_builtInTypes.Add ("double", "System.Double");
      s_builtInTypes.Add ("float", "System.Single");
      s_builtInTypes.Add ("int", "System.Int32");
      s_builtInTypes.Add ("uint", "System.UInt32");
      s_builtInTypes.Add ("long", "System.Int64");
      s_builtInTypes.Add ("ulong", "System.UInt64");
      s_builtInTypes.Add ("object", "System.Object");
      s_builtInTypes.Add ("short", "System.Int16");
      s_builtInTypes.Add ("ushort", "System.UInt16");
      s_builtInTypes.Add ("string", "System.String");      
    }

    public override string LineComment
    {
      get { return c_prefix; }
    }

    public override Regex ImportNamespaceExpr
    {
      get { return s_importNamespaceExpr; }
    }

    public override Regex ClassDeclarationExpr
    {
      get { return s_classDeclarationExpr; }
    }

    public override Regex NamespaceDeclarationExpr
    {
      get { return s_namespaceDeclarationExpr; }
    }

    public override Dictionary<string, string> BuiltInTypes
    {
      get { return s_builtInTypes; }
    }

    public override Regex TypeNameExpr
    {
      get { return s_typeNameExpr; }
    }
  }

  public class CaseInsensitiveStringComparer: StringComparer
  {
    public override int Compare (string x, string y)
    {
      return string.Compare (x, y, true);
    }

    public override bool Equals (string x, string y)
    {
      return string.Compare (x, y, true) == 0;
    }

    public override int GetHashCode (string obj)
    {
      return obj.ToLowerInvariant().GetHashCode();
    }
  }

  public class VBProvider : RegexProvider
  {
    private const string c_prefix = "'";
    private const RegexOptions c_regexOptions = RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase;

    private static Regex s_importNamespaceExpr = new Regex (@"^ \s* Imports \s+ (?<namespace> [\w._]+) \s+ ('|$)", c_regexOptions);
    private static Regex s_classDeclarationExpr = new Regex (@"^ \s* ((Public|Private|Protected|Friend|Shadows|MustInherit|Partial) \s+)* Class \s+ (?<class> [\w_]+)", c_regexOptions);
    private static Regex s_namespaceDeclarationExpr = new Regex (@"^ \s* Namespace \s+ (?<namespace> [\w_.]+) \s* ('|$)", c_regexOptions);
    private static Dictionary<string, string> s_builtInTypes = new Dictionary<string, string> (new CaseInsensitiveStringComparer());

    private static Regex s_typeNameExpr = new Regex (@"(?<basetype> (\w+ | \[\w+\]))", c_regexOptions | RegexOptions.IgnoreCase);

    static VBProvider ()
    {
      s_builtInTypes.Add ("Boolean", "System.Boolean");
      s_builtInTypes.Add ("Byte", "System.Byte");
      s_builtInTypes.Add ("Char", "System.Char");
      s_builtInTypes.Add ("Date", "System.DateTime");
      s_builtInTypes.Add ("Decimal", "System.Decimal");
      s_builtInTypes.Add ("Double", "System.Double");
      s_builtInTypes.Add ("Integer", "System.Int32");
      s_builtInTypes.Add ("Long", "System.Int64");
      s_builtInTypes.Add ("Object", "System.Object");
      s_builtInTypes.Add ("SByte", "System.SByte");
      s_builtInTypes.Add ("Short", "System.Int16");
      s_builtInTypes.Add ("Single", "System.Single");
      s_builtInTypes.Add ("String", "System.String");
      s_builtInTypes.Add ("UInteger", "System.UInt32");
      s_builtInTypes.Add ("ULong", "System.UInt64");
      s_builtInTypes.Add ("UShort", "System.UInt16");
    }

    public override string LineComment
    {
      get { return c_prefix; }
    }

    public override Regex ImportNamespaceExpr
    {
      get { return s_importNamespaceExpr; }
    }

    public override Regex ClassDeclarationExpr
    {
      get { return s_classDeclarationExpr; }
    }

    public override Regex NamespaceDeclarationExpr
    {
      get { return s_namespaceDeclarationExpr; }
    }

    public override Dictionary<string, string> BuiltInTypes
    {
      get { return s_builtInTypes; }
    }

    public override Regex TypeNameExpr
    {
      get { return s_typeNameExpr; }
    }
  }
}
