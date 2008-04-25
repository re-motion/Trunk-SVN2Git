using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public class MixinAddingBidirectionalRelationTwice : DomainObjectMixin<DomainObject>
  {
    [DBBidirectionalRelation ("VirtualSide", ContainsForeignKey = true)]
    public RelationTargetForMixinAddingBidirectionalRelationTwice RealSide
    {
      get
      {
        return Properties[typeof (MixinAddingBidirectionalRelationTwice), "RealSide"]
            .GetValue<RelationTargetForMixinAddingBidirectionalRelationTwice>();
      }
      set { Properties[typeof (MixinAddingBidirectionalRelationTwice), "RealSide"].SetValue (value); }
    }
  }
}