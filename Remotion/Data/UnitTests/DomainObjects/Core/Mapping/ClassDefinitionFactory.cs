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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
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
        string storageProviderID,
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
      SetStorageEntityName (entityName, classDefinition);
      SetStorageProviderDefinition (storageProviderID, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id,
        string entityName,
        string storageProviderID,
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
      SetStorageProviderDefinition (storageProviderID, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageEntity (
        string id,
        string entityName,
        string storageProviderID,
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
      SetStorageProviderDefinition (storageProviderID, classDefinition);
      return classDefinition;
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, params Type[] persistentMixins)
    {
      return CreateReflectionBasedClassDefinition (
          id, entityName, storageProviderID, classType, isAbstract, null, null, new PersistentMixinFinderMock (classType, persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, null, null, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, ReflectionBasedClassDefinition baseClass,  params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, baseClass, mixins);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinitionWithoutStorageEntity (Type type, ReflectionBasedClassDefinition baseClass)
    {
      return CreateReflectionBasedClassDefinitionWithoutStorageEntity (type.Name, type.Name, null, type, false, baseClass, new PersistentMixinFinder (type));
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinition ()
    {
      return CreateReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order), false);
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty ()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (Order), "Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }

    private static void SetStorageEntityName (string entityName, ReflectionBasedClassDefinition classDefinition)
    {
      if (entityName != null)
        classDefinition.SetStorageEntity (new TableDefinition ("SPID", entityName, classDefinition.ID+"View", new ColumnDefinition[0]));
      else
      {
        var entityStub = MockRepository.GenerateStub<IStorageEntityDefinition>();
        entityStub.Stub (stub => stub.LegacyEntityName).Return (null);

        classDefinition.SetStorageEntity (entityStub);
      }
    }

    private static void SetStorageProviderDefinition (string storageProviderID, ReflectionBasedClassDefinition classDefinition)
    {
      if (!string.IsNullOrEmpty (storageProviderID))
      {
        classDefinition.SetStorageProviderDefinition (
            new UnitTestStorageProviderStubDefinition (storageProviderID, typeof (UnitTestStorageObjectFactoryStub)));
      }
    }

  }
}