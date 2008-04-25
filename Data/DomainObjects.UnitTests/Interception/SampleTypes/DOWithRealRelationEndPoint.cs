using System;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithRealRelationEndPoint : DomainObject
  {
    [DBBidirectionalRelation ("RelatedObject", ContainsForeignKey = true)]
    public abstract DOWithVirtualRelationEndPoint RelatedObject { get; set; }
  }
}