using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class MixinWithPrivateCtorAndVirtualMethod : Mixin<object>
  {
    public static MixinWithPrivateCtorAndVirtualMethod Create ()
    {
      return new MixinWithPrivateCtorAndVirtualMethod ();
    }

    private MixinWithPrivateCtorAndVirtualMethod ()
    {
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinWithPrivateCtorAndVirtualMethod.OverriddenMethod(" + i + ")";
    }
  }
}
