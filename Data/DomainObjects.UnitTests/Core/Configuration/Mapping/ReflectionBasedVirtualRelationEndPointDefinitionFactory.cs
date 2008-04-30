using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  public static class ReflectionBasedVirtualRelationEndPointDefinitionFactory
  {
    private static PropertyInfo s_dummyPropertyInfo = typeof (Order).GetProperty ("OrderNumber");

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, string propertyTypeName, string sortExpression)
    {
      return new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpression, s_dummyPropertyInfo);
    }

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType, string sortExpression)
    {
      return new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression, s_dummyPropertyInfo);
    }

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType)
    {
      return CreateReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, null);
    }
  }
}