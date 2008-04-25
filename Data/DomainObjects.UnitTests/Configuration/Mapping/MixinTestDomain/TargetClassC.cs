using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (DerivedMixinNotOnBase))]
  public class TargetClassC : DomainObject
  {
    
  }
}