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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
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
          typeof (DerivedValidationDomainObjectClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasNoMyEntityName_And_HasBaseClass ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          null,
          typeof (BaseValidationDomainObjectClass),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          null,
          typeof (DerivedValidationDomainObjectClass),
          false,
          baseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasBaseClassWithSameEntityName ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "EntityName",
          typeof (BaseValidationDomainObjectClass),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          typeof (DerivedValidationDomainObjectClass),
          false,
          baseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasBaseClassWithDifferentEntityName ()
    {
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          typeof (BaseValidationDomainObjectClass),
          true);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          typeof (DerivedValidationDomainObjectClass),
          false,
          baseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult.Length, Is.EqualTo (1));
      var expectedMessage = "Class 'DerivedValidationDomainObjectClass' must not specify an entity name 'EntityName' which is different from inherited "
        +"entity name 'BaseEntityName'.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }

    
  }
}