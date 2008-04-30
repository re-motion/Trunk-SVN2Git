using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (DerivedMixinNotOnBase))]
  public class TargetClassC : DomainObject
  {
    
  }
}