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
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Remotion.Text.CommandLine;

namespace Remotion.Data.DomainObjects.RdbmsTools
{
  [Flags]
  public enum OperationMode
  {
    [CommandLineMode ("schema", Description = "Generate the database setup script(s).")]
    BuildSchema = 1,
  }

  /// <summary>
  /// <see cref="RdbmsToolsParameter"/> type is a combination of a parameter object for <see cref="RdbmsToolsRunner"/> 
  /// and a command line arguments class as required by <see cref="CommandLineClassParser"/>.
  /// </summary>
  [Serializable]
  public class RdbmsToolsParameter
  {
    [CommandLineModeArgument (true)]
    public OperationMode Mode = OperationMode.BuildSchema;

    [CommandLineStringArgument ("baseDirectory", true,
        Description = "The base directory to use for looking up the files to be processed (default: current).",
        Placeholder = "directory")]
    public string BaseDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument ("config", true,
        Description = 
            "The config file holding the application's configuration. "
            + "Unless the path is rooted, the config file is located relative to the curent directory.",
        Placeholder = "app.config")]
    public string ConfigFile = string.Empty;

    [CommandLineStringArgument ("schemaDirectory", true,
        Description = "Create schema file(s) in this directory (default: current).",
        Placeholder = "directory")]
    public string SchemaOutputDirectory = string.Empty;

    [CommandLineStringArgument ("schemaBuilder", true,
        Description = "The assembly qualified type name of the schema file builder to use for generating the SQL scripts.",
        Placeholder = "Namespace.ClassName,AssemblyName")]
    public string SchemaFileBuilderTypeName = typeof (FileBuilder).AssemblyQualifiedName;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public RdbmsToolsParameter ()
    {
    }
  }
}
