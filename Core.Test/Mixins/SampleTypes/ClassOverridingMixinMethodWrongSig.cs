using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class ClassOverridingMixinMethodWrongSig
  {
    [OverrideMixin]
    public string AbstractMethod(string s)
    {
      return "ClassOverridingMixinMethod.AbstractMethod" + s;
    }
  }
}
