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
        return relationEndPointDefinition.GetOppositeClassDefinition ().ClassType;
      else
        return relationEndPointDefinition.PropertyType;      
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return new RelationEndPointID (propertyAccessor.DomainObject.ID, propertyAccessor.PropertyData.RelationEndPointDefinition);
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

      if (propertyAccessor.PropertyData.RelationEndPointDefinition.IsVirtual)
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

      transaction.GetObject (propertyAccessor.DomainObject.ID, true); // ensure that the object is actually loaded before accessing the end-point

      var endPointID = CreateRelationEndPointID (propertyAccessor);
      var endPoint = (ObjectEndPoint) transaction.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      endPoint.SetOppositeObjectAndNotify ((DomainObject) value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      return transaction.GetOriginalRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
