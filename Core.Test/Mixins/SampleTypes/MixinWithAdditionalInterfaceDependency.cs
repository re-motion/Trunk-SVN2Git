using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (TargetClassWithAdditionalDependencies), AdditionalDependencies = new Type[] { typeof ( IMixinWithAdditionalClassDependency ) })]
  public class MixinWithAdditionalInterfaceDependency : Mixin<object, ITargetClassWithAdditionalDependencies>
  {
    [OverrideTarget]
    public string GetString ()
    {
      return "MixinWithAdditionalInterfaceDependency-" + Base.GetString ();
    }
  }
}