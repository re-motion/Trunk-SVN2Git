using System;
using System.Reflection;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using System.IO;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomainSimple = Compile (
        @"Configuration\Mapping\TestDomain\Simple",
        @"Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Simple.dll");

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"Configuration\Mapping\TestDomain\Empty",
        @"Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Empty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainErrors = Compile (
        @"Configuration\Mapping\TestDomain\Errors",
        @"Configuration.Dlls\Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.dll");

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