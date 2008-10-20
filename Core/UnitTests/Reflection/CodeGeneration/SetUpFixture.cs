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
using System.Diagnostics;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ModuleScope s_scope;
    private static ModuleScope s_unsavedScope;

    public static ModuleScope Scope
    {
      get
      {
        if (s_scope == null)
          throw new InvalidOperationException ("SetUp must be called before the scope is accessed.");
        return s_scope;
      }
    }

    public static ModuleScope UnsavedScope
    {
      get
      {
        if (s_scope == null)
          throw new InvalidOperationException ("SetUp must be called before the scope is accessed.");
        return s_unsavedScope;
      }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      Console.WriteLine ("Setting up code generation tests");
      s_scope = new ModuleScope (true, "Remotion.Reflection.CodeGeneration.Generated.Signed", "Remotion.Reflection.CodeGeneration.Generated.Signed.dll", "Remotion.Reflection.CodeGeneration.Generated.Unsigned", "Remotion.Reflection.CodeGeneration.Generated.Unsigned.dll");
      s_unsavedScope = new ModuleScope (true);
      DeleteIfExists (Path.Combine (s_scope.StrongNamedModuleDirectory ?? Environment.CurrentDirectory, s_scope.StrongNamedModuleName));
      DeleteIfExists (Path.Combine (s_scope.WeakNamedModuleDirectory ?? Environment.CurrentDirectory, s_scope.WeakNamedModuleName));
    }

    [TearDown]
    public virtual void TearDown ()
    {
      Console.WriteLine ("Tearing down code generation tests");
#if !NO_PEVERIFY
      string[] paths = AssemblySaver.SaveAssemblies (s_scope);
      s_scope = null;
      s_unsavedScope = null;

      foreach (string path in paths)
      {
        PEVerifier.VerifyPEFile (path);
        FileUtility.DeleteAndWaitForCompletion (path);
      }
#endif
    }

    private void DeleteIfExists (string path)
    {
      if (File.Exists (path))
        FileUtility.DeleteAndWaitForCompletion (path);
    }
  }
}