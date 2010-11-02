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
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1.RelationProperty2' cannot have two non-virtual end points.\r\n"
      + "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2.RelationProperty1' cannot have two non-virtual end points.")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "")]
    [Ignore("TODO 3424: relations without foreign key are not added to the mapping")]
    public void OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelation_ContainsForeignKeyIsFalseOnBothSites");
    }

    [Test]
    [Ignore("TODO 3424: no mapping exception is thrown !??")]
    public void OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnTheOneSite ()
    {
      ValidateMapping ("NotSupportedRelations.OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnOneSite");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.")]
    public void OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnTheManySite ()
    {
      ValidateMapping ("NotSupportedRelations.OnetoManyBidirectionalRelation_ContainsForeignKeyIsTrueOnManySite");
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException),
      ExpectedMessage = "Only relation end points with a property type of 'Remotion.Data.DomainObjects.DomainObject' can contain the foreign key.")]
    public void OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelation_ContainsForeignKeyIsTrueOnBothSites");
    }

    //SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule
    [Test]
    [ExpectedException(typeof(MappingException), ExpectedMessage = "")]
    [Ignore ("TODO 3424: relations without foreign key are not added to the mapping")]
    public void OneToOneBidirectionalRelationWithSortExpression ()
    {
      ValidateMapping ("NotSupportedRelations.OneToOneBidirectionalRelationWithSortExpression");
    }

    //SortExpressionParser (used for VirtualRelationEndPointDefinitions)
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "")]
    [Ignore ("TODO 3424: relations without foreign key are not added to the mapping")]
    public void OneToManyBidirectionalRelationWithInvalidSortExpression ()
    {
      ValidateMapping ("NotSupportedRelations.OneToManyBidirectionalRelationWithInvalidSortExpression");
    }

    //Exception is thrown in RelationReflector.ValidateOppositePropertyInfoBidirectionalRelationAttribute (first condition)
    [Test]
    [ExpectedException (typeof (MappingException),
      ExpectedMessage = "Opposite relation property 'RelationProperty1' declared on type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +"TestDomain.Errors.ValidationIntegration.NotSupportedRelations.BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite.InvalidRelationClass1' "
      +"does not define a matching 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
      + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedRelations."
      +"BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite.InvalidRelationClass2, property: RelationProperty2")]
    public void BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationWithBidirectionalRelationAttributeOnOneSite");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "")]
    [Ignore("TODO 3424")]
    public void BidirectionalRelationAttributeOnWrongPropertyType ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationAttributeOnWrongPropertyType");
    }

    [Test]
    [Ignore("TODO 3424")]
    public void BidirectionalRelationWithInvalidPropertyReferences ()
    {
      ValidateMapping ("NotSupportedRelations.BidirectionalRelationWithInvalidPropertyReferences");
    }
  }
}