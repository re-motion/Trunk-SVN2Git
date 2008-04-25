using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [DBTable]
  [Uses (typeof (DerivedMixinAddingPersistentProperties))]
  public class TargetClassForDerivedPersistentMixin : SimpleDomainObject<TargetClassForDerivedPersistentMixin>
  {
  }
}