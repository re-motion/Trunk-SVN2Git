using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [CopyCustomAttributes (typeof (MixinIndirectlyAddingInheritedAttributeFromSelf))]
  [AttributeWithParameters (0)]
  public class MixinIndirectlyAddingInheritedAttributeFromSelf : Mixin<object>
  {
  }
}
