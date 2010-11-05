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
    [ExpectedException(typeof(MappingException),
      ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration."
      +"NotSupportedPersistenceMapping.SamePropertyNameInInheritanceHierarchy.DerivedDerivedClass.DuplicatedPropertyInTree' of class "
      +"'SamePropertyNameInInheritanceHierarchy_DerivedDerivedClass' must not define storage specific name 'DuplicatedPropertyInTree', because class "
      +"'SamePropertyNameInInheritanceHierarchy_BaseClass' in same inheritance hierarchy already defines property 'Remotion.Data.UnitTests."
      +"DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping.SamePropertyNameInInheritanceHierarchy."
      +"BaseClass.DuplicatedPropertyInTree' with the same storage specific name.")]
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
      ExpectedMessage = "The domain object type cannot redefine the 'Remotion.Data.DomainObjects.StorageGroupAttribute' already defined on base type "
      +"'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"DuplicatedStorageGroupAttributeInInheritanceHierarchy.BaseClass'.")]
    public void DuplicatedStorageGroupAttributeInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.DuplicatedStorageGroupAttributeInInheritanceHierarchy");
    }

    //NonAbstractClassHasEntityNameValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = @"Neither class 'ConcreteClassAboveInheritanceRoot_ConcreteClassAboveInheritanceRoot' nor its base classes specify an entity name\. "
      +@"Make class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +@"ConcreteClassAboveInheritanceRoot.ClassAboveInheritanceRoot, Remotion.Data.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' abstract or "
      +@"apply a DBTable attribute to it or one of its base classes\.", MatchType = MessageMatch.Regex)]
    public void ConcreteClassAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.ConcreteClassAboveInheritanceRoot");
    }

    //MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The 'Remotion.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's "
      +"base definition.\r\n"
      + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration.NotSupportedPersistenceMapping."
      +"MappingAttributeAppliedOnOverriddenProperty.DerivedClass, property: Property")]
    public void MappingAttributeAppliedOnOverriddenProperty ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.MappingAttributeAppliedOnOverriddenProperty");
    }
  }
}