// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [TestFixture]
  public class MixerRunnerTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public void SetUp ()
    {
      _parameters = new MixerParameters ();
    }

    [Test]
    public void ParameterDefaults ()
    {
      Assert.That (_parameters.AssemblyOutputDirectory, Is.EqualTo (Environment.CurrentDirectory));
      Assert.That (_parameters.BaseDirectory, Is.EqualTo (Environment.CurrentDirectory));
      Assert.That (_parameters.ConfigFile, Is.EqualTo (""));
      Assert.That (_parameters.SignedAssemblyName, Is.EqualTo ("Remotion.Mixins.Persistent.Signed"));
      Assert.That (_parameters.UnsignedAssemblyName, Is.EqualTo ("Remotion.Mixins.Persistent.Unsigned"));
      Assert.That (_parameters.KeepTypeNames, Is.EqualTo (false));
    }

    [Test]
    public void CreateAppDomainSetup_Default ()
    {
      var setup = MixerRunner.CreateAppDomainSetup (_parameters);

      Assert.That (setup.ApplicationName, Is.EqualTo ("Mixer"));
      Assert.That (setup.ApplicationBase, Is.EqualTo (_parameters.BaseDirectory));
    }

    [Test]
    public void CreateMixer_Default ()
    {
      var runner = new MixerRunner (_parameters);
      var mixer = (Mixer) PrivateInvoke.InvokeNonPublicMethod (runner, "CreateMixer");

      Assert.That (((ConcreteTypeBuilderFactory) mixer.ConcreteTypeBuilderFactory).SignedAssemblyName, Is.EqualTo (_parameters.SignedAssemblyName));
      Assert.That (((ConcreteTypeBuilderFactory) mixer.ConcreteTypeBuilderFactory).UnsignedAssemblyName, Is.EqualTo (_parameters.UnsignedAssemblyName));
      Assert.That (((ConcreteTypeBuilderFactory) mixer.ConcreteTypeBuilderFactory).TypeNameProvider, Is.SameAs (GuidNameProvider.Instance));
      Assert.That (mixer.AssemblyOutputDirectory, Is.EqualTo (_parameters.AssemblyOutputDirectory));
    }

    [Test]
    public void CreateMixer_KeepTypeNames ()
    {
      var runner = new MixerRunner (_parameters);
      _parameters.KeepTypeNames = true;
      var mixer = (Mixer) PrivateInvoke.InvokeNonPublicMethod (runner, "CreateMixer");

      Assert.That (((ConcreteTypeBuilderFactory) mixer.ConcreteTypeBuilderFactory).TypeNameProvider, Is.SameAs (NamespaceChangingNameProvider.Instance));
    }

    [Test]
    public void RunDefault ()
    {
      _parameters.AssemblyOutputDirectory = "MixerRunnerTest";
      var unsignedAssemblyPath = Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.UnsignedAssemblyName + ".dll");
      
      try
      {
        Assert.That (Directory.Exists (_parameters.AssemblyOutputDirectory), Is.False);
        Assert.That (File.Exists (unsignedAssemblyPath), Is.False);

        var runner = new MixerRunner (_parameters);
        runner.Run ();

        Assert.That (Directory.Exists (_parameters.AssemblyOutputDirectory), Is.True);
        Assert.That (File.Exists (unsignedAssemblyPath), Is.True);
      }
      finally
      {
        Directory.Delete (_parameters.AssemblyOutputDirectory, true);
      }
    }
  }
}
