using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  [Uses (typeof (MixinBase))]
  public abstract class TargetClassBase : DomainObject
  {
    public abstract int P0 { get; }
  }
}
