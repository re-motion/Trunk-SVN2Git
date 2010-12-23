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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
        StorageProviderDefinition storageProviderDefinition,
        Type classType,
        bool isAbstract,
        ReflectionBasedClassDefinition baseClass,
        params Type[] persistentMixins)
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          id,
          classType,
          isAbstract,
          baseClass,
          null,
          new PersistentMixinFinderMock (classType, persistentMixins));
      SetStorageEntityName (entityName, storageProviderDefinition, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
        StorageProviderDefinition storageProviderDefinition,
        Type classType,
        bool isAbstract,
        ReflectionBasedClassDefinition baseClass,
        Type storageGroupType,
        IPersistentMixinFinder persistentMixinFinder)
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          id,
          classType,
          isAbstract,
          baseClass,
          storageGroupType,
          persistentMixinFinder);
      SetStorageEntityName (entityName, storageProviderDefinition, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageEntity (
        string id,
        string entityName,
        Type classType,
        bool isAbstract,
        ReflectionBasedClassDefinition baseClass,
        IPersistentMixinFinder persistentMixinFinder)
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          id,
          classType,
          isAbstract,
          baseClass,
          null,
          persistentMixinFinder);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
        StorageProviderDefinition storageProviderDefinition,
        Type classType,
        bool isAbstract,
        params Type[] persistentMixins)
    {
      return CreateReflectionBasedClassDefinition (
          id, entityName, storageProviderDefinition, classType, isAbstract, null, null, new PersistentMixinFinderMock (classType, persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
        StorageProviderDefinition storageProviderDefinition,
        Type classType,
        bool isAbstract,
        IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateReflectionBasedClassDefinition (
          id, entityName, storageProviderDefinition, classType, isAbstract, null, null, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, null, type, false, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        Type type, StorageProviderDefinition storageProviderDefinition, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, storageProviderDefinition, type, false, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        Type type, ReflectionBasedClassDefinition baseClass, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, null, type, false, baseClass, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        Type type, StorageProviderDefinition storageProviderDefinition, ReflectionBasedClassDefinition baseClass, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, storageProviderDefinition, type, false, baseClass, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageEntity (
        Type type, ReflectionBasedClassDefinition baseClass)
    {
      return CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          type.Name, type.Name, type, false, baseClass, new PersistentMixinFinder (type));
    }

    public static ReflectionBasedClassDefinition CreateFinishedClassDefinition (Type classType, ReflectionBasedClassDefinition baseClass)
    {
      var classDefinition = CreateReflectionBasedClassDefinition (classType, baseClass);

      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateFinishedClassDefinition (Type classType)
    {
      return CreateFinishedClassDefinition (classType, null);
    }

    public static ReflectionBasedClassDefinition CreateFinishedOrderDefinition ()
    {
      return CreateFinishedClassDefinition (typeof (Order));
    }

    public static ReflectionBasedClassDefinition CreateFinishedFileSystemItemDefinitionWithDerivedClasses ()
    {
      var fileSystemItemClassDefinition = CreateReflectionBasedClassDefinition (
          typeof (FileSystemItem), (ReflectionBasedClassDefinition) null, Type.EmptyTypes);
      var fileClassDefinition = CreateFinishedClassDefinition (typeof (File), fileSystemItemClassDefinition);
      var folderClassDefinition = CreateFinishedClassDefinition (typeof (Folder), fileSystemItemClassDefinition);

      fileSystemItemClassDefinition.SetDerivedClasses (new ClassDefinitionCollection { fileClassDefinition, folderClassDefinition });
      fileSystemItemClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      return fileSystemItemClassDefinition;
    }

    private static void SetStorageEntityName (
        string entityName, StorageProviderDefinition storageProviderDefinition, ReflectionBasedClassDefinition classDefinition)
    {
      if (storageProviderDefinition == null)
        storageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      if (entityName != null)
      {
        classDefinition.SetStorageEntity (
            new TableDefinition (
                storageProviderDefinition, entityName, classDefinition.ID + "View", new SimpleColumnDefinition[0], new ITableConstraintDefinition[0]));
      }
      else
      {
        var entityStub = MockRepository.GenerateStub<IStorageEntityDefinition>();
        entityStub.Stub (stub => stub.LegacyEntityName).Return (null);
        entityStub.Stub (stub => stub.StorageProviderDefinition).Return (storageProviderDefinition);

        classDefinition.SetStorageEntity (entityStub);
      }
    }
  }
}