using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [Uses (typeof (MixinAddingUnidirectionalRelation2))]
  [DBTable ("MixedDomains_TargetWithTwoUnidirectionalMixins")]
  [TestDomain]
  public class TargetClassWithTwoUnidirectionalMixins : SimpleDomainObject<TargetClassWithTwoUnidirectionalMixins>
  {
  }
}