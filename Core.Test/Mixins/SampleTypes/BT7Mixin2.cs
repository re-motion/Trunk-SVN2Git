using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin2Reqs : IBT7Mixin3, IBaseType7
  {
  }

  public interface IBT7Mixin2
  {
    string One<T> (T t);
    string Two ();
    string Three ();
    string Four ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin2 : Mixin<BaseType7, IBT7Mixin2Reqs>, IBT7Mixin2
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin2.One(" + t + ")-" + ((IBaseType7) Base).One (t) + "-" + ((IBT7Mixin3) Base).One (t) + "-" + Base.Two() + "-" + This.Two();
    }

    [OverrideTarget]
    public virtual string Two()
    {
      return "BT7Mixin2.Two";
    }

    [OverrideTarget]
    public virtual string Three ()
    {
      return "BT7Mixin2.Three-" + Base.Three ();
    }

    [OverrideTarget]
    public virtual string Four ()
    {
      return "BT7Mixin2.Four-" + Base.Four() + "-" + Base.NotOverridden();
    }
  }
}
