using System;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Serializable]
  public abstract class OppositeClassWithVirtualRelationProperties : SimpleDomainObject<OppositeClassWithVirtualRelationProperties>
  {
    [DBBidirectionalRelation ("Real1")]
    public abstract ClassWithRelationProperties Virtual1 { get; set; }
    [DBBidirectionalRelation ("Real2")]
    public abstract ClassWithRelationProperties Virtual2 { get; set; }
    [DBBidirectionalRelation ("Real3")]
    public abstract ClassWithRelationProperties Virtual3 { get; set; }
    [DBBidirectionalRelation ("Real4")]
    public abstract ClassWithRelationProperties Virtual4 { get; set; }
    [DBBidirectionalRelation ("Real5")]
    public abstract ClassWithRelationProperties Virtual5 { get; set; }
    [DBBidirectionalRelation ("Real6")]
    public abstract ClassWithRelationProperties Virtual6 { get; set; }
    [DBBidirectionalRelation ("Real7")]
    public abstract ClassWithRelationProperties Virtual7 { get; set; }
    [DBBidirectionalRelation ("Real8")]
    public abstract ClassWithRelationProperties Virtual8 { get; set; }
    [DBBidirectionalRelation ("Real9")]
    public abstract ClassWithRelationProperties Virtual9 { get; set; }
    [DBBidirectionalRelation ("Real10")]
    public abstract ClassWithRelationProperties Virtual10 { get; set; }
  }
}