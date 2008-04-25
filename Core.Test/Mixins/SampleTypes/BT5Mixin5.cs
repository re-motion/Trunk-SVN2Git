using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  // no attributes
  public class BT5Mixin5
  {
    [OverrideTarget]
    public event EventHandler Event;
  }
}