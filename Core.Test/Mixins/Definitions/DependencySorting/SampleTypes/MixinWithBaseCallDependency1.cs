using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Definitions.DependencySorting.SampleTypes
{
  public class MixinWithBaseCallDependency1 : Mixin<object, IBaseCallDependency1>, IBaseCallDependency2
  {
    
  }
}