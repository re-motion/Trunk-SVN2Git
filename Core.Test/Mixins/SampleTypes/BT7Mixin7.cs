using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT7Mixin7 { }

  // no attribute
  public class BT7Mixin7 : Mixin<object, IBT7Mixin2>, IBT7Mixin7
  {
  }
}
