using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin4 { }

  // no attribute
  public class BT7Mixin4 : Mixin<object, IBaseType7>, IBT7Mixin4
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin4.One(" + t + ")-" + Base.One(t);
    }
  }
}
