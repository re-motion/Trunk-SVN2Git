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
}