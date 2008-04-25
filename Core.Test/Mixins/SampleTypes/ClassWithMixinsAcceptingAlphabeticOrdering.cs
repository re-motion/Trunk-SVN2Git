using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (MixinAcceptingAlphabeticOrdering1))]
  [Uses (typeof (MixinAcceptingAlphabeticOrdering2))]
  public class ClassWithMixinsAcceptingAlphabeticOrdering
  {
    public override string ToString ()
    {
      return "ClassWithMixinsAcceptingAlphabeticOrdering.ToString";
    }
  }
}