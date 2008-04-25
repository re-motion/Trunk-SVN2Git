using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class ClassWithMixedMixin
  {
    public virtual string StringMethod (int i)
    {
      return "ClassWithMixedMixin.StringMethod (" + i + ")";
    }
  }
}