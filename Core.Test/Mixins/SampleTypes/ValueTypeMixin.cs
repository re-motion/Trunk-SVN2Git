using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public struct ValueTypeMixin
  {
    [OverrideTarget]
    public string VirtualMethod ()
    {
      return "ValueTypeMixin.VirtualMethod";
    }
  }
}