/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Text.CommandLine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public enum Language
  {
    CSharp,
    VB
  }
  
  public class Arguments
  {
    [CommandLineStringArgument (false,
        Description = "File name or file mask for the input file(s)",
        Placeholder = "filemask")]
    public string FileMask;

    [CommandLineStringArgument (false,
        Description = "Output file",
        Placeholder = "outputfile")]
    public string OutputFile;

    [CommandLineFlagArgument ("recursive", false,
        Description = "Resolve file mask recursively (default is off)")]
    public bool Recursive;

    [CommandLineEnumArgument ("language", true, 
        Description = "Language (default is CSharp)",
        Placeholder = "{CSharp|VB}")]
    public Language Language = Language.CSharp;

    //[CommandLineStringArgument ("lineprefix", true,
    //    Description = "Line prefix for WxePageFunction elements (default is // for C#, ' for VB.NET")]
    //public string LinePrefix = null;

    //[CommandLineStringArgument ("using", true,
    //    Description = "Regular expression for parsing namespace import statements")]
    //public string UsingExpression = null;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose error information (default is off)")]
    public bool Verbose;

    [CommandLineStringArgument ("prjfile", true,
        Description = "Visual Studio project file (csprj). If specified, the output file is only generated if any of the input files OR the project file is newer than the output file.")]
    public string ProjectFile;

    [CommandLineStringArgument ("functionbase", true,
        Description = "Default base type for generated WXE functions (default is Remotion.Web.ExecutionEngine.WxeFunction).")]
    public string FunctionBaseType = typeof(WxeFunction).FullName;
  }
}