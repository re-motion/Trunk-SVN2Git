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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class PropertyTypeIsSupportedByStorageProviderValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyTypeIsSupportedByStorageProviderValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyTypeIsSupportedByStorageProviderValidationRule();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          typeof (DerivedValidationDomainObjectClass),
          false);
    }

    [Test]
    public void PropertyWithStorageClassNone ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassNone"),
          "PropertyWithStorageClassNone",
          typeof (string),
          true,
          20,
          StorageClass.None);
      propertyDefinition.SetStorageProperty(new FakeColumnDefinition ("PropertyWithStorageClassNone"));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_NoRelationProperty_SupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          "PropertyWithStorageClassPersistent",
          typeof (string),
          true,
          20,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(new FakeColumnDefinition ("PropertyWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_NoRelationProperty_UnsupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithTypeObjectWithStorageClassPersistent"),
          "PropertyWithTypeObjectWithStorageClassPersistent",
          typeof (object),
          true,
          null,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(new FakeColumnDefinition ("PropertyWithTypeObjectWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult.Length, Is.EqualTo(1));
      var expectedMessage = "The property type 'Object' is not supported by this storage provider.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        +"Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }

    [Test]
    public void RelationPropertyWithStorageClassPersistent_RelationProperty ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("RelationPropertyWithStorageClassPersistent"),
          "RelationPropertyWithStorageClassPersistent",
          typeof (OtherDerivedValidationHierarchyClass),
          true,
          null,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty (new FakeColumnDefinition ("RelationPropertyWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }
  }
}