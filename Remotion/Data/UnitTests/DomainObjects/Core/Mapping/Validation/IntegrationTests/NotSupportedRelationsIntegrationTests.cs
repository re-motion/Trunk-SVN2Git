// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1\r\n"
      +"Property: RelationProperty2\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Errors.ValidationIntegration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites."
      +"InvalidRelationClass1.RelationProperty2\r\n"
      +"The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
      +"'InvalidRelationClass1', contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the "
      +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2\r\n"
      +"Property: RelationProperty1\r\n"
      +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Errors.ValidationIntegration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites."
      +"InvalidRelationClass2.RelationProperty1")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //TODO: Split rule into RdbmsXXXValidationRule and abstract XXXValidationRule. Instantiate RdbmsXXXValidationRule via factory from XXXValidationRule. Compare with rdbmsRelationReflector.
    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
    "The relation between property 'RelationProperty2', declared on type 'InvalidRelationClass1', and property 'RelationProperty1' declared on type "
    +"'InvalidRelationClass2', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
    +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
    +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1\r\n"
    +"Property: RelationProperty2\r\n"
    +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    +"TestDomain.Errors.ValidationIntegration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    +"InvalidRelationClass2.RelationProperty1\r\n"
    +"The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
    +"'InvalidRelationClass1', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
    +"'DBBidirectionalRelationAttribute'.\r\n\r\n"
    +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2\r\n"
    +"Property: RelationProperty1\r\n"
    +"Relation ID: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
    +"OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
    +"TestDomain.Errors.ValidationIntegration.NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites."
    +"InvalidRelationClass1.RelationProperty2")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites");
    }

    [Test]
    [Ignore("TODO 3424")]
    public void ManyToManyBidirectionalRelation ()
    {
      ValidateMapping ("NotSupportedRelations.ManyToManyBidirectionalRelation");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n\r\n"+
      "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      + "OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnManySite.InvalidRelationClass2'\r\nProperty: 'RelationProperty2'")]
    public void OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnTheManySite ()
    {
      ValidateMapping ("NotSupportedRelations.OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnManySite");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n\r\n"
      + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2'\r\nProperty: 'RelationProperty2'")]
    public void OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Property 'RelationProperty1' of class 'InvalidRelationClass1' must not specify a SortExpression, because cardinality is equal to 'one'.\r\n\r\n"
      + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      + "OneToOneBidirectionalRelationWithSortExpression.InvalidRelationClass1'\r\nProperty: 'RelationProperty1'")]
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
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToManyBidirectionalRelationWithInvalidSortExpression.InvalidRelationClass2'\r\nProperty: 'RelationProperty2'")]
    public void OneToManyBidirectionalRelationWithInvalidSortExpression ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelationWithInvalidSortExpression");
    }

    //RelationEndPointDeclarationsDoNotMatchValidationRule
    [Test]
    //[ExpectedException (typeof (MappingException), ExpectedMessage =
    //  "Opposite relation property 'RelationProperty1' declared on type 'InvalidRelationClass1' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
    //  + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
    //  + "BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite.InvalidRelationClass2\r\n"
    //  + "Property: RelationProperty2")]
    [Ignore("TODO 3424")]
    public void BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite");
    }
    
    //TODO 3424: check error message
    //Exception is thrown in RelationReflector.ValidateOppositePropertyInfoDeclaringType (first condition)
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "The declaring type 'BaseRelationClass2' does not match the type of the opposite relation propery 'RelationProperty1' "
      + "declared on type 'InvalidRelationClass1'.\r\n\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      + "BidirectionalRelationWithInvalidPropertyReferences.BaseRelationClass2\r\n"
      + "Property: RelationProperty3")]
    public void BidirectionalRelationWithInvalidPropertyReferences ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationWithInvalidPropertyReferences");
    }
  }
}