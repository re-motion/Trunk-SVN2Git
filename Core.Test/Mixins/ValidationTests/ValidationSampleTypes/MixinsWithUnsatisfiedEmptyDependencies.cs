using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public interface IEmptyInterface
  { }

  public class MixinWithUnsatisfiedEmptyThisDependency : Mixin<IEmptyInterface>
  {
  }

  public class MixinWithUnsatisfiedEmptyBaseDependency : Mixin<object, IEmptyInterface>
  {
  }

  public interface IEmptyAggregateInterface : IEmptyInterface
  {
  }

  public class MixinWithUnsatisfiedEmptyAggregateThisDependency : Mixin<IEmptyAggregateInterface>
  {
  }

  public class MixinWithUnsatisfiedEmptyAggregateBaseDependency : Mixin<object, IEmptyAggregateInterface>
  {
  }
}
