// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Remotion.Implementation
{
  public class FrameworkVersionRetriever
  {
    private readonly string _referenceAssemblyName;
    private readonly IEnumerable<_Assembly> _loadedAssemblies;

    public FrameworkVersionRetriever (string referenceAssemblyName, IEnumerable<_Assembly> loadedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("referenceAssemblyName", referenceAssemblyName);
      ArgumentUtility.CheckNotNull ("loadedAssemblies", loadedAssemblies);

      _referenceAssemblyName = referenceAssemblyName;
      _loadedAssemblies = loadedAssemblies;
    }

    public Version RetrieveVersion()
    {
      var loadedReferencedCandidate = TryGetCandidateFromLoadedAndReferencedAssemblies();
      if (loadedReferencedCandidate != null)
        return loadedReferencedCandidate;

      var diskCandidate = TryLoadCandidateFromDisk ();
      if (diskCandidate != null)
        return diskCandidate;

      string message = string.Format (
          "{0} is neither loaded nor referenced, and trying to load it by name ('{0}') didn't work either.", 
          _referenceAssemblyName);
      throw new FrameworkVersionNotFoundException (message);
    }

    private Version TryGetCandidateFromLoadedAndReferencedAssemblies ()
    {
      var candidates = new List<Version> ();
      foreach (_Assembly assembly in _loadedAssemblies)
      {
        AnalyzeAssemblyName (assembly.GetName (), candidates);

        foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies ())
          AnalyzeAssemblyName (referencedAssembly, candidates);
      }

      if (candidates.Count > 1)
      {
        string message = string.Format ("More than one version of {0} is currently loaded or referenced: {1}.", _referenceAssemblyName,
            GetCandidateString (candidates));
        throw new FrameworkVersionNotFoundException (message);
      }
      else if (candidates.Count == 1)
      {
        return candidates[0];
      }
      else
      {
        return null;
      }
    }

    private Version TryLoadCandidateFromDisk ()
    {
      try
      {
        var assemblyOnDisk = Assembly.Load (new AssemblyName (_referenceAssemblyName));
        return assemblyOnDisk.GetName ().Version;
      }
      catch (FileNotFoundException)
      {
        return null;
      }
    }

    private void AnalyzeAssemblyName (AssemblyName assemblyName, ICollection<Version> candidates)
    {
      if (assemblyName.Name == _referenceAssemblyName && !candidates.Contains (assemblyName.Version))
        candidates.Add (assemblyName.Version);
    }

    private string GetCandidateString (IEnumerable<Version> candidates)
    {
      var candidateStringBuilder = new StringBuilder();
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
