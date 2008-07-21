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
using System.IO;
using System.Reflection;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomainSimple = Compile (
        @"DomainObjects\Configuration\Mapping\TestDomain\Simple",
        @"DomainObjects.Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Simple.dll");

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"DomainObjects\Configuration\Mapping\TestDomain\Empty",
        @"DomainObjects.Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Empty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainErrors = Compile (
        @"DomainObjects\Configuration\Mapping\TestDomain\Errors",
        @"DomainObjects.Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.TestDomain.Errors.dll");

    public static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      string outputAssemblyDirectory = Path.GetDirectoryName (Path.GetFullPath (outputAssembly));
      if (!Directory.Exists (outputAssemblyDirectory))
        Directory.CreateDirectory (outputAssemblyDirectory);
      
      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          ArrayUtility.Combine (new string[] { "Remotion.Interfaces.dll", "Remotion.dll", "Remotion.Data.Interfaces.dll", "Remotion.Data.DomainObjects.dll" }, referencedAssemblies));

      compiler.Compile ();
      return compiler.CompiledAssembly;
    }
  }
}
