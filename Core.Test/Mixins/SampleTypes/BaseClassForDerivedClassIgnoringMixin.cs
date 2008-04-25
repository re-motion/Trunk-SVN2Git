using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (NullMixin))]
  [Uses (typeof (DerivedNullMixin))]
  [Uses (typeof (DerivedDerivedNullMixin))]
  [Uses (typeof (GenericMixinWithVirtualMethod<>))]
  [Uses (typeof (GenericMixinWithVirtualMethod2<object>))]
  public class BaseClassForDerivedClassIgnoringMixin
  {
  }
}