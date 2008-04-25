using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class ClassImplementingSimpleInterface : ISimpleInterface
  {
    public string Method ()
    {
      return "ClassImplementingSimpleInterface.Method";
    }
  }
}