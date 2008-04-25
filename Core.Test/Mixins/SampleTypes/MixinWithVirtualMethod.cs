using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinWithVirtualMethod : Mixin<object>
  {
    public virtual string VirtualMethod ()
    {
      return "MixinWithVirtualMethod.VirtualMethod";
    }
  }
}