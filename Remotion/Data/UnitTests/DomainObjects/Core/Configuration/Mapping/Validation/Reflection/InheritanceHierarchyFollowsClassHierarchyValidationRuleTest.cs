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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Reflection
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
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, "SPID", baseType, false);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (derivedType.Name, derivedType.Name, "SPID", derivedType, false, baseClassDefinition, new Type[0]);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsNotDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, "SPID", baseType, false);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (derivedType.Name, derivedType.Name, "SPID", derivedType, false, baseClassDefinition, new Type[0]);
      PrivateInvoke.SetNonPublicField (classDefinition, "_classType", typeof (ClassOutOfInheritanceHierarchy));

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
          "Type 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.ClassOutOfInheritanceHierarchy, Remotion.Data.UnitTests, Version=1.13.60.2, Culture=neutral, PublicKeyToken=fee00910d6e5f53b' of class 'BaseValidationDomainObjectClass' is not derived from type 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass, Remotion.Data.UnitTests, Version=1.13.60.2, Culture=neutral, PublicKeyToken=fee00910d6e5f53b' of base class 'BaseOfBaseValidationDomainObjectClass'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}