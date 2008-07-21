/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.ClassReflectorTests
{
  public class TestBaseForHierarchyWithoutStorageGroup : StandardMappingTest
  {
    protected void CreatePropertyDefinitionsForClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.Int32", "Int32", typeof (int), null, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String", "String", typeof (string), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.PrivateString", "PrivateString", typeof (string), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.UnidirectionalOneToOne", "UnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
    }

    protected void CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.String", "NewString", typeof (string), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.PrivateString", "DerivedPrivateString", typeof (string), true, null, StorageClass.Persistent));

      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.OtherString", "OtherString", typeof (string), true, null, StorageClass.Persistent));
    }
  }
}
