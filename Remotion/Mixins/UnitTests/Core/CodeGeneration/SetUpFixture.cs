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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.CodeGeneration;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ConcreteTypeBuilder s_savedTypeBuilder;

    private static bool _skipDeletion = false;
    private ResetCheckingModuleManager _moduleManager;

    /// <summary>
    /// Signals that the <see cref="SetUpFixture"/> should not delete the files it generates. Call this ad-hoc in a test to keep the files and inspect
    /// them with Reflector or ildasm.
    /// </summary>
    public static void SkipDeletion ()
    {
      _skipDeletion = true;
    }

    [SetUp]
    public void SetUp()
    {
      ResetGeneratedAssemblies ();
      _moduleManager = new ResetCheckingModuleManager (false);
      s_savedTypeBuilder = new ConcreteTypeBuilder (_moduleManager, new GuidNameProvider(), new GuidNameProvider());
    }

    [TearDown]
    public void TearDown()
    {
#if !NO_PEVERIFY
      string[] paths;
      try
      {
        _moduleManager.AllowReset = true;
        paths = s_savedTypeBuilder.SaveGeneratedConcreteTypes ();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assemblies: {0}", ex);
        throw;
      }

      foreach (string path in paths)
        PEVerifier.CreateDefault ().VerifyPEFile (path);

#endif

      if (!_skipDeletion)
        ResetGeneratedAssemblies (); // delete assemblies if everything went fine
      else
        Console.WriteLine ("Assemblies saved to: " + Environment.NewLine + SeparatedStringBuilder.Build (Environment.NewLine, paths));
      
      s_savedTypeBuilder = null;
    }

    public static ConcreteTypeBuilder SavedTypeBuilder
    {
      get
      {
        if (s_savedTypeBuilder == null)
          throw new InvalidOperationException ("SetUp must be executed first.");
        return s_savedTypeBuilder;
      }
    }

    private void ResetGeneratedAssemblies ()
    {
      string weakModulePath = ModuleManager.DefaultWeakModulePath.Replace ("{counter}", "*");
      string strongModulePath = ModuleManager.DefaultStrongModulePath.Replace ("{counter}", "*");
      string weakPdbPath = Path.GetFileNameWithoutExtension (weakModulePath) + ".pdb";
      string strongPdbPath = Path.GetFileNameWithoutExtension (strongModulePath) + ".pdb";

      DeleteFiles (weakModulePath);
      DeleteFiles (strongModulePath);
      DeleteFiles (weakPdbPath);
      DeleteFiles (strongPdbPath);
    }

    private void DeleteFiles (string searchPattern)
    {
      foreach (string file in Directory.GetFiles (Environment.CurrentDirectory, searchPattern))
        FileUtility.DeleteAndWaitForCompletion (file);
    }
  }
}
