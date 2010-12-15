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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Validation
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
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedValidationDomainObjectClass));
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
      propertyDefinition.SetStorageProperty(new SimpleColumnDefinition ("PropertyWithStorageClassNone", typeof(string), "supported", true));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_SupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithStorageClassPersistent"),
          "PropertyWithStorageClassPersistent",
          typeof (string),
          true,
          20,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(new SimpleColumnDefinition ("PropertyWithStorageClassPersistent", typeof(string), "supported", true));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_UnsupportedType ()
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          _classDefinition,
          typeof (DerivedValidationDomainObjectClass).GetProperty ("PropertyWithTypeObjectWithStorageClassPersistent"),
          "PropertyWithTypeObjectWithStorageClassPersistent",
          typeof (object),
          true,
          null,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(new UnsupportedStorageTypeColumnDefinition("test"));
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      var expectedMessage = "The property type 'Object' is not supported by this storage provider.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        +"Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}