using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseType7))]
  public class BT7Mixin0 : Mixin<object, IBT7Mixin2>
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin0.One(" + t + ")-" + Base.One(t);
    }
  }
}
