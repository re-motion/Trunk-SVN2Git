using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes
{
  public class MixinWithProtectedOverriderWithoutMixinBase
  {
    [OverrideTarget]
    protected string VirtualMethod ()
    {
      return "MixinWithProtectedOverriderWithoutMixinBase.VirtualMethod";
    }
  }
}
