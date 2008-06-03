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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Remotion.Implementation
{
  public class FrameworkVersionRetriever
  {
    private const string c_referenceAssemblyName = "Remotion";
    private readonly IEnumerable<_Assembly> _loadedAssemblies;

    public FrameworkVersionRetriever (IEnumerable<_Assembly> loadedAssemblies)
    {
      _loadedAssemblies = ArgumentUtility.CheckNotNull ("loadedAssemblies", loadedAssemblies);
    }

    public Version RetrieveVersion()
    {
      List<Version> candidates = new List<Version> ();
      foreach (_Assembly assembly in _loadedAssemblies)
      {
        AnalyzeAssemblyName (assembly.GetName(), candidates);

        foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies ())
          AnalyzeAssemblyName (referencedAssembly, candidates);
      }

      if (candidates.Count == 0)
      {
        string message = string.Format ("There is no version of {0} currently loaded or referenced.", c_referenceAssemblyName);
        throw new InvalidOperationException (message);
      }
      else if (candidates.Count > 1)
      {
        string message = string.Format ("More than one version of {0} is currently loaded or referenced: {1}.", c_referenceAssemblyName, 
            GetCandidateString (candidates));
        throw new InvalidOperationException (message);
      }
      else
        return candidates[0];
    }

    private void AnalyzeAssemblyName (AssemblyName assemblyName, ICollection<Version> candidates)
    {
      if (assemblyName.Name == c_referenceAssemblyName && !candidates.Contains (assemblyName.Version))
        candidates.Add (assemblyName.Version);
    }

    private string GetCandidateString (IEnumerable<Version> candidates)
    {
      StringBuilder candidateStringBuilder = new StringBuilder();
      bool first = true;
      foreach (Version candidate in candidates)
      {
        if (!first)
          candidateStringBuilder.Append (", ");
        candidateStringBuilder.Append (candidate);
        first = false;
      }
      return candidateStringBuilder.ToString();
    }

  }
}
