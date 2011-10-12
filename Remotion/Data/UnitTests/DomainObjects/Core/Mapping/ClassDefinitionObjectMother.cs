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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public static class ClassDefinitionObjectMother
  {
    public static ClassDefinition CreateClassDefinition (string id, Type classType, bool isAbstract, ClassDefinition baseClass)
    {
      return new ClassDefinition (id, classType, isAbstract, baseClass, null, new PersistentMixinFinderMock (classType));
    }

    public static ClassDefinition CreateClassDefinition (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        Type storageGroupType,
        IPersistentMixinFinder persistentMixinFinder)
    {
      return new ClassDefinition (id, classType, isAbstract, baseClass, storageGroupType, persistentMixinFinder);
    }

    public static ClassDefinition CreateClassDefinition (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        IPersistentMixinFinder persistentMixinFinder)
    {
      return new ClassDefinition (id, classType, isAbstract, baseClass, null, persistentMixinFinder);
    }

    public static ClassDefinition CreateClassDefinition (string id, Type classType, bool isAbstract)
    {
      return CreateClassDefinition (id, classType, isAbstract, (ClassDefinition) null);
    }

    public static ClassDefinition CreateClassDefinition (string id, Type classType, bool isAbstract, IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateClassDefinition (id, classType, isAbstract, null, null, persistentMixinFinder);
    }

    public static ClassDefinition CreateClassDefinition (Type classType)
    {
      return CreateClassDefinition (classType, null);
    }

    public static ClassDefinition CreateClassDefinition (Type type, ClassDefinition baseClass)
    {
      return CreateClassDefinition (type.Name, type, false, baseClass);
    }

    public static ClassDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (Type classType, ClassDefinition baseClass)
    {
      var classDefinition = CreateClassDefinition (classType, baseClass);

      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      return classDefinition;
    }

    public static ClassDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (Type classType)
    {
      return CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (classType, null);
    }

    public static ClassDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (typeof (Order));
    }

    public static ClassDefinition CreateFileSystemItemDefinition_WithEmptyMembers_AndWithDerivedClasses ()
    {
      var fileSystemItemClassDefinition = CreateClassDefinitionWithMixins (typeof (FileSystemItem));
      var fileClassDefinition = CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (typeof (File), fileSystemItemClassDefinition);
      var folderClassDefinition = CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (typeof (Folder), fileSystemItemClassDefinition);

      fileSystemItemClassDefinition.SetDerivedClasses (new [] { fileClassDefinition, folderClassDefinition });
      fileSystemItemClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      return fileSystemItemClassDefinition;
    }

    public static ClassDefinition CreateClassDefinitionWithTable (Type type, StorageProviderDefinition storageProviderDefinition)
    {
      var classDefinition = CreateClassDefinition (type);
      classDefinition.SetStorageEntity (TableDefinitionObjectMother.Create (storageProviderDefinition));
      return classDefinition;
    }

    public static ClassDefinition CreateClassDefinitionWithStorageGroup (Type type, Type storageGroupType)
    {
      return CreateClassDefinitionWithStorageGroup (type, storageGroupType, null);
    }

    public static ClassDefinition CreateClassDefinitionWithStorageGroup (Type type, Type storageGroupType, ClassDefinition baseClassDefinition)
    {
      return new ClassDefinition (type.Name, type, false, baseClassDefinition, storageGroupType, new PersistentMixinFinderMock (type));
    }

    public static ClassDefinition CreateClassDefinitionWithMixins (Type type, params Type[] mixins)
    {
      return CreateClassDefinition (type.Name, type, false, new PersistentMixinFinderMock (type, mixins));
    }

  }
}