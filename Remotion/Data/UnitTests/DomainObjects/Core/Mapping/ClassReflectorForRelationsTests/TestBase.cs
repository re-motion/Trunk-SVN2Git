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
  public class TestBase: MappingReflectionTestBase
  {
    protected void CreatePropertyDefinitionsForClassWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BaseString", "BaseString", typeof (string), null, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne", "BaseUnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne", "BasePrivateUnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentProperties), "Int32", "Int32", typeof (int), null, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentProperties), "String", "String", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentProperties), "PrivateString", "PrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne", "UnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    protected void CreatePropertyDefinitionsForDerivedClassWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition> ();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithDifferentProperties), "String", "NewString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithDifferentProperties), "PrivateString", "DerivedPrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create(classDefinition, typeof (DerivedClassWithDifferentProperties), "OtherString", "OtherString", typeof (string), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }
  }
}
