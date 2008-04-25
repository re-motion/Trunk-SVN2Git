using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public class AssemblyCompiler
  {
    public static AssemblyCompiler CreateInMemoryAssemblyCompiler (string sourceDirectory, params string[] referencedAssemblies)
    {
      return new AssemblyCompiler (sourceDirectory, referencedAssemblies);
    }
    
    private readonly string _sourceDirectory;
    private Assembly _compiledAssembly;
    private readonly CompilerParameters _compilerParameters;

    public AssemblyCompiler (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNullOrEmpty ("outputAssembly", outputAssembly);
      ArgumentUtility.CheckNotNullOrItemsNull ("referencedAssemblies", referencedAssemblies);

      _sourceDirectory = sourceDirectory;

      _compilerParameters = new CompilerParameters ();
      _compilerParameters.GenerateExecutable = false;
      _compilerParameters.OutputAssembly = outputAssembly;
      _compilerParameters.GenerateInMemory = false;
      _compilerParameters.TreatWarningsAsErrors = false;
      _compilerParameters.ReferencedAssemblies.AddRange (referencedAssemblies);
    }

    private AssemblyCompiler (string sourceDirectory, params string[] referencedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNullOrItemsNull ("referencedAssemblies", referencedAssemblies);

      _sourceDirectory = sourceDirectory;

      _compilerParameters = new CompilerParameters ();
      _compilerParameters.GenerateExecutable = false;
      _compilerParameters.OutputAssembly = null;
      _compilerParameters.GenerateInMemory = true;
      _compilerParameters.TreatWarningsAsErrors = false;
      _compilerParameters.ReferencedAssemblies.AddRange (referencedAssemblies);
    }

    public Assembly CompiledAssembly
    {
      get { return _compiledAssembly; }
    }

    public string OutputAssembly
    {
      get { return _compilerParameters.OutputAssembly; }
    }

    public void Compile ()
    {
      _compiledAssembly = null;
      CodeDomProvider provider = new CSharpCodeProvider ();

      string[] sourceFiles = Directory.GetFiles (_sourceDirectory);

      CompilerResults compilerResults = provider.CompileAssemblyFromFile (_compilerParameters, sourceFiles);

      if (compilerResults.Errors.Count > 0)
      {
        StringBuilder errorBuilder = new StringBuilder ();
        errorBuilder.AppendFormat ("Errors building {0} into {1}", _sourceDirectory, compilerResults.PathToAssembly).AppendLine ();
        foreach (CompilerError compilerError in compilerResults.Errors)
          errorBuilder.AppendFormat ("  ").AppendLine (compilerError.ToString ());

        throw new AssemblyCompilationException (errorBuilder.ToString ());
      }

      _compiledAssembly = compilerResults.CompiledAssembly;
    }

    public void CompileInSeparateAppDomain ()
    {
      AppDomain appDomain = null;
      try
      {
        appDomain = AppDomain.CreateDomain (
            "CompilerAppDomain",
            null,
            AppDomain.CurrentDomain.BaseDirectory,
            AppDomain.CurrentDomain.RelativeSearchPath,
            AppDomain.CurrentDomain.ShadowCopyFiles);
        appDomain.DoCallBack (Compile);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);
      }     
    }
  }
}