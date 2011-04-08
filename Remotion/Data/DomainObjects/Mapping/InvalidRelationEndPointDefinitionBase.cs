using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class InvalidRelationEndPointDefinitionBase : IRelationEndPointDefinition
  {
    private readonly ClassDefinition _classDefinition;
    private readonly string _propertyName;
    private readonly Type _propertyType;
    private RelationDefinition _relationDefinition;

    public InvalidRelationEndPointDefinitionBase (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      _classDefinition = classDefinition;
      _propertyName = propertyName;
      _propertyType = propertyType;
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName;  }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _relationDefinition; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public PropertyInfo PropertyInfo
    {
      get { return null; }
    }

    public bool IsPropertyInfoResolved
    {
      get { return false; }
    }

    public bool IsMandatory
    {
      get { throw new NotImplementedException(); }
    }

    public CardinalityType Cardinality
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsVirtual
    {
      get { return false;  }
    }

    public bool IsAnonymous
    {
      get { return false;  }
    }

    public bool CorrespondsTo (string classID, string propertyName)
    {
      throw new NotImplementedException();
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }
  }
}