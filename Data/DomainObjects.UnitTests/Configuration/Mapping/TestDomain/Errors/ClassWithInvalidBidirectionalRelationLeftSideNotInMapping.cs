using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidBidirectionalRelationLeftSideNotInMapping : DomainObject
  {
    protected ClassWithInvalidBidirectionalRelationLeftSideNotInMapping ()
    {
    }

    [DBBidirectionalRelation ("BaseInvalidOppositePropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide BaseInvalidOppositePropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("BaseInvalidOppositeCollectionPropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide BaseInvalidOppositeCollectionPropertyTypeLeftSide { get; set; }
  }
}