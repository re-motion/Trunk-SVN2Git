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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class PropertyNamesAreUniqueWithinInheritanceTreeValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyNamesAreUniqueWithinInheritanceTreeValidationRule _validationRule;
    private ReflectionBasedClassDefinition _baseOfBaseClassDefinition;
    private ReflectionBasedClassDefinition _derivedBaseClassDefinition;
    private ReflectionBasedClassDefinition _derivedClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyNamesAreUniqueWithinInheritanceTreeValidationRule();
      _baseOfBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseOfBaseDomainObject",
          "BaseEntityName",
          StorageProviderDefinition,
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      _derivedBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          false,
          _baseOfBaseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));
      _derivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false,
          _derivedBaseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));
    }

    [Test]
    public void HasNoBaseClass_And_HasNoPropertyDefintions ()
    {
      Assert.That (_validationRule.Validate (_derivedBaseClassDefinition).ToArray(), Is.Empty);
    }

    [Test]
    public void HasBaseClass_And_HasNoPropertyDefintions ()
    {
      _baseOfBaseClassDefinition.SetReadOnly();
      _derivedBaseClassDefinition.SetReadOnly ();

      Assert.That (_validationRule.Validate (_derivedBaseClassDefinition).ToArray (), Is.Empty);
    }

    [Test]
    public void HasBaseClass_And_HasPropertyDefintions ()
    {
      _derivedBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "FirstName", "FirstName")}, true));
      _derivedBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "LastName", "LastName")}, true));

      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly ();

      Assert.That (_validationRule.Validate (_derivedBaseClassDefinition).ToArray (), Is.Empty);
    }

    [Test]
    public void HasBaseClass_And_HasSamePropertyDefintionsInBaseClass ()
    {
      _derivedBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "Name", "Name")}, true));
      _baseOfBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{ ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_baseOfBaseClassDefinition, "Name", "Name")}, true));

      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_derivedBaseClassDefinition);

      var expectedMessage = "Class 'BaseValidationDomainObjectClass' must not define property 'FakeProperty', because base class "
        +"'BaseOfBaseValidationDomainObjectClass' already defines a property with the same name.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ReflectionBasedPropertyDefinitionFactory\r\n"
        +"Property: FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void HasBaseClass_And_HasSamePropertyDefintionsInBaseOfBaseClass ()
    {
      _derivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] {ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedClassDefinition, "Name", "Name")}, true));
      _baseOfBaseClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] {ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_baseOfBaseClassDefinition, "Name", "Name")}, true));

      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly();
      _derivedClassDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_derivedClassDefinition);

      var expectedMessage = "Class 'DerivedValidationDomainObjectClass' must not define property 'FakeProperty', because base class "
        +"'BaseOfBaseValidationDomainObjectClass' already defines a property with the same name.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ReflectionBasedPropertyDefinitionFactory\r\n"
        +"Property: FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}