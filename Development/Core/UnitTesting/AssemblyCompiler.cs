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
