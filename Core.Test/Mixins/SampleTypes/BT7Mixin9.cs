using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin9 { }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin9 : Mixin<object, IBaseType7>, IBT7Mixin9
  {
    [OverrideTarget]
    public string Five()
    {
      return "BT7Mixin9.Five-" + Base.Five();
    }
  }
}
