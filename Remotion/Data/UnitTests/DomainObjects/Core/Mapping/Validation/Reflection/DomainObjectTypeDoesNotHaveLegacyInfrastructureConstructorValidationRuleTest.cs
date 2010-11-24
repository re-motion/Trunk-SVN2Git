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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeDoesNotHaveLegacyInfrastructureConcstructorValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRuleTest : ValidationRuleTestBase
  {
    private DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule();
    }

    [Test]
    public void NonAbstractType_WithoutLegacyConstructor ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (NonAbstractClassWithoutLegacyConstructor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NonAbstractType_WithLegacyConstructor()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (NonAbstractClassWithLegacyConstructor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage = 
        "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer argument). "
        +"The reflection-based mapping does not use this constructor any longer and requires it to be removed.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"DomainObjectTypeDoesNotHaveLegacyInfrastructureConcstructorValidationRule.NonAbstractClassWithLegacyConstructor";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void AbstractType_WithoutInstantiableAttribute_And_WithoutLegacyConstructor ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (AbstractClassWithoutAttributeAndLegacyCtor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void AbstractType_WithoutInstantiableAttribute_And_WithLegacyConstructor ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (AbstractClassWithoutAttributeAndWithLegacyCtor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void AbstractType_WithInstantiableAttribute_And_WithoutLegacyConstructor ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (AbstractClassWithAttributeAndWithoutLegacyCtor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void AbstractType_WithInstantiableAttribute_And_WithLegacyConstructor ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (AbstractClassWithAttributeAndWithLegacyCtor));
      
      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
        "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer argument). "
        +"The reflection-based mapping does not use this constructor any longer and requires it to be removed.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"DomainObjectTypeDoesNotHaveLegacyInfrastructureConcstructorValidationRule.AbstractClassWithAttributeAndWithLegacyCtor";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}