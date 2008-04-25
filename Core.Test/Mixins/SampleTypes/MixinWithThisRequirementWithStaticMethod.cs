using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinWithThisRequirementWithStaticMethod : Mixin<ClassWithStaticMethod>
  {
  }

  [Uses (typeof (MixinWithThisRequirementWithStaticMethod))]
  public class ClassWithStaticMethod
  {
    public static void StaticMethod ()
    {
    }

    public void InstanceMethod ()
    {
    }

    public void VirtualMethod ()
    {
    }
  }
}
