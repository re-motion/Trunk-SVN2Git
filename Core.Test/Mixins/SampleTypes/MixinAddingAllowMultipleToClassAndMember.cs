using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Multi]
  [AcceptsAlphabeticOrdering]
  public class MixinAddingAllowMultipleToClassAndMember
  {
    [OverrideTarget]
    [Multi]
    public void Foo ()
    {
    }
  }

  [Multi]
  [AcceptsAlphabeticOrdering]
  public class MixinAddingAllowMultipleToClassAndMember2 : MixinAddingAllowMultipleToClassAndMember
  {
  }
}