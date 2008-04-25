using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [CopyCustomAttributes (typeof (AttributeSource))]
  public class MixinIndirectlyAddingAttribute : Mixin<object>
  {
    [AttributeWithParameters (1, "bla", Property = 4, Field = 5)]
    public class AttributeSource
    {
      [AttributeWithParameters (4)]
      public void AttributeSourceMethod ()
      {
      }
    }

    [OverrideTarget]
    [CopyCustomAttributes (typeof (AttributeSource), "AttributeSourceMethod")]
    public new string ToString ()
    {
      return "";
    }
  }
}
