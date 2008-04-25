using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTest
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII4)));
    }


  }
}