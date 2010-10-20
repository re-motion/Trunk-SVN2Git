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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule();
    }

    [Test]
    public void DifferentEntiyNamesInSameInheritanceHierarchyLevel ()
    {
      var baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBaseOfBaseDomainObject",
          null,
          "SPID",
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      var derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase1DomainObject",
          "EntityNamesAreDistinctBase1DomainObject",
          "SPID",
          typeof (BaseValidationDomainObjectClass),
          false,
          baseOfBaseClass);
      var derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase2DomainObject",
          "EntityNamesAreDistinctBase2DomainObject",
          "SPID",
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          baseOfBaseClass);

      var validationResult = _validationRule.Validate (baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void SameEntiyNamesInSameInheritanceHierarchyLevel ()
    {
      var baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBaseOfBaseDomainObject",
          null,
          "SPID",
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      var derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase1DomainObject",
          "SameEntityName",
          "SPID",
          typeof (BaseValidationDomainObjectClass),
          false,
          baseOfBaseClass);
      var derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase2DomainObject",
          "SameEntityName",
          "SPID",
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          baseOfBaseClass);

      var validationResult = _validationRule.Validate (baseOfBaseClass);

      var expectedMessage = "At least two classes in different inheritance branches derived from abstract class 'EntityNamesAreDistinctBaseOfBaseDomainObject'"
              + " specify the same entity name 'SameEntityName', which is not allowed.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void DifferentEntiyNamesInDifferentInheritanceHierarchyLevel ()
    {
      var baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBaseOfBaseDomainObject",
          null,
          "SPID",
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      var derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase1DomainObject",
          "EntityNamesAreDistinctBase1DomainObject",
          "SPID",
          typeof (BaseValidationDomainObjectClass),
          false,
          baseOfBaseClass);
      var derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase2DomainObject",
          null,
          "SPID",
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          baseOfBaseClass);
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctDerivedDomainObject",
          "EntityNamesAreDistinctDerivedDomainObject",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          derivedBaseClass1);

      var validationResult = _validationRule.Validate (baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void SameEntiyNamesInDifferentInheritanceHierarchyLevel ()
    {
      var baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBaseOfBaseDomainObject",
          null,
          "SPID",
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      var derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase1DomainObject",
          null,
          "SPID",
          typeof (BaseValidationDomainObjectClass),
          false,
          baseOfBaseClass);
      var derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctBase2DomainObject",
          "SameEntityName",
          "SPID",
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          baseOfBaseClass);
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNamesAreDistinctDerivedDomainObject",
          "SameEntityName",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          derivedBaseClass1);

      var validationResult = _validationRule.Validate (baseOfBaseClass);

      var expectedMessage = "At least two classes in different inheritance branches derived from abstract class 'EntityNamesAreDistinctBaseOfBaseDomainObject'"
              + " specify the same entity name 'SameEntityName', which is not allowed.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}