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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.TypeDiscovery;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  [TestFixture]
  public class Mixer_IntegrationTest
  {
    private string _assemblyOutputDirectory;

    [SetUp]
    public void SetUp ()
    {
      _assemblyOutputDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Mixer_IntegrationTest");
    }

    [Test]
    public void SavesMixedTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            using (MixinConfiguration.BuildNew().ForClass<BaseType1>().AddMixins (typeof (BT1Mixin1)).EnterScope())
            {
              Mixer mixer = Mixer.Create ("Signed", "Unsigned", GuidNameProvider.Instance, _assemblyOutputDirectory);
              mixer.PrepareOutputDirectory();
              mixer.Execute (MixinConfiguration.ActiveConfiguration);

              Assembly theAssembly = Assembly.LoadFile (mixer.ConcreteTypeBuilderFactory.GetUnsignedModulePath (_assemblyOutputDirectory));
              var types = theAssembly.GetTypes();

              var concreteType = types.Where (t => t.BaseType == typeof (BaseType1)).SingleOrDefault();
              Assert.That (concreteType, Is.Not.Null);
              Assert.That (
                  MixinReflector.GetClassContextFromConcreteType (concreteType),
                  Is.EqualTo (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1))));

              object instance = Activator.CreateInstance (concreteType);
              Assert.That (Mixin.Get<BT1Mixin1> (instance), Is.Not.Null);

              ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (theAssembly);
              Type concreteTypeFromFactory = TypeFactory.GetConcreteType (typeof (BaseType1));
              Assert.That (concreteTypeFromFactory, Is.SameAs (concreteType));

              Assert.That (theAssembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false), Is.True);
            }
          });
    }
  }
}