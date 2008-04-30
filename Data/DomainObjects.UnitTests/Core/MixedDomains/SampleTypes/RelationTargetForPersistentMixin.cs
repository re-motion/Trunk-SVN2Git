using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [DBTable ("MixedDomains_RelationTarget")]
  [Instantiable]
  [TestDomain]
  public abstract class RelationTargetForPersistentMixin : SimpleDomainObject<RelationTargetForPersistentMixin>
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty1 { get; set; }

    [DBBidirectionalRelation ("VirtualRelationProperty", ContainsForeignKey = true)]
    public abstract TargetClassForPersistentMixin RelationProperty2 { get; set; }

    [DBBidirectionalRelation ("CollectionProperty1Side")]
    public abstract TargetClassForPersistentMixin RelationProperty3 { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyNSide")]
    public abstract ObjectList<TargetClassForPersistentMixin> RelationProperty4 { get; }
  }
}