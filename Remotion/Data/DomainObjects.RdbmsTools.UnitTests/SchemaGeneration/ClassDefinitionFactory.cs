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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id, string entityName, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          id,
          classType,
          isAbstract,
          baseClass,
          null,
          new PersistentMixinFinder (classType));
      SetStorageEntityName (entityName, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
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
      SetStorageEntityName (entityName, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageDefinition (
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

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, type, false, null);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, ReflectionBasedClassDefinition baseClass)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, type, false, baseClass);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageDefinition (
        Type type, ReflectionBasedClassDefinition baseClass)
    {
      return CreateReflectionBasedClassDefinitionWithoutStorageDefinition (
          type.Name, type.Name, type, false, baseClass, new PersistentMixinFinder (type));
    }

    private static void SetStorageEntityName (string entityName, ReflectionBasedClassDefinition classDefinition)
    {
      var storageProviderDefinition = new RdbmsProviderDefinition ("DefaultStorageProvider", typeof (SqlStorageObjectFactory), "dummy");
      if (entityName != null)
        classDefinition.SetStorageEntity (
            new TableDefinition (storageProviderDefinition, entityName, classDefinition.ID + "View", new ColumnDefinition[0]));
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