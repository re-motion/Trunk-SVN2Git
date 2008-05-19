using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class DependencyTest : CodeGenerationBaseTest
  {
    [Test]
    public void CircularThisDependenciesWork ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithCircularThisDependency1), typeof (MixinWithCircularThisDependency2)).EnterScope())
      {
        object o = ObjectFactory.Create<NullTarget> ().With ();
        ICircular2 c1 = (ICircular2) o;
        Assert.AreEqual ("MixinWithCircularThisDependency2.Circular12-MixinWithCircularThisDependency1.Circular1-"
            + "MixinWithCircularThisDependency2.Circular2", c1.Circular12 ());
      }
    }

    [Test]
    public void ThisCallToClassImplementingInternalInterface ()
    {
      ClassImplementingInternalInterface ciii = ObjectFactory.Create<ClassImplementingInternalInterface> ().With ();
      MixinWithClassFaceImplementingInternalInterface mixin = Mixin.Get<MixinWithClassFaceImplementingInternalInterface> (ciii);
      Assert.AreEqual ("ClassImplementingInternalInterface.Foo", mixin.GetStringViaThis ());
    }

    [Test]
    public void ThisCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> ().With ();
      MixinWithIndirectRequirements mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.AreEqual ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
          + "ClassImplementingIndirectRequirements.Method3", mixin.GetStuffViaThis ());
    }
  }
}