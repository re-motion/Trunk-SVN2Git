using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinWithVirtualMethod2 : Mixin<object>
  {
    public virtual string VirtualMethod ()
    {
      return "MixinWithVirtualMethod2.VirtualMethod";
    }
  }
}