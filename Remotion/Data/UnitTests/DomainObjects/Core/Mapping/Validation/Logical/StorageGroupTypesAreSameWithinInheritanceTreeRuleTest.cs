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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class StorageGroupTypesAreSameWithinInheritanceTreeRuleTest : ValidationRuleTestBase
  {
    private StorageGroupTypesAreSameWithinInheritanceTreeRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageGroupTypesAreSameWithinInheritanceTreeRule();
    }

    [Test]
    public void ClassWithoutBaseClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), typeof (DBStorageGroupAttribute), null);
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithoutStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), null, null);
      var derivedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), null, baseClassDefinition);

      var validationResult = _validationRule.Validate (derivedClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithSameStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), typeof(DBStorageGroupAttribute), null);
      var derivedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), typeof(DBStorageGroupAttribute), baseClassDefinition);

      var validationResult = _validationRule.Validate (derivedClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithDifferentStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseOfBaseValidationDomainObjectClass), typeof (DBStorageGroupAttribute), null);
      var derivedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithStorageGroup (typeof (BaseValidationDomainObjectClass), typeof (StubStorageGroup1Attribute), baseClassDefinition);

      var validationResult = _validationRule.Validate (derivedClassDefinition);

      var expectedMessage = "Class 'BaseValidationDomainObjectClass' must have the same storage group type as its base class "
        +"'BaseOfBaseValidationDomainObjectClass'.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}