using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Definitions.DependencySorting.SampleTypes
{
  public class MixinWithBaseCallDependency2OverridingM1 : Mixin<object, IBaseCallDependency2>
  {
    [OverrideTarget]
    public void M1 ()
    {
    }
  }
}