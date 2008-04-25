using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class ClassWithNonPublicOverrider
  {
    [OverrideMixin]
    private string AbstractMethod (int i)
    {
      return null;
    }

    [OverrideMixin]
    private string AbstractProperty
    {
      get { return null; }
    }

    [OverrideMixin]
    private event Func<string> AbstractEvent
    {
      add { }
      remove { }
    }
  }
}