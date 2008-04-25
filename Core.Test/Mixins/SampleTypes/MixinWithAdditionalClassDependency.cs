using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IMixinWithAdditionalClassDependency
  {
    string GetString ();
  }

  [Extends (typeof (TargetClassWithAdditionalDependencies), AdditionalDependencies = new Type[] { typeof ( MixinWithNoAdditionalDependency ) })]
  public class MixinWithAdditionalClassDependency : Mixin<object, ITargetClassWithAdditionalDependencies>, IMixinWithAdditionalClassDependency 
  {
    [OverrideTarget]
    public string GetString ()
    {
      return "MixinWithAdditionalClassDependency-" + Base.GetString ();
    }
  }
}