using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [CopyCustomAttributes (typeof (MixinIndirectlyAddingNonInheritedAttributeFromSelf))]
  [NonInheritedAttribute]
  public class MixinIndirectlyAddingNonInheritedAttributeFromSelf : Mixin<object>
  {
  }
}
