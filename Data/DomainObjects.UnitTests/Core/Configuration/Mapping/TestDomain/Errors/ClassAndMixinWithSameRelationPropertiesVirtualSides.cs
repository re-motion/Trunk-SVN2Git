using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinVirtualSides))]
  public abstract class TargetClassWithRelationPropertyVirtualSides : DomainObject
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn("One")]
    public abstract RelationTargetVirtualSides Relation { get; set; }
  }

  [DBTable]
  [Instantiable]
  public abstract class RelationTargetVirtualSides : DomainObject
  {
    [DBBidirectionalRelation ("Relation", ContainsForeignKey = true)]
    public abstract TargetClassWithRelationPropertyVirtualSides Opposite { get; set; }
  }

  public class MixinVirtualSides : DomainObjectMixin<TargetClassWithRelationPropertyVirtualSides>
  {
    [DBBidirectionalRelation ("Opposite", ContainsForeignKey = false)]
    [DBColumn ("Two")]
    public RelationTargetVirtualSides Relation
    {
      get { return Properties[typeof (MixinVirtualSides), "Relation"].GetValue<RelationTargetVirtualSides> (); }
      set { Properties[typeof (MixinVirtualSides), "Relation"].SetValue (value); }
    }
  }
}