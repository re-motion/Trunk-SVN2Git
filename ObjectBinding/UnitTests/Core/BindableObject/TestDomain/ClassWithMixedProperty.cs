using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  [Uses (typeof (MixinAddingProperty))]
  public class ClassWithMixedProperty
  {
  }
}
