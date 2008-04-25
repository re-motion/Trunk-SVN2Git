using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [CopyCustomAttributes (typeof (AttributeSource))]
  public class MixinIndirectlyAddingNonInheritedAttribute : Mixin<object>
  {
    [NonInheritedAttribute]
    public class AttributeSource
    {
    }
  }
}
