using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.MixedTypeCodeGeneration
{
  [TestFixture]
  public class IntroductionTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntroducedInterfacesAreImplementedViaDelegation ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IBT1Mixin1 bt1AsMixedIface = bt1 as IBT1Mixin1;
      Assert.IsNotNull (bt1AsMixedIface);
      Assert.AreEqual ("BT1Mixin1.IntroducedMethod", bt1AsMixedIface.IntroducedMethod ());
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithExplicitImplementation)).With ();
      IExplicit explicito = bt1 as IExplicit;
      Assert.IsNotNull (explicito);
      Assert.AreEqual ("XXX", explicito.Explicit ());
    }

    [Test]
    public void MixinCanIntroduceGenericInterface ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>)).With ();
      IGeneric<BaseType1> generic = bt1 as IGeneric<BaseType1>;
      Assert.IsNotNull (generic);
      Assert.AreEqual ("Generic", generic.Generic (bt1));
    }

    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingInheritedInterface)).With ();
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII1) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII2) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII2) bt1).Method2 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method3", ((IMixinIII3) bt1).Method3 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method4", ((IMixinIII4) bt1).Method4 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII4) bt1).Method2 ());
    }

    [Test]
    public void MixinImplementingFullPropertiesWithPartialIntroduction ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingFullPropertiesWithPartialIntroduction)).EnterScope())
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
        MethodInfo[] allMethods = bt1.GetType ().GetMethods (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        string[] allMethodNames = Array.ConvertAll<MethodInfo, string> (allMethods, delegate (MethodInfo mi) { return mi.Name; });
        Assert.That (allMethodNames, List.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.get_Prop1"));
        Assert.That (allMethodNames, List.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.set_Prop2"));

        Assert.That (allMethodNames, List.Not.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.set_Prop1"));
        Assert.That (allMethodNames, List.Not.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.get_Prop2"));
      }
    }

    [Test]
    public void ExplicitlySuppressedInterfaceIntroduction ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (MixinSuppressingSimpleInterface)).With();
      Assert.IsFalse (o is ISimpleInterface);
      Assert.IsTrue (Mixin.Get<MixinSuppressingSimpleInterface> (o) is ISimpleInterface);
    }

    [Test]
    public void ImplicitlySuppressedInterfaceIntroduction ()
    {
      ClassImplementingSimpleInterface o = CreateMixedObject<ClassImplementingSimpleInterface> (typeof (MixinImplementingSimpleInterface)).With();
      Assert.IsTrue (o is ISimpleInterface);
      Assert.AreEqual ("ClassImplementingSimpleInterface.Method", o.Method ());
    }

    [Test]
    public void MultipleSimilarInterfaces ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (List<>)).With ();
      Assert.IsTrue (o is IList<NullTarget>);
      Assert.IsTrue (o is ICollection<NullTarget>);
      Assert.IsTrue (o is IEnumerable<NullTarget>);
      Assert.IsTrue (o is IList);
      Assert.IsTrue (o is ICollection);
      Assert.IsTrue (o is IEnumerable);
    }
  }
}