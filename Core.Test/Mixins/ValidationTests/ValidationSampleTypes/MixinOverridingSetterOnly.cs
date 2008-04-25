using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class MixinOverridingSetterOnly
  {
    [OverrideTarget]
    public virtual int Property
    {
      set { }
    }
  }
}