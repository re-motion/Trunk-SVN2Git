// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.IO;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ModuleScope s_scope;

    public static ModuleScope Scope
    {
      get
      {
        if (s_scope == null)
          throw new InvalidOperationException ("SetUp must be called before the scope is accessed.");
        return s_scope;
      }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      Console.WriteLine ("Setting up MethodWrapperEmitterTests");
      s_scope = new ModuleScope (
          true,
          true,
          null,
          null,
          "Remotion.Reflection.CodeGeneration.MethodWrapperEmitterTests.Generated.Unsigned",
          "Remotion.Reflection.CodeGeneration.MethodWrapperEmitterTests.Generated.Unsigned.dll");
      DeleteIfExists (Path.Combine (s_scope.WeakNamedModuleDirectory ?? Environment.CurrentDirectory, s_scope.WeakNamedModuleName));
    }

    [TearDown]
    public virtual void TearDown ()
    {
      Console.WriteLine ("Tearing down MethodWrapperEmitterTests");
#if !NO_PEVERIFY
      var path = s_scope.SaveAssembly (false);
      s_scope = null;
      PEVerifier.CreateDefault().VerifyPEFile (path);
      FileUtility.DeleteAndWaitForCompletion (path);
      FileUtility.DeleteAndWaitForCompletion (path.Replace (".dll", ".pdb"));
#endif
    }

    private void DeleteIfExists (string path)
    {
      if (File.Exists (path))
        FileUtility.DeleteAndWaitForCompletion (path);
    }
  }
}
