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
using
    Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Reflection.
        MappingAttributesAreSupportedForPropertyTypeValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreSupportedForPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreSupportedForPropertyTypeValidationRule _validtionRule;

    [SetUp]
    public void SetUp ()
    {
      _validtionRule = new MappingAttributesAreSupportedForPropertyTypeValidationRule();
    }

    [Test]
    public void ValidPropertyWithStringPropertyAttribute ()
    {
      var type = typeof (ClassWithValidPropertyAttributes);
      var propertyInfo = type.GetProperty ("StringProperty");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);
     
      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithStringPropertyAttribute ()
    {
      var type = typeof (ClassWithInvalidPropertyAttributes);
      var propertyInfo = type.GetProperty ("IntPropertyWithStringPropertyAttribute");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, false, 
        "The 'Remotion.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.");
    }

    [Test]
    public void ValidPropertyWithBinaryPropertyAttribute ()
    {
      var type = typeof (ClassWithValidPropertyAttributes);
      var propertyInfo = type.GetProperty ("BinaryProperty");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithBinaryPropertyAttribute ()
    {
      var type = typeof (ClassWithInvalidPropertyAttributes);
      var propertyInfo = type.GetProperty ("BoolPropertyWithBinaryPropertyAttribute");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, false,
        "The 'Remotion.Data.DomainObjects.BinaryPropertyAttribute' may be only applied to properties of type 'System.Byte[]'.");
    }

    [Test]
    public void ValidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var type = typeof (ClassWithValidPropertyAttributes);
      var propertyInfo = type.GetProperty ("ExtensibleEnumProperty");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var type = typeof (ClassWithInvalidPropertyAttributes);
      var propertyInfo = type.GetProperty ("StringPropertyWithExtensibleEnumPropertyAttribute");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, false,
        "The 'Remotion.Data.DomainObjects.ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'Remotion.ExtensibleEnums.IExtensibleEnum'.");
    }

    [Test]
    public void ValidPropertyWithMandatoryPropertyAttribute ()
    {
      var type = typeof (ClassWithValidPropertyAttributes);
      var propertyInfo = type.GetProperty ("MandatoryProperty");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithMandatoryPropertyAttribute ()
    {
      var type = typeof (ClassWithInvalidPropertyAttributes);
      var propertyInfo = type.GetProperty ("StringPropertyWithMandatoryPropertyAttribute");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      var propertyDefinition = new TestablePropertyDefinition (classDefinition, propertyInfo, 20, StorageClass.Persistent);

      var validationResult = _validtionRule.Validate (propertyDefinition);

      AssertMappingValidationResult (validationResult, false,
        "The 'Remotion.Data.DomainObjects.MandatoryAttribute' may be only applied to properties assignable to types "
        +"'Remotion.Data.DomainObjects.DomainObject' or 'Remotion.Data.DomainObjects.ObjectList`1[T]'.");
    }
  }
}