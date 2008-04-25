using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [NonIntroduced (typeof (ISimpleInterface))]
  public class MixinSuppressingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinSuppressingSimpleInterface.Method";
    }
  }
}