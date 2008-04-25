using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinRequiringAllMembersRequirements
  {
    void Method ();
    int Property { get; set; }
    event Func<string> Event;
  }

  public class MixinRequiringAllMembersFace
      : Mixin<IMixinRequiringAllMembersRequirements>
  {
    public int PropertyViaThis
    {
      get { return This.Property; }
    }
  }
}
