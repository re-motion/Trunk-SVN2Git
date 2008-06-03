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
using Remotion.Text.CommandLine;

namespace Remotion.Security.Metadata.Extractor
{
  public class CommandLineArguments
  {
    [CommandLineStringArgument ("assembly", false,
        Description="The path to the assembly containing the application domain to analyze.",
        Placeholder="assemblyPath")]
    public string DomainAssemblyName;

    [CommandLineStringArgument ("output", false,
        Description = "The name of the XML metadata output file.",
        Placeholder = "metadata")]
    public string MetadataOutputFile;

    [CommandLineStringArgument ("language", true,
        Description="The language code for the multilingual descriptions of the metadata objects.",
        Placeholder="language")]
    public string Languages = string.Empty;

    [CommandLineFlagArgument ("suppress", false,
        Description = "Suppress export of metadata file.")]
    public bool SuppressMetadata = false;

    [CommandLineFlagArgument ("invariant", false,
        Description = "Export multilingual descriptions of the metadata objects for the invariant culture.")]
    public bool InvariantCulture;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public void CheckArguments ()
    {
      if (string.IsNullOrEmpty (Languages) && SuppressMetadata && !InvariantCulture)
        throw new CommandLineArgumentApplicationException ("You must export at least a localization file or a metadata file.");
    }
  }
}
