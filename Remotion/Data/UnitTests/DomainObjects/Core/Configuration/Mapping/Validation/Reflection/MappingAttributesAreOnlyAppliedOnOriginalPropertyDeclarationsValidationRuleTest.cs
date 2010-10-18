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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule();
    }

    [Test]
    public void OriginalPropertyDeclaration ()
    {
      var type = typeof (BaseMappingAttributesClass);
      var propertyInfo = type.GetProperty ("Property1");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validationRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NonOriginalPropertyDeclarationWithoutMappingAttribute ()
    {
      var type = typeof (DerivedClassWithMappingAttribute);
      var propertyInfo = type.GetProperty ("Property2");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validationRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NonOriginalPropertyDeclarationWithMappingAttribute ()
    {
      var type = typeof (DerivedClassWithMappingAttribute);
      var propertyInfo = type.GetProperty ("Property1");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validationRule.Validate (propertyDefinition);

      var expectedMessage = 
        "The 'Remotion.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n  "
        +"Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.DerivedClassWithMappingAttribute, "
        +"property: Property1";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}