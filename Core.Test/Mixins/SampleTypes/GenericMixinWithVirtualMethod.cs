using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class GenericMixinWithVirtualMethod<T> : Mixin<T>
      where T : class
  {
    public virtual string VirtualMethod ()
    {
      return "GenericMixinWithVirtualMethod.VirtualMethod";
    }
  }
}