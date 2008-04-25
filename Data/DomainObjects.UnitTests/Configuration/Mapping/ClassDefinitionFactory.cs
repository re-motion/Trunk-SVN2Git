using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateOrderDefinition()
    {
      return new ReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order), false,
          new List<Type> ());
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }
  }
}