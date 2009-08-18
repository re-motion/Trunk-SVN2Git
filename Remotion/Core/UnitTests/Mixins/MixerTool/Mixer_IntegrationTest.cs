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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  [TestFixture]
  public class Mixer_IntegrationTest : MixerToolBaseTest
  {
    [Test]
    public void SavesMixedTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);

            using (MixinConfiguration.BuildNew ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1)).EnterScope ())
            {
              mixer.Execute ();

              Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
              Assert.AreEqual (2, theAssembly.GetTypes ().Length);
              Type generatedType = GetFirstMixedType (theAssembly);

              Assert.IsNotNull (MixinReflector.GetClassContextFromConcreteType (generatedType));
              Assert.AreEqual (
                  MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType1)),
                  MixinReflector.GetClassContextFromConcreteType (generatedType));

              object instance = Activator.CreateInstance (generatedType);
              Assert.IsTrue (generatedType.IsInstanceOfType (instance));
              Assert.IsNotNull (Mixin.Get<BT1Mixin1> (instance));
            }
          });
    }

    [Test]
    public void AssemblyGeneratedByMixerToolCanBeLoadedIntoTypeBuilder ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew ().EnterScope ())
            {
              using (MixinConfiguration.BuildFromActive ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1)).EnterScope ())
              {
                mixer.Execute ();
              }
            }
          });

      AppDomainRunner.Run (
          delegate
          {
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (theAssembly);
            using (MixinConfiguration.BuildNew ().EnterScope ())
            {
              using (MixinConfiguration.BuildFromActive ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1)).EnterScope ())
              {
                Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
                Assert.Contains (generatedType, theAssembly.GetTypes ());
              }
            }
          });
    }

    [Test]
    public void AssemblyGeneratedByMixerTool_HasNonApplicationAssemblyAttribute ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1)).EnterScope ())
            {
              mixer.Execute ();
            }
          });

      AppDomainRunner.Run (
          delegate
          {
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Assert.IsTrue (theAssembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
          });
    }
  }
}
