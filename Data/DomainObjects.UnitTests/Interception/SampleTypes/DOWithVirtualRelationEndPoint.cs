namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithVirtualRelationEndPoint : DomainObject
  {
    [DBBidirectionalRelation ("RelatedObject")]
    public abstract DOWithRealRelationEndPoint RelatedObject { get; set; }
  }
}