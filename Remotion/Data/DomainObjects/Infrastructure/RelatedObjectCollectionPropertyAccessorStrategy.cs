// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

      RelationEndPointID id = CreateRelationEndPointID(propertyAccessor);
      CollectionEndPoint endPoint = (CollectionEndPoint) transaction.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      endPoint.ReplaceOppositeCollection ((DomainObjectCollection) value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetOriginalRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
