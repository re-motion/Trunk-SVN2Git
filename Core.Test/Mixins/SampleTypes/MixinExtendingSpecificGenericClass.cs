using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends(typeof (GenericClassExtendedByMixin<int>))]
  public class MixinExtendingSpecificGenericClass
  {
  }
}