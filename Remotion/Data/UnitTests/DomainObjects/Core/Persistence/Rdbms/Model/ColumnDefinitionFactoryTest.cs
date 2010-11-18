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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionFactoryTest
  {
    [Test]
    public void PropertyWithStorageSpecificIdentifierAttributeAttribute_GetNameFromAttribute ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithAllDataTypes));
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));

      var result = new ColumnDefinitionFactory().CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("Boolean"));
    }

    [Test]
    public void PropertyWithoutStorageSpecificIdentifierAttributeAttribute_DomainObjectPropertyType ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (FileSystemItem));
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (FileSystemItem).GetProperty ("ParentFolder"));

      var result = new ColumnDefinitionFactory().CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("ParentFolderID"));
    }

    [Test]
    public void PropertyWithoutStorageSpecificIdentifierAttributeAttribute_NoDomainObjectPropertyType ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Distributor));
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (Distributor).GetProperty ("NumberOfShops"));

      var result = new ColumnDefinitionFactory ().CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("NumberOfShops"));
    }
  }
}