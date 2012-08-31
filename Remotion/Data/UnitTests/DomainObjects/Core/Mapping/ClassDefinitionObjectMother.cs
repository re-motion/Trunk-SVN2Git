// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public static class ClassDefinitionObjectMother
  {
    public static ClassDefinition CreateClassDefinition ()
    {
      return CreateClassDefinition (typeof (Order));
    }

    public static ClassDefinition CreateClassDefinition (Type classType)
    {
      return CreateClassDefinition ("Test", classType);
    }

    public static ClassDefinition CreateClassDefinition (Type type, ClassDefinition baseClass)
    {
      return CreateClassDefinition (type.Name, type, false, baseClass, null, new PersistentMixinFinderStub (type));
    }

    public static ClassDefinition CreateClassDefinition (string id)
    {
      return CreateClassDefinition (id, typeof (Order));
    }

    public static ClassDefinition CreateClassDefinition (string id, ClassDefinition baseClass)
    {
      return CreateClassDefinition (id, typeof (Order), false, baseClass, null, new PersistentMixinFinderStub (typeof (Order)));
    }

    public static ClassDefinition CreateClassDefinition (string id, Type classType)
    {
      return CreateClassDefinition (id, classType, false, null, null, new PersistentMixinFinderStub (classType));
    }

    public static ClassDefinition CreateClassDefinition (string id, Type classType, ClassDefinition baseClass)
    {
      return CreateClassDefinition (id, classType, false, baseClass, null, new PersistentMixinFinderStub (classType));
    }
    
    public static ClassDefinition CreateClassDefinition (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        Type storageGroupType,
        IPersistentMixinFinder persistentMixinFinder)
    {
      var instanceCreator = InterceptedDomainObjectCreator.Instance;
      return CreateClassDefinition(id, classType, isAbstract, baseClass, storageGroupType, persistentMixinFinder, instanceCreator);
    }

    public static ClassDefinition CreateClassDefinition (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        Type storageGroupType,
        IPersistentMixinFinder persistentMixinFinder,
        InterceptedDomainObjectCreator instanceCreator)
    {
      return new ClassDefinition (id, classType, isAbstract, baseClass, storageGroupType, persistentMixinFinder, instanceCreator);
    }

    public static ClassDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (Type classType)
    {
      return CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (classType, null);
    }

    public static ClassDefinition CreateClassDefinition_WithEmptyMembers_AndDerivedClasses (Type classType, ClassDefinition baseClass)
    {
      var classDefinition = CreateClassDefinition (classType, baseClass);

      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      return classDefinition;
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
      return new ClassDefinition (
          type.Name, type, false, baseClassDefinition, storageGroupType, new PersistentMixinFinderStub (type), InterceptedDomainObjectCreator.Instance);
    }

    public static ClassDefinition CreateClassDefinitionWithMixins (Type type, params Type[] mixins)
    {
      return CreateClassDefinition (type.Name, type, false, null, null, new PersistentMixinFinderStub (type, mixins));
    }

    public static ClassDefinition CreateClassDefinitionWithAbstractFlag (bool isAbstract)
    {
      return CreateClassDefinitionWithAbstractFlag (isAbstract, typeof (Order));
    }

    public static ClassDefinition CreateClassDefinitionWithAbstractFlag (bool isAbstract, Type classType)
    {
      return CreateClassDefinition ("Test", classType, isAbstract, null, null, new PersistentMixinFinderStub (classType));
    }

    public static ClassDefinition CreateClassDefinitionWithMixinFinder (IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateClassDefinition ("Test", typeof (Order), false, null, null, persistentMixinFinder);
    }

    public static ClassDefinition CreateClassDefinitionWithMixinFinder (Type classType, IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateClassDefinition ("Test", classType, false, null, null, persistentMixinFinder);
    }


  }
}