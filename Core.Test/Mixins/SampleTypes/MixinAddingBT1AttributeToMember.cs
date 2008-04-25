using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [AcceptsAlphabeticOrdering]
  public class MixinAddingBT1AttributeToMember
  {
    [OverrideTarget]
    [BT1]
    public string VirtualMethod ()
    {
      return "";
    }
  }

  [AcceptsAlphabeticOrdering]
  public class MixinAddingBT1AttributeToMember2 : MixinAddingBT1AttributeToMember
  {
  }
}