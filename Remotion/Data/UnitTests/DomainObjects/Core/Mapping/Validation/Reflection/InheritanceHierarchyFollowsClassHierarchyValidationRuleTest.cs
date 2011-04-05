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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class InheritanceHierarchyFollowsClassHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private InheritanceHierarchyFollowsClassHierarchyValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new InheritanceHierarchyFollowsClassHierarchyValidationRule();
    }

    [Test]
    public void ClassDefinitionWithoutBaseClass ()
    {
      var type = typeof (BaseOfBaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, StorageProviderDefinition, type, false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, StorageProviderDefinition, baseType, false);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (derivedType.Name, derivedType.Name, StorageProviderDefinition, derivedType, false, baseClassDefinition, new Type[0]);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsNotDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, StorageProviderDefinition, baseType, false);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (derivedType.Name, derivedType.Name, StorageProviderDefinition, derivedType, false, baseClassDefinition, new Type[0]);
      PrivateInvoke.SetNonPublicField (classDefinition, "_classType", typeof (ClassOutOfInheritanceHierarchy));

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
         @"Type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.ClassOutOfInheritanceHierarchy, Remotion.Data.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' of class 'BaseValidationDomainObjectClass' is not derived from type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass, Remotion.Data.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' of base class 'BaseOfBaseValidationDomainObjectClass'\.";
      var regex = new Regex (expectedMessage);
      Assert.That (validationResult.IsValid, Is.False);
      Assert.That (regex.IsMatch (validationResult.Message), Is.True);
    }
  }
}