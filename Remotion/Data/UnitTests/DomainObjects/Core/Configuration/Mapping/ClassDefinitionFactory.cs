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
      return new ReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, baseClass, new PersistentMixinFinderMock (classType, persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass, IPersistentMixinFinder persistentMixinFinder)
    {
      return new ReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, baseClass, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, params Type[] persistentMixins)
    {
      return CreateReflectionBasedClassDefinition (id, entityName, storageProviderID, classType, isAbstract, null, new PersistentMixinFinderMock (classType, persistentMixins));
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, IPersistentMixinFinder persistentMixinFinder)
    {
      return CreateReflectionBasedClassDefinition(id, entityName, storageProviderID, classType, isAbstract, null, persistentMixinFinder);
    }

    public static ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] mixins)
    {
      return CreateReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, mixins);
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinition()
    {
      return CreateReflectionBasedClassDefinition("Order", "OrderTable", "StorageProviderID", typeof (Order), false);
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }
  }
}
