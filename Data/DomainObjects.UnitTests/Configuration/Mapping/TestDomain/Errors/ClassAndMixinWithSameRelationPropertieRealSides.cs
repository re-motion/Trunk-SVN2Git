using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinRealSides))]
  public abstract class TargetClassWithRelationPropertyRealSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn("One")]
    public abstract RelationTargetRealSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetRealSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = false)]
    public abstract TargetClassWithRelationPropertyRealSides Opposite { get; set; }
  }

  public class MixinRealSides : DomainObjectMixin<TargetClassWithRelationPropertyRealSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = true)]
    [DBColumn ("Two")]
    public RelationTargetRealSides Relation
    {
      get { return Properties[typeof (MixinRealSides), "Relation"].GetValue<RelationTargetRealSides> (); }
      set { Properties[typeof (MixinRealSides), "Relation"].SetValue (value); }
    }
  }
}