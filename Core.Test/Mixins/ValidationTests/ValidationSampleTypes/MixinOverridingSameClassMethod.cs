using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class MixinOverridingSameClassMethod
  {
    [OverrideTarget]
    public virtual string AbstractMethod(int i)
    {
      return "MixinOverridingSameClassMethod.AbstractMethod-" + i;
    }
  }
}
