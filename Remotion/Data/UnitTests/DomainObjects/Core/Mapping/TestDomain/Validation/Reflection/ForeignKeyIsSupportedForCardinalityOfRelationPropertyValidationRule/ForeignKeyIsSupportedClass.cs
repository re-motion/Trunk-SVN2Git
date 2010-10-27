// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
{
  public class ForeignKeyIsSupportedClass : DomainObject
  {
    public string PropertyWithNoDbBidirectionalRelationAttribute { get; set; }

    [DBBidirectionalRelation ("OppositeProperty", ContainsForeignKey = true)]
    public string NoCollectionProperty_ContainsForeignKey { get; set; }

    [DBBidirectionalRelation ("OppositeProperty", ContainsForeignKey = true)]
    public ObjectList<OtherDerivedValidationHierarchyClass> CollectionProperty_ContainsForeignKey { get; set; }

    [DBBidirectionalRelation ("OppositeProperty", ContainsForeignKey = false)]
    public string NoCollectionProperty_ContainsNoForeignKey { get; set; }

    [DBBidirectionalRelation ("OppositeProperty", ContainsForeignKey = false)]
    public ObjectList<OtherDerivedValidationHierarchyClass> CollectionProperty_ContainsNoForeignKey { get; set; }
  }
}