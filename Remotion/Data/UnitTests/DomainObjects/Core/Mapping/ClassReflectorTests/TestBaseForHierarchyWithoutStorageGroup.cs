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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ClassReflectorTests
{
  public class TestBaseForHierarchyWithoutStorageGroup : MappingReflectionTestBase
  {
    protected void CreatePropertyDefinitionsForClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithoutStorageGroupWithMixedProperties), "Int32", "Int32", typeof (int), null, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithoutStorageGroupWithMixedProperties), "String", "String", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithoutStorageGroupWithMixedProperties), "PrivateString", "PrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add( ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithoutStorageGroupWithMixedProperties), "UnidirectionalOneToOne", "UnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (properties);
    }

    protected void CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition> ();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithoutStorageGroupWithMixedProperties), "String", "NewString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add(ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithoutStorageGroupWithMixedProperties), "PrivateString", "DerivedPrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithoutStorageGroupWithMixedProperties), "OtherString", "OtherString", typeof (string), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (properties);
    }
  }
}
