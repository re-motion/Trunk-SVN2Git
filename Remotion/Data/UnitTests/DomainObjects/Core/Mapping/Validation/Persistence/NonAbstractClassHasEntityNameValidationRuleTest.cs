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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class NonAbstractClassHasEntityNameValidationRuleTest : ValidationRuleTestBase
  {
    private NonAbstractClassHasEntityNameValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new NonAbstractClassHasEntityNameValidationRule();
    }

    [Test]
    public void ClassTypeResolved_NoEntityName_Abstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          true);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_EntityName_NotAbstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          "EntityName",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_EntityName_Abstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          "EntityName",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeUnresolved ()
    {
      var classDefinition = new ReflectionBasedClassDefinitionWithUnresolvedClassType (
          "NonAbstractClassHasEntityNameDomainObject",
          new StorageEntityDefinitionStub(null),
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_NoEntityName_NotAbstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
          "Neither class 'DerivedValidationDomainObjectClass' nor its base classes specify an entity name. Make class 'DerivedValidationDomainObjectClass' "
          +"abstract or apply a 'DBTable' attribute to it or one of its base classes.\r\n\r\n"
          +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
    
  }
}