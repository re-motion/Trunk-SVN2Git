using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  [Instantiable]
  [Uses (typeof (MixinB))]
  [Uses (typeof (MixinC))]
  [Uses (typeof (MixinE))]
  public abstract class TargetClassB : TargetClassA
  {
    public abstract int P3 { get; }
    public abstract int P4 { get; set; }
  }
}
