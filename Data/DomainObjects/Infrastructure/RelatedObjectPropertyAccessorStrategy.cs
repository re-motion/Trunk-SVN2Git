using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal class RelatedObjectPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectPropertyAccessorStrategy Instance = new RelatedObjectPropertyAccessorStrategy();

    private RelatedObjectPropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition.PropertyType.Equals (typeof (ObjectID)))
        return relationEndPointDefinition.RelationDefinition.GetOppositeClassDefinition (relationEndPointDefinition).ClassType;
      else
        return relationEndPointDefinition.PropertyType;      
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return new RelationEndPointID (propertyAccessor.DomainObject.ID, propertyAccessor.RelationEndPointDefinition);
    }

    public RelationEndPoint GetRelationEndPoint (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.DataManager.RelationEndPointMap[CreateRelationEndPointID (propertyAccessor)];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      RelationEndPoint endPoint = GetRelationEndPoint (propertyAccessor, transaction);
      return endPoint != null && endPoint.HasChanged;
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      RelationEndPoint endPoint = GetRelationEndPoint (propertyAccessor, transaction);
      return endPoint != null && endPoint.HasBeenTouched;
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("accessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (propertyAccessor.RelationEndPointDefinition.IsVirtual)
        return GetValueWithoutTypeCheck (propertyAccessor, transaction) == null;
      else // for nonvirtual end points check out the ObjectID, which is stored in the DataContainer; this allows IsNull to avoid loading the object
        return ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck (propertyAccessor, transaction) == null;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      transaction.SetRelatedObject (CreateRelationEndPointID (propertyAccessor), (DomainObject) value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetOriginalRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
