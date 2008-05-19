using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class MixedMixinTest
  {
    [Test]
    public void DoubleMixinOverrides_CreateMixinInstance ()
    {
      MixinMixingClass instance = ObjectFactory.Create<MixinMixingClass> ().With ();
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (instance));
    }

    [Test]
    public void DoubleMixinOverrides_CreateClassInstance ()
    {
      ClassWithMixedMixin instance = ObjectFactory.Create<ClassWithMixedMixin> ().With();
      Assert.IsNotNull (Mixin.Get<MixinMixingClass> (instance));
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (Mixin.Get<MixinMixingClass> (instance)));

      Assert.AreEqual ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)", instance.StringMethod (3));
    }
  }
}
