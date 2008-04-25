using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Definitions.DependencySorting.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinOverridingM2
  {
    [OverrideTarget]
    public void M2 ()
    {
    }
  }
}