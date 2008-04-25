namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Instantiable]
  public abstract class RelationTargetForMixinAddingBidirectionalRelationTwice : SimpleDomainObject<RelationTargetForMixinAddingBidirectionalRelationTwice>
  {
    [DBBidirectionalRelation ("RealSide")]
    public abstract TargetClass1ForMixinAddingBidirectionalRelationTwice VirtualSide { get; set; }
  }
}