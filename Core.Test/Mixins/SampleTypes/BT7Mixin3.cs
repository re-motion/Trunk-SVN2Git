using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin3
  {
    string One<T> (T t);
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin3 : Mixin<object, IBT7Mixin1>, IBT7Mixin3
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin3.One(" + t + ")-" + Base.BT7Mixin1Specific() + "-" + Base.One(t);
    }
  }
}
