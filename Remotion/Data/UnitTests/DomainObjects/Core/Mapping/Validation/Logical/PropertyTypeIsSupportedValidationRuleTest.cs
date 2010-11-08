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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class PropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyTypeIsSupportedValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyTypeIsSupportedValidationRule();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          "DefaultStorageProvider",
          typeof (DerivedValidationDomainObjectClass),
          false);
    }

    [Test]
    public void NoRelationProperty_SupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          "PropertyWithStorageClassPersistent",
          typeof (string),
          true,
          20,
          StorageClass.Persistent,
          new FakeColumnDefinition ("PropertyWithStorageClassPersistent"));
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoRelationProperty_UnsupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithTypeObjectWithStorageClassPersistent"),
          "PropertyWithTypeObjectWithStorageClassPersistent",
          typeof (object),
          true,
          null,
          StorageClass.Persistent,
          new FakeColumnDefinition ("PropertyWithTypeObjectWithStorageClassPersistent"));
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      var expectedMessage = "The property type System.Object is not supported.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        +"Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationProperty ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("RelationPropertyWithStorageClassPersistent"),
          "RelationPropertyWithStorageClassPersistent",
          typeof (OtherDerivedValidationHierarchyClass),
          true,
          null,
          StorageClass.Persistent,
          new FakeColumnDefinition ("RelationPropertyWithStorageClassPersistent"));
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}