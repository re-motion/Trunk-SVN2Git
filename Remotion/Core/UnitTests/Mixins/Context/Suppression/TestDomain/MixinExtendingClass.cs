using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Context.Suppression.TestDomain
{
  [Extends (typeof (ClassWithMixins))]
  [IgnoreForMixinConfiguration]
  public class MixinExtendingClass { }
}