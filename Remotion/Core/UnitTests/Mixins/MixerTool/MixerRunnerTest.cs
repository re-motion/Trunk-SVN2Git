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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [TestFixture]
  [Serializable]
  public class MixerRunnerTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public virtual void SetUp ()
    {
      _parameters = new MixerParameters ();
      ResetGeneratedFiles ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      ResetGeneratedFiles ();
    }


    public string UnsignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.UnsignedAssemblyName + ".dll"); }
    }

    public string SignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.SignedAssemblyName + ".dll"); }
    }

    [Test]
    public void ParameterDefaults ()
    {
      Assert.AreEqual (Environment.CurrentDirectory, _parameters.AssemblyOutputDirectory);
      Assert.AreEqual (Environment.CurrentDirectory, _parameters.BaseDirectory);
      Assert.AreEqual ("", _parameters.ConfigFile);
      Assert.AreEqual ("Remotion.Mixins.Persistent.Signed", _parameters.SignedAssemblyName);
      Assert.AreEqual ("Remotion.Mixins.Persistent.Unsigned", _parameters.UnsignedAssemblyName);
      Assert.AreEqual (false, _parameters.KeepTypeNames);
    }

    [Test]
    public void RunDefault ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      var runner = new MixerRunner (_parameters);
      runner.Run ();
      Assert.IsTrue (File.Exists (UnsignedAssemblyPath));
    }

    [Test]
    public void RunWithDifferentBaseDirectory ()
    {
      string basePath = Path.Combine (Environment.CurrentDirectory, "TempBasePath");
      string localSampleTypesPath = Path.Combine (Environment.CurrentDirectory, "SampleTypes.dll");
      string localGeneratedPath = Path.Combine (Environment.CurrentDirectory, "Remotion.Mixins.Generated.Unsigned.dll");
      string remoteSampleTypesPath = Path.Combine (basePath, "SampleTypes.dll");

      if (Directory.Exists (basePath))
        Directory.Delete (basePath, true);

      Directory.CreateDirectory (basePath);

      if (File.Exists (localSampleTypesPath))
        File.Delete (localSampleTypesPath);

      if (File.Exists (localGeneratedPath))
        File.Delete (localGeneratedPath);

      string mixinDllPath = Path.Combine (basePath, typeof (ExtendsAttribute).Assembly.ManifestModule.Name);
      File.Copy (typeof (ExtendsAttribute).Assembly.Location, mixinDllPath);

      var compiler = new AssemblyCompiler ("Mixins\\MixerTool\\SampleAssembly", remoteSampleTypesPath, mixinDllPath);
      compiler.CompileInSeparateAppDomain ();

      _parameters.BaseDirectory = basePath;

      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      var runner = new MixerRunner (_parameters);
      runner.Run ();
      Assert.IsTrue (File.Exists (UnsignedAssemblyPath));

      File.Move (remoteSampleTypesPath, localSampleTypesPath);
      File.Move (UnsignedAssemblyPath, localGeneratedPath);

      AppDomainRunner.Run (
          delegate (object[] args)
          {
            var path = (string) args[0];
            Assembly assembly = Assembly.LoadFile (path);
            Assert.AreEqual (2, assembly.GetTypes().Length); // concrete type + base call proxy
            Type generatedType = GetFirstMixedType (assembly);
            Assert.AreEqual ("BaseType", generatedType.BaseType.Name);
          },  localGeneratedPath);

      File.Delete (localSampleTypesPath);
      File.Delete (localGeneratedPath);
    }

    [Test]
    public void RunWithKeepTypeNames ()
    {
      _parameters.KeepTypeNames = true;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          var runner = new MixerRunner (_parameters);
          runner.Run();
        }
      }

      AppDomainRunner.Run (
          delegate
          {
            Assembly assembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Type generatedType = GetFirstMixedType (assembly);
            Assert.IsTrue (generatedType.Namespace.EndsWith ("MixedTypes"));
          });
    }

    private void ResetGeneratedFiles ()
    {
      if (File.Exists (UnsignedAssemblyPath))
        FileUtility.DeleteAndWaitForCompletion (UnsignedAssemblyPath);
      if (File.Exists (SignedAssemblyPath))
        FileUtility.DeleteAndWaitForCompletion (SignedAssemblyPath);
    }

    private Type GetFirstMixedType (Assembly assembly)
    {
      foreach (Type t in assembly.GetTypes ())
      {
        if (t.IsDefined (typeof (ConcreteMixedTypeAttribute), false))
          return t;
      }
      return null;
    }
  }
}
