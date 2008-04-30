using System;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Uses (typeof (MixinAddingBidirectionalRelationTwice))]
  public class TargetClass1ForMixinAddingBidirectionalRelationTwice : SimpleDomainObject<TargetClass1ForMixinAddingBidirectionalRelationTwice>
  {
  }
}