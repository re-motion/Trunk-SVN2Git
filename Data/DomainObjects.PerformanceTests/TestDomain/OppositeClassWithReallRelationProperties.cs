using System;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Serializable]
  public abstract class OppositeClassWithRealRelationProperties : SimpleDomainObject<OppositeClassWithRealRelationProperties>
  {
    [DBBidirectionalRelation ("Virtual1", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real1 { get; set; }
    [DBBidirectionalRelation ("Virtual2", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real2 { get; set; }
    [DBBidirectionalRelation ("Virtual3", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real3 { get; set; }
    [DBBidirectionalRelation ("Virtual4", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real4 { get; set; }
    [DBBidirectionalRelation ("Virtual5", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real5 { get; set; }
    [DBBidirectionalRelation ("Virtual6", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real6 { get; set; }
    [DBBidirectionalRelation ("Virtual7", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real7 { get; set; }
    [DBBidirectionalRelation ("Virtual8", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real8 { get; set; }
    [DBBidirectionalRelation ("Virtual9", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real9 { get; set; }
    [DBBidirectionalRelation ("Virtual10", ContainsForeignKey = true)]
    public abstract ClassWithRelationProperties Real10 { get; set; }
  }
}