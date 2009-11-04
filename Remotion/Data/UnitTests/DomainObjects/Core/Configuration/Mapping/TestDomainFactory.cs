// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomainSimple = Compile (
        @"DomainObjects\Core\Configuration\Mapping\TestDomain\Simple",
        @"DomainObjects.Core.Configuration.Dlls\Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Simple.dll");

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"DomainObjects\Core\Configuration\Mapping\TestDomain\Empty",
        @"DomainObjects.Core.Configuration.Dlls\Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Empty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainErrors = Compile (
        @"DomainObjects\Core\Configuration\Mapping\TestDomain\Errors",
        @"DomainObjects.Core.Configuration.Dlls\Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.dll");

    public static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      string outputAssemblyDirectory = Path.GetDirectoryName (Path.GetFullPath (outputAssembly));
      if (!Directory.Exists (outputAssemblyDirectory))
        Directory.CreateDirectory (outputAssemblyDirectory);

      AssemblyCompiler compiler = new AssemblyCompiler (sourceDirectory, outputAssembly,
                                                        ArrayUtility.Combine (
                                                            new[]
                                                              {
                                                                  "Remotion.Interfaces.dll", "Remotion.dll", "Remotion.Data.Interfaces.dll",
                                                                  "Remotion.Data.DomainObjects.dll", "Remotion.ObjectBinding.Interfaces.dll",
                                                                  "Remotion.ObjectBinding.dll"
                                                              }, referencedAssemblies));

      try
      {
        compiler.Compile ();
      }
      catch (Exception e)
      {
        Console.WriteLine (e.Message);
        throw;
      }

      return compiler.CompiledAssembly;
    }
  }
}
