using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [DBTable ("MixedDomains_TargetWithUnidirectionalMixin2")]
  [TestDomain]
  public class TargetClassWithUnidirectionalMixin2 : SimpleDomainObject<TargetClassWithUnidirectionalMixin2>
  {
    
  }
}