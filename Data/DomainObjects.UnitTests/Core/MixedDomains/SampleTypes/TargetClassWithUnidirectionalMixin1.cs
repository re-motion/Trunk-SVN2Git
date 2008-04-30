using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [DBTable ("MixedDomains_TargetWithUnidirectionalMixin1")]
  [TestDomain]
  public class TargetClassWithUnidirectionalMixin1 : SimpleDomainObject<TargetClassWithUnidirectionalMixin1>
  {
    
  }
}