using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public class ClassOverridingSpecificMixinMember
  {
    [OverrideMixin (typeof (MixinWithVirtualMethod))]
    public virtual string VirtualMethod ()
    {
      return "ClassOverridingSpecificMixinMember.ToString";
    }
  }
}
