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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreSupportedForPropertyTypeValidationRule;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreSupportedForPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreSupportedForPropertyTypeValidationRule _validtionRule;
    private ReflectionBasedClassDefinition _classDefinition;
    private Type _validType;
    private Type _invalidType;

    [SetUp]
    public void SetUp ()
    {
      _validtionRule = new MappingAttributesAreSupportedForPropertyTypeValidationRule();
      _validType = typeof (ClassWithValidPropertyAttributes);
      _invalidType = typeof (ClassWithInvalidPropertyAttributes);
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (_validType.Name, _validType.Name, "SPID", _validType, false);
    }

    [Test]
    public void ValidPropertyWithStringPropertyAttribute ()
    {
      var propertyInfo = _validType.GetProperty ("StringProperty");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithStringPropertyAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("IntPropertyWithStringPropertyAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false, 
        "The 'StringPropertyAttribute' may be only applied to properties of type 'String'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        +"Property: IntPropertyWithStringPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithBinaryPropertyAttribute ()
    {
      var propertyInfo = _validType.GetProperty ("BinaryProperty");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithBinaryPropertyAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("BoolPropertyWithBinaryPropertyAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false, "The 'BinaryPropertyAttribute' may be only applied to properties of type 'Byte[]'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        +"Property: BoolPropertyWithBinaryPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var propertyInfo = _validType.GetProperty ("ExtensibleEnumProperty");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("StringPropertyWithExtensibleEnumPropertyAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false, "The 'ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'IExtensibleEnum'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithExtensibleEnumPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithMandatoryPropertyAttribute ()
    {
      var propertyInfo = _validType.GetProperty ("MandatoryProperty");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithMandatoryPropertyAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("StringPropertyWithMandatoryPropertyAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false, 
        "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute");
    }

    [Test]
    public void InvalidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("StringPropertyWithMandatoryPropertyAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false,
        "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute");
    }

    [Test]
    public void ValidValidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyInfo = _validType.GetProperty ("BidirectionalRelationProperty");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidValidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyInfo = _invalidType.GetProperty ("StringPropertyWithBidirectionalRelationAttribute");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).First();

      AssertMappingValidationResult (validationResult, false,
        "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithBidirectionalRelationAttribute");
    }

    [Test]
    public void SeveralInvalidProperties ()
    {
      var propertyInfo1 = _invalidType.GetProperty ("StringPropertyWithMandatoryPropertyAttribute");
      var propertyDefinition1 = new TestablePropertyDefinition (_classDefinition, propertyInfo1, 20, StorageClass.Persistent);

      var propertyInfo2 = _invalidType.GetProperty ("StringPropertyWithExtensibleEnumPropertyAttribute");
      var propertyDefinition2 = new TestablePropertyDefinition (_classDefinition, propertyInfo2, 20, StorageClass.Persistent);

      var propertyInfo3 = _invalidType.GetProperty ("BoolPropertyWithBinaryPropertyAttribute");
      var propertyDefinition3 = new TestablePropertyDefinition (_classDefinition, propertyInfo3, 20, StorageClass.Persistent);

      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1, propertyDefinition2, propertyDefinition3}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).ToArray();

      var expectedMessage1 = "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute";
      var expectedMessage2 = "The 'ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'IExtensibleEnum'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithExtensibleEnumPropertyAttribute";
      var expectedMessage3 = "The 'BinaryPropertyAttribute' may be only applied to properties of type 'Byte[]'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: BoolPropertyWithBinaryPropertyAttribute";
      Assert.That (validationResult.Length, Is.EqualTo (3));
      AssertMappingValidationResult (validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResult[1], false, expectedMessage2);
      AssertMappingValidationResult (validationResult[2], false, expectedMessage3);
    }
  }
}