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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.Security.Metadata;

namespace Remotion.Security.MSBuild.Tasks
{
  public class ExtractSecurityMetadata : Task
  {
    private ITaskItem[] _assemblies;
    private string _outputFilename;

    [Required]
    public ITaskItem[] Assemblies
    {
      get { return _assemblies; }
      set { _assemblies = value; }
    }

    [Required]
    public string OutputFilename
    {
      get { return _outputFilename; }
      set { _outputFilename = value; }
    }

    public override bool Execute ()
    {
      try
      {
        MetadataExtractor extractor = new MetadataExtractor (new MetadataToXmlConverter ());

        foreach (ITaskItem assembly in _assemblies)
        {
          Log.LogMessage (MessageImportance.Low, "Analyzing assembly {0}", assembly.ItemSpec);
          extractor.AddAssembly (assembly.ItemSpec);
        }

        extractor.Save (_outputFilename);
        Log.LogMessage (MessageImportance.Normal, "Extracted security metadata to {0}", _outputFilename);

        return true;
      }
      catch (Exception e)
      {
        Log.LogErrorFromException (e);
        return false;
      }
    }
  }
}
