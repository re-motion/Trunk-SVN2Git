using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class OtherClassWithInvalidBidirectionalRelationLeftSide : DomainObject
  {
    protected OtherClassWithInvalidBidirectionalRelationLeftSide ()
    {
    }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositePropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositeCollectionPropertyTypeLeftSide { get; set; }
  }
}