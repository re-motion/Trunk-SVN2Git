using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin1
  {
    string One<T> (T t);
    string BT7Mixin1Specific ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin1 : Mixin<BaseType7, IBaseType7> , IBT7Mixin1
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin1.One(" + t + ")-" + Base.One(t);
    }

    public string BT7Mixin1Specific ()
    {
      return "BT7Mixin1.BT7Mixin1Specific-" + Base.Three() + "-" + This.Three();
    }
  }
}
