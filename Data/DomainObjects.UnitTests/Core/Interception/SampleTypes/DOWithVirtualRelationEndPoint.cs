namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithVirtualRelationEndPoint : DomainObject
  {
    [DBBidirectionalRelation ("RelatedObject")]
    public abstract DOWithRealRelationEndPoint RelatedObject { get; set; }
  }
}