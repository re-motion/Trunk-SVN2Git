using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;
using Remotion.Reflection;

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
              mixer.Execute (true);

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
                mixer.Execute (true);
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
              mixer.Execute (true);
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