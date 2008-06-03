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
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTool;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  public class MixerToolBaseTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public void SetUp ()
    {
      _parameters = new MixerParameters();
      ResetGeneratedFiles();
    }

    [TearDown]
    public void TearDown ()
    {
      ResetGeneratedFiles ();
    }

    public MixerParameters Parameters
    {
      get { return _parameters; }
    }

    public string UnsignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.UnsignedAssemblyName + ".dll"); }
    }

    public string SignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.SignedAssemblyName + ".dll"); }
    }

    public void ResetGeneratedFiles ()
    {
      if (File.Exists (UnsignedAssemblyPath))
        File.Delete (UnsignedAssemblyPath);
      if (File.Exists (SignedAssemblyPath))
        File.Delete (SignedAssemblyPath);
    }

    public Type GetFirstMixedType (Assembly assembly)
    {
      foreach (Type t in assembly.GetTypes ())
      {
        if (t.IsDefined (typeof (ConcreteMixedTypeAttribute), false))
          return t;
      }
      return null;
    }

    public Set<ClassContext> GetContextsFromGeneratedTypes (Assembly assembly)
    {
      Set<ClassContext> contextsFromTypes = new Set<ClassContext> ();
      foreach (Type concreteType in assembly.GetTypes ())
      {
        ClassContext context = MixinReflector.GetClassContextFromConcreteType (concreteType);
        if (context != null)
          contextsFromTypes.Add (context);
      }
      return contextsFromTypes;
    }
  }
}
