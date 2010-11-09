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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class NotSupportedPersistenceMappingIntegrationTests : ValidationIntegrationTestBase
  {
    //StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule
    [Test]
    [ExpectedException(typeof(MappingException), ExpectedMessage = 
      "Property 'DuplicatedPropertyInTree' of class 'DerivedDerivedClass' must not define storage specific name 'DuplicatedPropertyInTree', "
      +"because class 'BaseClass' in same inheritance hierarchy already defines property 'DuplicatedPropertyInTree' with the same storage specific name.\r\n\r\n"
      +"Declaring type: DerivedDerivedClass\r\n"
      +"Property: DuplicatedPropertyInTree")]
    public void SamePropertyNameInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SamePropertyNameInInheritanceHierarchy");
    }

    //Exception is thrown in ClassDefinitionCollection
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = @"Class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration."
      +@"NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.BaseClass' and 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
      +@"TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.DerivedDerivedClass' both have "
      +@"the same class ID 'SameClassNameInInheritanceHierarchy_DuplicatedClassName'\. Use the ClassIDAttribute to define unique IDs for these classes\. "
      +@"The assemblies involved are 'Remotion.Data.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' and 'Remotion.Data.UnitTests, Version=.*, "
      +@"Culture=.*, PublicKeyToken=.*'\.", MatchType = MessageMatch.Regex)]
    public void SameClassNameInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy");
    }

    //StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The domain object type cannot redefine the 'StorageGroupAttribute' already defined on base type 'BaseClass'.\r\n\r\n"
      + "Declaration type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"DuplicatedStorageGroupAttributeInInheritanceHierarchy.DerivedDerivedClass'")]
    public void DuplicatedStorageGroupAttributeInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.DuplicatedStorageGroupAttributeInInheritanceHierarchy");
    }

    //NonAbstractClassHasEntityNameValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Neither class 'ClassAboveInheritanceRoot' nor its base classes specify an entity name. Make class 'ClassAboveInheritanceRoot' abstract or apply "
      +"a 'DBTable' attribute to it or one of its base classes.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"ConcreteClassAboveInheritanceRoot.ClassAboveInheritanceRoot'")]
    public void ConcreteClassAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.ConcreteClassAboveInheritanceRoot");
    }

    //MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
      + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"MappingAttributeAppliedOnOverriddenProperty.DerivedClass\r\nProperty: Property")]
    public void MappingAttributeAppliedOnOverriddenProperty ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.MappingAttributeAppliedOnOverriddenProperty");
    }

    //EntityNameMatchesParentEntityNameValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
      "Class 'DerivedClass' must not specify an entity name 'DerivedName' which is different from inherited entity name 'BaseName'.\r\n\r\n"
      +"Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"SameEntityNamesInInheritanceHierarchy.DerivedClass")]
    public void SameEntityNamesInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SameEntityNamesInInheritanceHierarchy");
    }
  
  }
}