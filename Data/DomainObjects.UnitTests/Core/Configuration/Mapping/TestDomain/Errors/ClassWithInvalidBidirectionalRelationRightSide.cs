using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidBidirectionalRelationRightSide: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelationRightSide ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide NoContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositePropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("BaseInvalidOppositePropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide BaseInvalidOppositePropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositeCollectionPropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("BaseInvalidOppositeCollectionPropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide BaseInvalidOppositeCollectionPropertyTypeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeForCollectionPropertyRightSide { get; set; }    
  
    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide CollectionPropertyContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("NonCollectionPropertyHavingASortExpressionLeftSide")]
    public abstract ObjectList<ClassWithInvalidBidirectionalRelationLeftSide> NonCollectionPropertyHavingASortExpressionRightSide { get; set; }
  }
}