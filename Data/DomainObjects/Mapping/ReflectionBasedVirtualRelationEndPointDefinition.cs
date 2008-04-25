using System;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedVirtualRelationEndPointDefinition : VirtualRelationEndPointDefinition
  {
    [NonSerialized]
    private readonly PropertyInfo _propertyInfo;

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string propertyTypeName,
        string sortExpression,
        PropertyInfo propertyInfo)
        : base (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpression,
        PropertyInfo propertyInfo)
      : base (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }
  }
}