using System;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class MixinWithNonVirtualMethodToBeOverridden
  {
    public string AbstractMethod(int i)
    {
      return "This method is not really abstract.";
    }
  }
}
