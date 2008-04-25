using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "MixinImplementingSimpleInterface.Method";
    }
  }
}