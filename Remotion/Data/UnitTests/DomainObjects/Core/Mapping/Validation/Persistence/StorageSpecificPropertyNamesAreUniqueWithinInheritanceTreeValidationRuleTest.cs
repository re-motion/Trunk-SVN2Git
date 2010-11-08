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
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRuleTest : ValidationRuleTestBase
  {
    private StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule _validationRule;
    private ReflectionBasedClassDefinition _derivedBaseClass1;
    private ReflectionBasedClassDefinition _derivedBaseClass2;
    private ReflectionBasedClassDefinition _derivedClass;
    private ReflectionBasedClassDefinition _baseOfBaseClass;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule();
      _baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesBaseOfBaseDomainObject",
          "StorageSpecificPropertyNamesBaseOfBaseDomainObject",
          "SPID",
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      _derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedBase1DomainObject",
          "StorageSpecificPropertyNamesDerivedBase1DomainObject",
          "SPID",
          typeof (BaseValidationDomainObjectClass),
          false,
          _baseOfBaseClass);
      _derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedBase2DomainObject",
          "StorageSpecificPropertyNamesDerivedBase2DomainObject",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          _baseOfBaseClass);
      _derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "SPID",
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          _baseOfBaseClass);
    }

    [Test]
    public void NoInheritanceRoot ()
    {
      var validationResult = _validationRule.Validate (_derivedClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_NonPersistentPropertiesWithSameStorageSpecificPropertyName ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName1", "FirstName", StorageClass.None));
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName2", "FirstName", StorageClass.None));
      classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName1", "FirstName"));
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName2", "FirstName"));
      classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
          "Property 'FakeProperty' of class 'DerivedValidationDomainObjectClass' must not define storage specific name 'FirstName', because class "
          +"'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FakeProperty' with the same storage specific "
          +"name.\r\n\r\n"
          +"Declaring type: DerivedValidationDomainObjectClass\r\n"
          +"Property: FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "SPID",
          typeof (DerivedValidationDomainObjectClass),
          false);

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName1", "FirstName1"));
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "FirstName2", "FirstName2"));
      classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      _derivedBaseClass1.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass1, "FirstName1", "FirstName"));
      _derivedBaseClass2.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass2, "FirstName2", "FirstName"));
      _derivedBaseClass1.SetReadOnly();
      _derivedBaseClass2.SetReadOnly ();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      var expectedMessage =
          "Property 'FakeProperty' of class 'DerivedValidationDomainObjectClass' must not define storage specific name 'FirstName', because class "
          +"'BaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FakeProperty' with the same storage specific name.\r\n\r\n"
          +"Declaring type: DerivedValidationDomainObjectClass\r\n"
          +"Property: FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      _derivedBaseClass1.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass1, "FirstName1", "FirstName1"));
      _derivedBaseClass2.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass2, "FirstName2", "FirstName2"));
      _derivedBaseClass1.SetReadOnly ();
      _derivedBaseClass2.SetReadOnly ();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInDifferentInheritanceHierarchieLevel ()
    {
      _derivedClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedClass, "FirstName1", "FirstName"));
      _derivedBaseClass2.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass2, "FirstName2", "FirstName"));
      _derivedClass.SetReadOnly ();
      _derivedBaseClass2.SetReadOnly ();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      var expectedMessage =
          "Property 'FakeProperty' of class 'OtherDerivedValidationHierarchyClass' must not define storage specific name 'FirstName', because class "
          +"'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FakeProperty' with the same storage specific name.\r\n\r\n"
          +"Declaring type: OtherDerivedValidationHierarchyClass\r\n"
          +"Property: FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInDifferentInheritanceHierarchieLevel ()
    {
      _derivedClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedClass, "FirstName1", "FirstName1"));
      _derivedBaseClass2.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClass2, "FirstName2", "FirstName2"));
      _derivedClass.SetReadOnly ();
      _derivedBaseClass2.SetReadOnly ();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }


  }
}