using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [IgnoresMixin (typeof (DerivedNullMixin))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod<>))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod2<>))]
  public class DerivedClassIgnoringMixin : BaseClassForDerivedClassIgnoringMixin
  {
  }
}