using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTest : CodeGenerationBaseTest
  {
    [Test]
    public void SignedBaseClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (object)).Clear().EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BaseType1)).Clear().EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassSignedMixinGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (List<int>)).Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (List<int>), GenerationPolicy.ForceGeneration);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassSignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (NullTarget), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinWithOverride ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingToString> ().Clear().AddMixins (typeof (MixinOverridingToString)).EnterScope())
      {
        object instance = ObjectFactory.Create<ClassOverridingToString>().With();
        Assert.AreEqual ("Overridden: ClassOverridingToString", instance.ToString());
      }
    }
  }
}