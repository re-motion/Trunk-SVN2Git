using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.UnitTests.Mixins.CodeGeneration.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    public class ClassOverridingMixinMethod
    {
      [OverrideMixin]
      public new string ToString ()
      {
        return "Overridden!";
      }
    }

    public class SimpleMixin : Mixin<object>
    {
    }

    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (Mixin<object>));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMethod> ().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod)).With ();
        Assert.AreEqual ("Overridden!", Mixin.Get (generatedType, instance).ToString ());
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      typeEmitter.CreateMethod ("ToString", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturning (new ConstReference ("Generated _and_ overridden").ToExpression ())
          .AddCustomAttribute (new CustomAttributeBuilder (typeof (OverrideMixinAttribute).GetConstructor (Type.EmptyTypes), new object[0]));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (SimpleMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType).With ();
        Assert.AreEqual ("Generated _and_ overridden", Mixin.Get<SimpleMixin> (instance).ToString ());
      }
    }
  }
}