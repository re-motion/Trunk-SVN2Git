using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin8 { }

  // no attributes
  public class BT7Mixin8 : Mixin<object, IBaseType7>, IBT7Mixin8
  {
    [OverrideTarget]
    public string Five()
    {
      return "BT7Mixin8.Five-" + Base.Five();
    }
  }
}
