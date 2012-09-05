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
using System.Reflection;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTools;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.TypeDiscovery;
using System.Linq;

namespace Remotion.Mixins.UnitTests.Core.MixerTools
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

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists (_assemblyOutputDirectory))
        Directory.Delete (_assemblyOutputDirectory, true);
    }

    [Test]
    public void SavesMixedTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            using (MixinConfiguration.BuildNew()
                .ForClass<BaseType1>().AddMixins (typeof (BT1Mixin1))
                .ForClass<Page> ().AddMixin (typeof (NullMixin))
                .EnterScope())
            {
              Mixer mixer = Mixer.Create ("Signed", "Unsigned", _assemblyOutputDirectory, new GuidNameProvider());
              mixer.PrepareOutputDirectory();
              mixer.Execute (MixinConfiguration.ActiveConfiguration);

              Assembly theAssembly = Assembly.LoadFile (mixer.ConcreteTypeBuilderFactory.GetUnsignedModulePath (_assemblyOutputDirectory));
              var types = theAssembly.GetTypes();

              var concreteType = types.Where (t => t.BaseType == typeof (BaseType1)).SingleOrDefault();
              Assert.That (concreteType, Is.Not.Null);
              Assert.That (
                  MixinTypeUtility.GetClassContextForConcreteType (concreteType),
                  Is.EqualTo (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1))));

              object instance = Activator.CreateInstance (concreteType);
              Assert.That (Mixin.Get<BT1Mixin1> (instance), Is.Not.Null);

              var concreteTypeFromSystemAssembly = types.Where (t => t.BaseType == typeof (Page)).SingleOrDefault ();
              Assert.That (concreteTypeFromSystemAssembly, Is.Not.Null);
              
              ConcreteTypeBuilder.Current.LoadConcreteTypes (theAssembly);
              Type concreteTypeFromFactory = TypeFactory.GetConcreteType (typeof (BaseType1));
              Assert.That (concreteTypeFromFactory, Is.SameAs (concreteType));

              Assert.That (theAssembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false), Is.True);
            }
          });
    }
  }
}
