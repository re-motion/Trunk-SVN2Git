using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (MixinRequiringAllMembersFace))]
  [Uses (typeof (MixinRequiringAllMembersBase))]
  public class ClassFulfillingAllMemberRequirementsDuck
  {
    public void Method ()
    {
      throw new NotImplementedException ();
    }

    public int Property
    {
      get { return 42; }
      set { throw new NotImplementedException (); }
    }

    public event Func<string> Event;
  }
}