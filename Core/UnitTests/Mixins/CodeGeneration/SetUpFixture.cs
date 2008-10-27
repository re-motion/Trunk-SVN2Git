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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ConcreteTypeBuilder s_savedTypeBuilder;
    private static ConcreteTypeBuilder s_alternativeTypeBuilder;

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
      ResetGeneratedAssemblies (); // delete assemblies if everything went fine


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