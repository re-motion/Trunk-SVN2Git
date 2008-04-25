using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT6Mixin1
  {
    string Mixin1Method ();
  }

  [Extends (typeof (BaseType6))]
  public class BT6Mixin1 : Mixin<IBaseType6>, IBT6Mixin1
  {
    public string Mixin1Method ()
    {
      return "BT6Mixin1.Mixin1Method";
    }
  }

  [CompleteInterface (typeof (BaseType6))]
  public interface ICBT6Mixin1 : IBT6Mixin1, IBaseType6
  {
  }
}
