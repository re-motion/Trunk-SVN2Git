using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT6Mixin3
  {
    string Mixin3Method ();
  }

  public interface IBT6Mixin3Constraints : ICBT6Mixin1, ICBT6Mixin2 {}

  [Extends (typeof (BaseType6))]
  public class BT6Mixin3<This> : Mixin<This>, IBT6Mixin3
      where This : class, IBT6Mixin3Constraints
  {
    public string Mixin3Method ()
    {
      return "BT6Mixin3.Mixin3Method";
    }
  }

  [CompleteInterface (typeof (BaseType6))]
  public interface ICBT6Mixin3 : IBT6Mixin1, IBT6Mixin2, IBaseType6
  {
  }
}
