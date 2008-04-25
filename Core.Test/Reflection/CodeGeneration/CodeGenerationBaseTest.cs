using System;
using System.IO;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    private ModuleScope _scope;
    private bool _assemblySaveSuppressed;

    [SetUp]
    public virtual void SetUp ()
    {
      _scope = new ModuleScope (true);
      _assemblySaveSuppressed = false;
      DeleteIfExists (Path.Combine (_scope.StrongNamedModuleDirectory ?? Environment.CurrentDirectory, _scope.StrongNamedModuleName));
      DeleteIfExists (Path.Combine (_scope.WeakNamedModuleDirectory ?? Environment.CurrentDirectory, _scope.WeakNamedModuleName));
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (!_assemblySaveSuppressed)
      {
        string[] paths = AssemblySaver.SaveAssemblies (_scope);
        foreach (string path in paths)
        {
          PEVerifier.VerifyPEFile (path);
          FileUtility.DeleteAndWaitForCompletion (path);
        }
      }
    }

    private void DeleteIfExists (string path)
    {
      if (File.Exists (path))
        FileUtility.DeleteAndWaitForCompletion (path);
    }

    public ModuleScope Scope
    {
      get { return _scope; }
    }

    public void SuppressAssemblySave ()
    {
      _assemblySaveSuppressed = true;
    }
  }
}