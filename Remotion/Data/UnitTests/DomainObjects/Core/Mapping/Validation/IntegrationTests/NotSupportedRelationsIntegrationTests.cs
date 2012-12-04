// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class NotSupportedRelationsIntegrationTests : ValidationIntegrationTestBase
  {
    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "The relation between property 'RelationProperty2', declared on type 'InvalidRelationClass1', and property 'RelationProperty1' declared on type "
      +"'InvalidRelationClass2', contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the "
      +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1\r\n"
      +"Property: RelationProperty2\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites."
      +"InvalidRelationClass1.RelationProperty2->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration."
      +"NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2.RelationProperty1\r\n"
      +"----------\r\n"
      +"The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
      +"'InvalidRelationClass1', contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the "
      +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2\r\n"
      +"Property: RelationProperty1\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites."
      +"InvalidRelationClass2.RelationProperty1->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration."
      +"NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1.RelationProperty2")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
    "The relation between property 'RelationProperty2', declared on type 'InvalidRelationClass1', and property 'RelationProperty1' declared on type "
    +"'InvalidRelationClass2', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
    +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
    +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1\r\n"
    +"Property: RelationProperty2\r\n"
    +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    +"TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    + "InvalidRelationClass2.RelationProperty1->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    + "InvalidRelationClass1.RelationProperty2\r\n"
    +"----------\r\n"
    +"The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
    +"'InvalidRelationClass1', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
    +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
    +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2\r\n"
    +"Property: RelationProperty1\r\n"
    +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    +"TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    + "InvalidRelationClass1.RelationProperty2->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    + "InvalidRelationClass2.RelationProperty1")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"+
      "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnManySite.InvalidRelationClass2\r\nProperty: RelationProperty2")]
    public void OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnTheManySite ()
    {
      ValidateMapping ("NotSupportedRelations.OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnManySite");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2\r\nProperty: RelationProperty2")]
    public void OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Property 'RelationProperty1' of class 'InvalidRelationClass1' must not specify a SortExpression, because cardinality is equal to 'one'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "OneToOneBidirectionalRelationWithSortExpression.InvalidRelationClass1\r\nProperty: RelationProperty1")]
    public void OneToOneBidirectionalRelationWithSortExpression ()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelationWithSortExpression");
    }

    //SortExpressionIsValidValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
       "SortExpression 'InvalidProperty' cannot be parsed: 'InvalidProperty' is not a valid mapped property name. Expected the .NET property name of a property declared by the "
      +"'InvalidRelationClass1' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin or a "
      +"derived class of 'InvalidRelationClass1', the full unique re-store property identifier can be specified.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"OneToManyBidirectionalRelationWithInvalidSortExpression.InvalidRelationClass2\r\nProperty: RelationProperty2")]
    public void OneToManyBidirectionalRelationWithInvalidSortExpression ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelationWithInvalidSortExpression");
    }

    //RelationEndPointNamesAreConsistentValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "Opposite relation property 'RelationProperty1' declared on type 'InvalidRelationClass1' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite.InvalidRelationClass2\r\n"
      + "Property: RelationProperty2")]
    public void BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite");
    }

    //RelationEndPointNamesAreConsistentValidationRule / CheckForInvalidRelationEndPointsValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "Opposite relation property 'RelationProperty1' declared on type 'InvalidRelationClass2' defines a 'DBBidirectionalRelationAttribute' whose "
      + "opposite property does not match.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelation_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass1\r\n"
      + "Property: RelationProperty1\r\n"
      + "----------\r\n"
      + "Opposite relation property 'RelationProperty2' declared on type 'InvalidRelationClass1' does not "
      + "define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelation_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass2\r\n"
      + "Property: RelationProperty1\r\n"
      + "----------\r\n"
      + "Property 'RelationProperty2' on class 'InvalidRelationClass1' could not be found.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelation_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass1")]
    public void BidirectionalRelation_RelationEndPointDefinitionsDoNotMatch ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_RelationEndPointDefinitionsDoNotMatch");
    }

    //RelationEndPointTypesAreConsistentValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "The type 'BaseRelationClass2' does not match the type of the opposite relation propery 'RelationProperty1' declared on type 'InvalidRelationClass1'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelation_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot.BaseRelationClass2\r\n"
      + "Property: RelationProperty3")]
    public void BidirectionalRelation_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "The type 'Base' does not match the type of the opposite relation propery 'RelationPropertyPointingToDerived' declared on type 'InvalidRelationClass'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "BidirectionalRelation_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot.Base\r\n"
      + "Property: RelationProperty")]
    public void BidirectionalRelation_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot");
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    [ExpectedException(typeof(MappingException), ExpectedMessage =
      "Opposite relation property 'RelationProperty' declared on type 'ClassNotInMapping' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "RelationPropertyTypeNotInMapping.InvalidRelationClass1\r\n"
      + "Property: RelationProperty\r\n"
      + "----------\r\n"
      + "Property 'RelationProperty' on class 'ClassNotInMapping' could not be found.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "RelationPropertyTypeNotInMapping.ClassNotInMapping\r\n"
      + "----------\r\n"
      + "The relation property 'RelationProperty' has return type 'ClassNotInMapping', which is not a part of the mapping. Relation properties must "
      + "not point to classes above the inheritance root.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      + "RelationPropertyTypeNotInMapping.InvalidRelationClass1\r\n"
      + "Property: RelationProperty")]
    public void RelationPropertyTypeNotInMapping ()
    {
      ValidateMapping ("NotSupportedRelations.RelationPropertyTypeNotInMapping");
    }

    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    [ExpectedException(typeof(MappingException), ExpectedMessage = 
      "The relation between property 'RelationProperty', declared on type 'InvalidRelationClass1', and property 'RelationProperty' declared on type "
      +"'InvalidRelationClass2', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
      +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"ManyToManyBidirectionalRelation.InvalidRelationClass1\r\n"
      +"Property: RelationProperty\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"ManyToManyBidirectionalRelation.InvalidRelationClass2:Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration."
      +"NotSupportedRelations.ManyToManyBidirectionalRelation.InvalidRelationClass2.RelationProperty->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Validation.Integration.NotSupportedRelations.ManyToManyBidirectionalRelation.InvalidRelationClass1.RelationProperty\r\n"
      +"----------\r\n"
      +"The relation between property 'RelationProperty', declared on type 'InvalidRelationClass2', and property 'RelationProperty' declared on type "
      +"'InvalidRelationClass1', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
      +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"ManyToManyBidirectionalRelation.InvalidRelationClass2\r\n"
      +"Property: RelationProperty\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
      +"ManyToManyBidirectionalRelation.InvalidRelationClass1:Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration."
      +"NotSupportedRelations.ManyToManyBidirectionalRelation.InvalidRelationClass1.RelationProperty->Remotion.Data.UnitTests."
      +"DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.ManyToManyBidirectionalRelation.InvalidRelationClass2.RelationProperty")]
    public void ManyToManyBidirectionalRelation ()
    {
      ValidateMapping ("NotSupportedRelations.ManyToManyBidirectionalRelation");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The relation property 'BidirectionalRelationProperty' has return type 'DomainObject', which is not a part of the mapping. "
        + "Relation properties must not point to classes above the inheritance root.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.BidirectionalRelation_ReferencingDomainObjectType.ClassReferencingDomainObjectType\r\n"
        + "Property: BidirectionalRelationProperty",
        MatchType = MessageMatch.Contains)]
    public void BidirectionalRelation_ReferencingDomainObjectType ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_ReferencingDomainObjectType");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.BidirectionalRelation_ReferencingNonDomainObject.ClassReferencingNonDomainObject\r\n"
        + "Property: BidirectionalRelationProperty")]
    public void BidirectionalRelation_ReferencingNonDomainObject ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_ReferencingNonDomainObject");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.BidirectionalRelation_ReferencingObject.ClassReferencingObject\r\n"
        + "Property: BidirectionalRelationProperty\r\n"
        + "----------\r\n"
        + "The property type 'Object' is not supported. If you meant to declare a relation, 'Object' must be derived from 'DomainObject'. "
        + "For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.BidirectionalRelation_ReferencingObject.ClassReferencingObject\r\n"
        + "Property: BidirectionalRelationProperty")]
    public void BidirectionalRelation_ReferencingObjectType ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelation_ReferencingObject");
    }
  }
}