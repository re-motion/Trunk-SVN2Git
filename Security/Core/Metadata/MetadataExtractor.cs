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
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class MetadataExtractor
  {
    private IMetadataConverter _converter;
    private List<Assembly> _assemblies;

    public MetadataExtractor (IMetadataConverter converter)
    {
      ArgumentUtility.CheckNotNull ("converter", converter);

      _assemblies = new List<Assembly> ();
      _converter = converter;
    }

    public void AddAssembly (Assembly assembly)
    {
      _assemblies.Add (assembly);
    }

    public void AddAssembly (string assemblyPath)
    {
      if (!assemblyPath.EndsWith (".dll"))
        assemblyPath = assemblyPath + ".dll";

      Assembly assembly = Assembly.LoadFrom (assemblyPath);
      AddAssembly (assembly);
    }

    public void Save (string filename)
    {
      MetadataCache metadata = new MetadataCache ();
      AssemblyReflector reflector = new AssemblyReflector ();

      foreach (Assembly assembly in _assemblies)
        reflector.GetMetadata (assembly, metadata);

      _converter.ConvertAndSave (metadata, filename);
    }
  }
}
