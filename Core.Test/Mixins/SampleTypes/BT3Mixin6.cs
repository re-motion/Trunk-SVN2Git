using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT3Mixin6ThisDependencies : IBaseType31, IBaseType32, IBaseType33, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6BaseDependencies : IBaseType34, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6 { }

  [Extends(typeof(BaseType3))]
  [Serializable]
  public class BT3Mixin6<TThis, TBase> : Mixin<TThis, TBase>, IBT3Mixin6
      where TThis : class, IBT3Mixin6ThisDependencies
      where TBase : class, IBT3Mixin6BaseDependencies
  {
  }
}
