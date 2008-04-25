using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseClassForDerivedClassIgnoredByMixin))]
  [IgnoresClass (typeof (DerivedClassIgnoredByMixins))]
  [IgnoresClass (typeof (GenericClassForMixinIgnoringDerivedClass<>))]
  [IgnoresClass (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>))]
  public class MixinIgnoringDerivedClass
  {
  }
}