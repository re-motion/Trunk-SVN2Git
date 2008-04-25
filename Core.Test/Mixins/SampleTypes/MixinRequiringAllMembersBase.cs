using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinRequiringAllMembersBase
      : Mixin<object, IMixinRequiringAllMembersRequirements>
  {
    public int PropertyViaBase
    {
      get { return Base.Property; }
    }
  }
}
