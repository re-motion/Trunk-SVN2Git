using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class GenericMixinWithVirtualMethod2<T> : Mixin<T>
      where T : class
  {
    public virtual string VirtualMethod ()
    {
      return "GenericMixinWithVirtualMethod2.VirtualMethod";
    }
  }
}