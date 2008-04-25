using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal class RelatedObjectCollectionPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectCollectionPropertyAccessorStrategy Instance = new RelatedObjectCollectionPropertyAccessorStrategy();

    private RelatedObjectCollectionPropertyAccessorStrategy ()
    {
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return RelatedObjectPropertyAccessorStrategy.Instance.CreateRelationEndPointID (propertyAccessor);
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      return RelatedObjectPropertyAccessorStrategy.Instance.GetPropertyType (propertyDefinition, relationEndPointDefinition);
    }

    public bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return RelatedObjectPropertyAccessorStrategy.Instance.HasChanged (propertyAccessor, transaction);
    }

    public bool HasBeenTouched (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return RelatedObjectPropertyAccessorStrategy.Instance.HasBeenTouched (propertyAccessor, transaction);
    }

    public bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("accessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return false;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      throw new InvalidOperationException ("Related object collections cannot be set.");
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetOriginalRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
