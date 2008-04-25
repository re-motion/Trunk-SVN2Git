using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (MixinRequiringAllMembersFace))]
  public class ClassFulfillingAllMemberRequirementsExplicitly : IMixinRequiringAllMembersRequirements
  {
    void IMixinRequiringAllMembersRequirements.Method ()
    {
    }

    int IMixinRequiringAllMembersRequirements.Property
    {
      get { return 37; }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    event Func<string> IMixinRequiringAllMembersRequirements.Event
    {
      add { throw new Exception ("The method or operation is not implemented."); }
      remove { throw new Exception ("The method or operation is not implemented."); }
    }
  }
}