using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinMixedSides))]
  public abstract class TargetClassWithRelationPropertyMixedSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn("One")]
    public abstract RelationTargetMixedSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetMixedSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = false)]
    public abstract TargetClassWithRelationPropertyMixedSides Opposite { get; set; }
  }

  public class MixinMixedSides : DomainObjectMixin<TargetClassWithRelationPropertyMixedSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn ("Two")]
    public RelationTargetMixedSides Relation
    {
      get { return Properties[typeof (MixinMixedSides), "Relation"].GetValue<RelationTargetMixedSides> (); }
      set { Properties[typeof (MixinMixedSides), "Relation"].SetValue (value); }
    }
  }
}