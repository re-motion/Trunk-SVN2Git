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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, params Type[] persistentMixins)
    {
      return new ReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, baseClass, new PersistentMixinFinderMock (persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, IPersistentMixinFinder persistentMixinFinder)
    {
      return new ReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, baseClass, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, params Type[] persistentMixins)
    {
      return CreateReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, null, new PersistentMixinFinderMock (persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateReflectionBasedClassDefinition(id, entityName, storageProviderID, classType, isAbstract, null, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinition()
    {
      return CreateReflectionBasedClassDefinition("Order", "OrderTable", "StorageProviderID", typeof (Order), false,
          new PersistentMixinFinderMock());
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }


    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition(type.Name, type.Name, "TestDomain", type, false, new PersistentMixinFinderMock(mixins));
    }
  }
}
