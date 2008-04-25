using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT5Mixin3
  {
    void Foo ();
  }

  public class BT5Mixin3 : Mixin<IBT5Mixin3, IBT5Mixin3>, IBT5Mixin3
  {
    public void Foo ()
    {
    }
  }
}
