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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Reflection.TypeDiscovery;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectorTest : MappingReflectionTestBase
  {
    private IMappingLoader _mappingReflector;

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();
      _mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService ());
    }

    [Test]
    public void Initialization_DefaultTypeDiscoveryService ()
    {
      var reflector = new MappingReflector ();

      Assert.That (reflector.TypeDiscoveryService, Is.SameAs (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService ()));
    }

    [Test]
    public void GetResolveTypes ()
    {
      
      Assert.IsTrue (_mappingReflector.ResolveTypes);
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());

      var actualClassDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true);
      var actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions).ToDictionary (rd => rd.ID);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (FakeMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      Assembly assembly = GetType().Assembly;
      MappingReflector expectedMappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly));
      var expectedClassDefinitions = new ClassDefinitionCollection (expectedMappingReflector.GetClassDefinitions(), true);
      var expectedRelationDefinitions = expectedMappingReflector.GetRelationDefinitions (expectedClassDefinitions).ToDictionary (rd => rd.ID);

      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly, assembly));
      var actualClassDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
      classDefinitionChecker.Check (expectedClassDefinitions, actualClassDefinitions, false, false);

      var actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions).ToDictionary (rd => rd.ID);
      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (expectedRelationDefinitions, actualRelationDefinitions, false);
    }

    [Test]
    public void GetClassDefinitions()
    {
      Assembly assembly = GetType ().Assembly;
      var mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly, assembly));
      var classDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true);

      Assert.That (classDefinitions.Count, Is.GreaterThan (0));
    }

    [Test]
    public void CreateClassDefinitionValidator ()
    {
      var validator = (ClassDefinitionValidator) _mappingReflector.CreateClassDefinitionValidator();

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (6));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (DomainObjectTypeIsNotGenericValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (InheritanceHierarchyFollowsClassHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule)));
      Assert.That (validator.ValidationRules[5], Is.TypeOf (typeof (StorageGroupTypesAreSameWithinInheritanceTreeRule)));
    }

    [Test]
    public void CreatePropertyDefinitionValidator ()
    {
      var validator = (PropertyDefinitionValidator) _mappingReflector.CreatePropertyDefinitionValidator();

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (4));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (MappingAttributesAreSupportedForPropertyTypeValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (StorageClassIsSupportedValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (PropertyTypeIsSupportedValidationRule)));
    }

    [Test]
    public void CreateRelationDefinitionValidator ()
    {
      var validator = (RelationDefinitionValidator) _mappingReflector.CreateRelationDefinitionValidator();

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (10));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (RdbmsRelationEndPointCombinationIsSupportedValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (VirtualRelationEndPointPropertyTypeIsSupportedValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule)));
      Assert.That (validator.ValidationRules[5], Is.TypeOf (typeof (RelationEndPointPropertyTypeIsSupportedValidationRule)));
      Assert.That (validator.ValidationRules[6], Is.TypeOf (typeof (RelationEndPointNamesAreConsistentValidationRule)));
      Assert.That (validator.ValidationRules[7], Is.TypeOf (typeof (RelationEndPointTypesAreConsistentValidationRule)));
      Assert.That (validator.ValidationRules[8], Is.TypeOf (typeof (CheckForInvalidRelationEndPointsValidationRule)));
      Assert.That (validator.ValidationRules[9], Is.TypeOf (typeof (CheckForTypeNotFoundClassDefinitionValidationRule)));
    }

    [Test]
    public void CreateSortExpressionValidator ()
    {
      var validator = (SortExpressionValidator) _mappingReflector.CreateSortExpressionValidator();

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (1));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (SortExpressionIsValidValidationRule)));
    }

  }
}