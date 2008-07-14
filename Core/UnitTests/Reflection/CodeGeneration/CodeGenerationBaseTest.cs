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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.DPExtensions;
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
#if !NO_PEVERIFY
        string[] paths = AssemblySaver.SaveAssemblies (_scope);
        foreach (string path in paths)
        {
          PEVerifier.VerifyPEFile (path);
          FileUtility.DeleteAndWaitForCompletion (path);
        }
#endif
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
