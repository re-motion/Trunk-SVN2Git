using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixin]
    public virtual event EventHandler Event;
  }
}
