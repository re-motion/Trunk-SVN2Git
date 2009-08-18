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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTool;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  public class MixerToolBaseTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public void SetUp ()
    {
      _parameters = new MixerParameters ();
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
        FileUtility.DeleteAndWaitForCompletion (UnsignedAssemblyPath);
      if (File.Exists (SignedAssemblyPath))
        FileUtility.DeleteAndWaitForCompletion (SignedAssemblyPath);
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
      var contextsFromTypes = new Set<ClassContext> ();
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
