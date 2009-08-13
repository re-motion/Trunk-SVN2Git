// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Mixins.Utilities;
using Remotion.Mixins.CodeGeneration;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ConcreteTypeBuilder s_savedTypeBuilder;
    private static ConcreteTypeBuilder s_alternativeTypeBuilder;

    private static bool _skipDeletion = false;

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
      s_savedTypeBuilder = new ConcreteTypeBuilder ();
      s_alternativeTypeBuilder = new ConcreteTypeBuilder ();
    }

    [TearDown]
    public void TearDown()
    {
      Console.WriteLine (CodeGenerationStatistics.GetStatisticsString());

#if !NO_PEVERIFY
      string[] paths;
      try
      {
        paths = s_savedTypeBuilder.SaveAndResetDynamicScope ();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assemblies: {0}", ex);
        return;
      }

      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);

#endif

      if (!_skipDeletion)
        ResetGeneratedAssemblies (); // delete assemblies if everything went fine
      else
        Console.WriteLine ("Assemblies saved to: " + Environment.NewLine + SeparatedStringBuilder.Build (Environment.NewLine, paths));
      
      s_savedTypeBuilder = null;
      s_alternativeTypeBuilder = null;
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

    public static ConcreteTypeBuilder AlternativeTypeBuilder
    {
      get
      {
        if (s_alternativeTypeBuilder == null)
          throw new InvalidOperationException ("SetUp must be executed first.");
        return s_alternativeTypeBuilder;
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
