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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    [SetUp]
    public virtual void SetUp()
    {
      ResetGeneratedAssemblies ();
      ConcreteTypeBuilder.SetCurrent (null);
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
        File.Delete (file);
    }

    [TearDown]
    public virtual void TearDown()
    {

#if !NO_PEVERIFY
      string[] paths;
      try
      {
        paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
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
      ConcreteTypeBuilder.SetCurrent (null);
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildFromActive().ForClass (targetType).Clear().AddMixins (mixinTypes).EnterScope())
        return TypeFactory.GetConcreteType (targetType, GenerationPolicy.ForceGeneration);
    }

    public FuncInvokerWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildFromActive().ForClass<T> ().Clear().AddMixins (mixinTypes).EnterScope())
        return ObjectFactory.Create<T>(GenerationPolicy.ForceGeneration);
    }
  }
}