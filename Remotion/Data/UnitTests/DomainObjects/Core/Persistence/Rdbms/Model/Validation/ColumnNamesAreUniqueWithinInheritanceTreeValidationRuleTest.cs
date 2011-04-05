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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class ColumnNamesAreUniqueWithinInheritanceTreeValidationRuleTest : ValidationRuleTestBase
  {
    private ColumnNamesAreUniqueWithinInheritanceTreeValidationRule _validationRule;
    private ReflectionBasedClassDefinition _derivedBaseClass1;
    private ReflectionBasedClassDefinition _derivedBaseClass2;
    private ReflectionBasedClassDefinition _derivedClass;
    private ReflectionBasedClassDefinition _baseOfBaseClass;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule();
      _baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesBaseOfBaseDomainObject",
          "StorageSpecificPropertyNamesBaseOfBaseDomainObject",
          StorageProviderDefinition,
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      _derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedBase1DomainObject",
          "StorageSpecificPropertyNamesDerivedBase1DomainObject",
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          false,
          _baseOfBaseClass);
      _derivedBaseClass2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedBase2DomainObject",
          "StorageSpecificPropertyNamesDerivedBase2DomainObject",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false,
          _derivedBaseClass1);
      _derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "StorageSpecificPropertyNamesDerivedDomainObject",
          "StorageSpecificPropertyNamesDerivedDomainObject",
          StorageProviderDefinition,
          typeof (OtherDerivedValidationHierarchyClass),
          false,
          _baseOfBaseClass);

      _baseOfBaseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedBaseClass1, _derivedClass }, true, true));
      _derivedBaseClass1.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedBaseClass2 }, true, true));
      _derivedBaseClass2.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedClass.SetDerivedClasses (new ClassDefinitionCollection());
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
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          StorageClass.None,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition("Property", typeof(string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          StorageClass.None,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ propertyDefinition1, propertyDefinition2}, true));
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate (_derivedBaseClass2);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedValidationDomainObjectClass), StorageProviderDefinition);
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, 
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));

      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1, propertyDefinition2}, true));
      classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
          "Property 'PropertyWithStorageClassPersistent' of class 'DerivedValidationDomainObjectClass' must not define storage specific name 'Property', "
          + "because class 'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property "
          +"'Property' with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: Property";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedValidationDomainObjectClass), StorageProviderDefinition);
         
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property1", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          new SimpleColumnDefinition ("Property2", typeof (string), "varchar", true, false));

      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1, propertyDefinition2}, true));
      classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_TwoPersistentPropertiesWithSameStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass1, 
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (BaseValidationDomainObjectClass).GetProperty ("BaseProperty"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition2}, true));
      _derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      var expectedMessage =
          "Property 'Property' of class 'DerivedValidationDomainObjectClass' must not define storage specific name 'Property', because class "
          + "'BaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'BaseProperty' with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseValidationDomainObjectClass\r\n"
          + "Property: BaseProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_ThreePersistentPropertiesWithSameStorageSpecificPropertyName ()
    {
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _baseOfBaseClass, 
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (BaseOfBaseValidationDomainObjectClass).GetProperty ("BaseOfBaseProperty"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass1,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (BaseValidationDomainObjectClass).GetProperty ("BaseProperty"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));
      var propertyDefinition3 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedClass,
          "FirstName3",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1 }, true));
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition2 }, true));
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      _derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition3 }, true));
      _derivedBaseClass1.SetReadOnly ();
      
      var validationResults = _validationRule.Validate (_baseOfBaseClass).ToArray();

      Assert.That (validationResults.Length, Is.EqualTo (2));

      var expectedMessage1 =
          "Property 'BaseProperty' of class 'BaseValidationDomainObjectClass' must not define storage specific name 'Property', because class "
          + "'BaseOfBaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'BaseOfBaseProperty' with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass\r\n"
          + "Property: BaseOfBaseProperty";
      var expectedMessage2 =
          "Property 'Property' of class 'OtherDerivedValidationHierarchyClass' must not define storage specific name 'Property', because class "
          +"'BaseOfBaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'BaseOfBaseProperty' with the same storage "
          + "specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass\r\n"
          + "Property: BaseOfBaseProperty";
      AssertMappingValidationResult (validationResults[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResults[1], false, expectedMessage2);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass1,
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (BaseValidationDomainObjectClass).GetProperty ("BaseProperty"),
          new SimpleColumnDefinition ("Property1", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property2", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition2}, true));
      _derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInDifferentInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedClass,
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (OtherDerivedValidationHierarchyClass).GetProperty ("OtherProperty"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition2 }, true));
      _derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1}, true));
      _derivedClass.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      var expectedMessage =
          "Property 'OtherProperty' of class 'OtherDerivedValidationHierarchyClass' must not define storage specific name 'Property', because class "
          + "'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property 'Property' with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: Property";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInDifferentInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedClass,
          "FirstName1",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (OtherDerivedValidationHierarchyClass).GetProperty ("OtherProperty"),
          new SimpleColumnDefinition ("Property1", typeof (string), "varchar", true, false));
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          _derivedBaseClass2,
          "FirstName2",
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("Property"),
          new SimpleColumnDefinition ("Property2", typeof (string), "varchar", true, false));

      _baseOfBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition2}, true));
      _derivedClass.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate (_baseOfBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}