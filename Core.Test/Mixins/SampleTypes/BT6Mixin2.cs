using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT6Mixin2
  {
    string Mixin2Method ();
  }

  [Extends (typeof (BaseType6))]
  public class BT6Mixin2 : Mixin<IBaseType6>, IBT6Mixin2
  {
    public string Mixin2Method ()
    {
      return "BT6Mixin2.Mixin2Method";
    }
  }

  [CompleteInterface (typeof (BaseType6))]
  public interface ICBT6Mixin2 : IBT6Mixin2, IBaseType6
  {
  }
}
