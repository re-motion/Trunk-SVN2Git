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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Persistence.EntityNameMatchesParentEntityNameValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Persistence
{
  [TestFixture]
  public class EntityNameMatchesParentEntityNameValidationRuleTest : ValidationRuleTestBase
  {
    private EntityNameMatchesParentEntityNameValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new EntityNameMatchesParentEntityNameValidationRule();
    }

    [Test]
    public void HasNoBaseClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          "SPID",
          typeof (EntityNameMatchesParentEntityNameDomainObject),
          false);
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasNoMyEntityName_And_HasBaseClass ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          null,
          "SPID",
          typeof (EntityNameMatchesParentEntityNameBaseDomainObject),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          null,
          "SPID",
          typeof (EntityNameMatchesParentEntityNameDomainObject),
          false,
          baseClassDefinition,
          new PersistentMixinFinderMock (typeof (EntityNameMatchesParentEntityNameDomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClassWithSameEntityName ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "EntityName",
          "SPID",
          typeof (EntityNameMatchesParentEntityNameBaseDomainObject),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          "SPID",
          typeof (EntityNameMatchesParentEntityNameDomainObject),
          false,
          baseClassDefinition,
          new PersistentMixinFinderMock (typeof (EntityNameMatchesParentEntityNameDomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClassWithDifferentEntityName ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          "SPID",
          typeof (EntityNameMatchesParentEntityNameBaseDomainObject),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          "SPID",
          typeof (EntityNameMatchesParentEntityNameDomainObject),
          false,
          baseClassDefinition,
          new PersistentMixinFinderMock (typeof (EntityNameMatchesParentEntityNameDomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage = "Class 'EntityNameMatchesParentEntityNameDomainObject' must not specify an entity name 'EntityName' "
        +"which is different from inherited entity name 'BaseEntityName'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    
  }
}