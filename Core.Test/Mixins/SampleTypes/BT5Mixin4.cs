using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  // no attributes
  public class BT5Mixin4
  {
    [OverrideTarget]
    public string Property
    {
      set { }
    }
  }
}