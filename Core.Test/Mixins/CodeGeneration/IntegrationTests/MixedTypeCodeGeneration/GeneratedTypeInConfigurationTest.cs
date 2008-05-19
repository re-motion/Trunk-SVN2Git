using System;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (NullTarget)).With ();
        Assert.IsNotNull (Mixin.Get (generatedType, instance));
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType).With ();
        Assert.IsNotNull (Mixin.Get (typeof (NullMixin), instance));
      }
    }
  }
}